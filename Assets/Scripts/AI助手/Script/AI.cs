using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using System;

public class AI : MonoBehaviour
{
    // 输入的信息
    [SerializeField] private InputField m_InputWord;
    // HttpRequestExample 脚本的引用
    private HttpRequestExample m_HttpRequestExample;
    private APICaller m_ApiCaller;
    private Coroutine currentCoroutine; // 存储当前正在运行的协程
    private ChatHistoryManager chatHistoryManager; // 聊天记录管理
    private ChatUIManager chatUIManager; // UI管理

    private string currentChatId; // 当前对话ID
    private List<ChatHistoryManager.ChatMessage> chatHistory = new List<ChatHistoryManager.ChatMessage>(); // 聊天记录

    [SerializeField] private Button m_NewChatButton; // 新建对话按钮

    void Awake()
    {
        // 获取 HttpRequestExample、ChatHistoryManager 和 ChatUIManager 脚本的引用
        m_HttpRequestExample = GetComponent<HttpRequestExample>();
        chatHistoryManager = GetComponent<ChatHistoryManager>();
        chatUIManager = GetComponent<ChatUIManager>();
        m_ApiCaller = GetComponent<APICaller>();

        // 初始化按钮状态
        chatUIManager.UpdateButtonState(false);
        chatUIManager.m_SendButton.onClick.AddListener(SendData);
        m_NewChatButton.onClick.AddListener(CreateNewChat);

        // 加载所有聊天记录会话
        chatHistoryManager.LoadAllChatSessions();

        // 加载最新对话
        LoadLatestChat();
        // 加载聊天记录按钮
        // chatUIManager.LoadChatRecordsButtons(chatHistoryManager.chatSessions);
        chatUIManager.LoadChatRecordsButtons(
            chatHistoryManager.chatSessions,
            OnChatRecordButtonClicked // 直接传入回调方法
        );
        // 在Awake方法末尾添加
        chatUIManager.LoadKnowledgeCards(chatHistoryManager.chatSessions);
    }

    // 发送信息
    public void SendData()
    {
        if (chatUIManager.isProcessing)
        {
            // 如果正在处理，取消请求
            StopCoroutine(currentCoroutine);
            chatUIManager.UpdateButtonState(false);
            return;
        }

        if (string.IsNullOrEmpty(m_InputWord.text))
            return;

        string _msg = m_InputWord.text;
        chatUIManager.ShowUserMessage(_msg);

        // 保存用户消息到历史记录
        chatHistory.Add(new ChatHistoryManager.ChatMessage("user", _msg));
        SaveChatHistory();

        // 开始请求
        chatUIManager.UpdateButtonState(true);
        currentCoroutine = m_HttpRequestExample.SendRequest(_msg, CallBack);
        m_InputWord.text = "";
    }

    // 回调函数，处理API响应
    private void CallBack(string _callback)
    {
        if (!string.IsNullOrEmpty(_callback))
        {
            chatUIManager.ShowAIReply(_callback, "");

            // 保存AI回复到历史记录
            chatHistory.Add(new ChatHistoryManager.ChatMessage("assistant", _callback));
            SaveChatHistory();

            // 获取最新的用户提问和模型回答
            string userQuestion = chatHistory[chatHistory.Count - 2].content;
            string aiAnswer = chatHistory[chatHistory.Count - 1].content;

            // 组合成完整的对话
            string fullConversation = $"用户: {userQuestion}\nAI: {aiAnswer}";
            Debug.Log("完整对话: " + fullConversation);

            // 添加提示词
            string prompt = "分析这段对话，检查AI回答的内容是否有错误，如果没有就返回“没有错误”，如果有就返回错误点个数，并指出哪里错误，正确应该是什么样，要求回答要精简准确";
            string analysisRequest = $"{fullConversation}\n{prompt}";
            Debug.Log("分析请求: " + analysisRequest);

            // 发送分析请求
            m_ApiCaller.MakeRequest(analysisRequest, (analysisResult) =>
            {
                // 更新最新的 AI 回复的review字段
                chatHistory[chatHistory.Count - 1].review = analysisResult;
                SaveChatHistory(); // 保存更新后的聊天记录
                Debug.Log("分析结果: " + analysisResult);
                // 获取最新的AI回复的聊天气泡
                ChatPrefab aiChat = chatUIManager.m_rootTrans.transform.GetComponentsInChildren<ChatPrefab>()
                    .Where(c => c.GetComponentInChildren<Text>().text == aiAnswer).Last();

                // 如果找到气泡，更新审查文本
                if (aiChat != null)
                {
                    aiChat.SetReviewText(analysisResult);
                }
            });

        }

        // 请求完成，更新按钮状态
        chatUIManager.UpdateButtonState(false);
    }

