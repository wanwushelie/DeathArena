using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class KnowledgeSummary : MonoBehaviour
{
    private ChatHistoryManager chatHistoryManager;
    private string currentChatId;
    private List<ChatHistoryManager.ChatMessage> chatHistory;

    // API 基本信息
    private string apiUrl = "https://dashscope.aliyuncs.com/api/v1/services/aigc/text-generation/generation";
    private string apiKey = "sk-9bc69ef7b0e444838ffddff1d1323371";

    void Awake()
    {
        chatHistoryManager = GetComponent<ChatHistoryManager>();
        Debug.Log("KnowledgeSummary Awake: " + (chatHistoryManager != null ? "ChatHistoryManager initialized" : "ChatHistoryManager not found"));
    }

    // 切换对话时调用
    public void OnChatSwitched(string newChatId)
    {
        Debug.Log("OnChatSwitched called with newChatId: " + newChatId);
        
        // 加载新对话的会话数据
        var newSession = chatHistoryManager.LoadChatSession(newChatId);
        if (newSession != null)
        {
            // 检查是否需要总结上一个对话
            if (currentChatId != null && chatHistory != null && chatHistory.Count > 0)
            {
                var oldSession = chatHistoryManager.LoadChatSession(currentChatId);
                // 只有当未制卡时才进行总结
                if (oldSession != null && !oldSession.isCardGenerated)
                {
                    StartCoroutine(SummarizeKnowledgePoints(currentChatId));
                }
            }

            // 更新当前对话ID和历史记录
            currentChatId = newChatId;
            chatHistory = newSession.messages;
        }
    }

    // 协程：总结知识点
    private IEnumerator SummarizeKnowledgePoints(string chatId)
    {
        Debug.Log("SummarizeKnowledgePoints called for chatId: " + chatId);
        // 构建对话内容
        string fullConversation = "";
        var session = chatHistoryManager.LoadChatSession(chatId);
        foreach (var message in session.messages)
        {
            fullConversation += $"{message.role}: {message.content}\n";
        }

        // 构建提示词
        string prompt = "总结这段对话中的知识点，列出要点，要求回答精简准确。";
        string summaryRequest = $"{fullConversation}\n{prompt}";

        // 发送请求
        yield return SendRequest(summaryRequest, (summaryResult) =>
        {
            // 处理总结结果
            Debug.Log("知识点总结: " + summaryResult);
            UpdateKnowledgePoints(chatId, summaryResult);
        });
    }

    // 更新知识点列表和“是否制卡”变量
    private void UpdateKnowledgePoints(string chatId, string summaryResult)
    {
        var session = chatHistoryManager.LoadChatSession(chatId);
        if (session != null)
        {
            // 更新知识点列表
            session.knowledgePoints = summaryResult;
            // 设置“是否制卡”为 true
            session.isCardGenerated = true;

            // 保存更新后的会话
            chatHistoryManager.SaveChatSession(session);
        }
    }

    // 发送请求的协程
    private IEnumerator SendRequest(string inputText, System.Action<string> callback)
    {
        // 构建请求体
        string jsonBody = JsonUtility.ToJson(new RequestData
        {
            model = "qwen-turbo",
            input = new InputData
            {
                messages = new Message[]
                {
                    new Message { role = "system", content = "You are a helpful assistant." },
                    new Message { role = "user", content = inputText }
                }
            },
            parameters = new ParametersData
            {
                result_format = "message"
            }
        });

        // 创建 UnityWebRequest 对象
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
        request.SetRequestHeader("Content-Type", "application/json");

        // 发送请求并等待响应
        yield return request.SendWebRequest();

        // 检查错误
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error: {request.error}");
            callback(request.error);
        }
        else
        {
            // 打印返回结果
            Debug.Log($"Response: {request.downloadHandler.text}");

            // 解析响应数据
            var result = JsonUtility.FromJson<ApiResponse>(request.downloadHandler.text);

            // 提取 content 字段
            if (result != null && result.output != null && result.output.choices != null && result.output.choices.Length > 0)
            {
                string responseContent = result.output.choices[0].message.content;
                callback(responseContent);
            }
            else
            {
                callback("");
            }
        }
    }

    // 定义请求数据结构
    [System.Serializable]
    private class RequestData
    {
        public string model;
        public InputData input;
        public ParametersData parameters;
    }

    [System.Serializable]
    private class InputData
    {
        public Message[] messages;
    }

    [System.Serializable]
    private class ParametersData
    {
        public string result_format;
    }

    [System.Serializable]
    private class Message
    {
        public string role;
        public string content;
    }

    // 定义 API 响应数据结构
    [System.Serializable]
    private class ApiResponse
    {
        public Output output;
    }

    [System.Serializable]
    private class Output
    {
        public Choice[] choices;
    }

    [System.Serializable]
    private class Choice
    {
        public string finish_reason;
        public Message message;
    }
}