    private void AnalysisCallback(string _callback)
    {
        Debug.Log("分析结果: " + _callback);
    }

    // 创建新对话
    private void CreateNewChat()
    {
        // 清除当前聊天显示
        chatUIManager.ClearChatDisplay();

        // 生成新对话ID
        currentChatId = Guid.NewGuid().ToString();
        chatHistory = new List<ChatHistoryManager.ChatMessage>();

        // 保存新对话
        SaveChatHistory();
        // 重新加载聊天记录按钮
        // chatUIManager.LoadChatRecordsButtons(chatHistoryManager.chatSessions);
        chatUIManager.LoadChatRecordsButtons(
            chatHistoryManager.chatSessions,
            OnChatRecordButtonClicked // 直接传入回调方法
        );
        // 调用知识点总结
        GetComponent<KnowledgeSummary>().OnChatSwitched(currentChatId);
    }

    // 加载最新对话
    private void LoadLatestChat()
    {
        // 首先尝试加载当前活跃的聊天ID
        string activeChatId = chatHistoryManager.GetActiveChatId();
        if (activeChatId != null && chatHistoryManager.chatSessions.ContainsKey(activeChatId))
        {
            var session = chatHistoryManager.LoadChatSession(activeChatId);
            chatHistory = session.messages;
            currentChatId = activeChatId;
            // 显示消息
            foreach (var message in chatHistory)
            {
                if (message.role == "user")
                {
                    chatUIManager.ShowUserMessage(message.content);
                }
                else
                {
                    chatUIManager.ShowAIReply(message.content, message.review);
                }
            }
            return;
        }

        // 如果没有活跃聊天记录，则按修改时间加载最新的
        if (chatHistoryManager.chatSessions.Count == 0)
        {
            CreateNewChat();
            return;
        }

        var latestSession = chatHistoryManager.chatSessions.Values.OrderByDescending(s => s.lastModified).First();
        currentChatId = latestSession.sessionId;
        chatHistory = latestSession.messages;

        // 显示消息
        foreach (var message in chatHistory)
        {
            if (message.role == "user")
            {
                chatUIManager.ShowUserMessage(message.content);
            }
            else
            {
                chatUIManager.ShowAIReply(message.content, message.review);
            }
        }
    }

    // 保存聊天记录
    private void SaveChatHistory()
    {
        var wrapper = new ChatHistoryManager.ChatSessionWrapper
        {
            sessionId = currentChatId,
            lastModified = DateTime.Now,
            messages = chatHistory
        };

        chatHistoryManager.SaveChatSession(wrapper);
        chatHistoryManager.SaveActiveChatId(currentChatId);
    }

    // 处理聊天记录按钮点击事件
    public void OnChatRecordButtonClicked(string chatId)
    {
        // 加载选中的聊天记录
        var session = chatHistoryManager.LoadChatSession(chatId);
        if (session != null)
        {
            chatHistory = session.messages;
            currentChatId = chatId;

            // 显示消息
            chatUIManager.ClearChatDisplay();
            foreach (var message in chatHistory)
            {
                if (message.role == "user")
                {
                    chatUIManager.ShowUserMessage(message.content);
                }
                else
                {
                    chatUIManager.ShowAIReply(message.content, message.review);
                }
            }

            // 保存当前活跃聊天ID
            chatHistoryManager.SaveActiveChatId(currentChatId);
            // 调用知识点总结
            GetComponent<KnowledgeSummary>().OnChatSwitched(chatId);
        }
        else
        {
            Debug.LogError($"未找到聊天记录ID: {chatId}");
        }
        // 在OnChatRecordButtonClicked方法末尾添加
        chatUIManager.LoadKnowledgeCards(chatHistoryManager.chatSessions);
    }
}