using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events; // 添加这行

public class ChatUIManager : MonoBehaviour
{
    [SerializeField] public RectTransform m_rootTrans; // 聊天文本放置的层
    [SerializeField] private ScrollRect m_ScroTectObject; // 滚动条
    [SerializeField] public Button m_SendButton; // 发送按钮
    [SerializeField] private ChatPrefab m_PostChatPrefab; // 发送聊天气泡
    [SerializeField] private ChatPrefab m_RobotChatPrefab; // 回复的聊天气泡
    [SerializeField] private GameObject m_ChatRecordsPanel; // 聊天记录按钮的父物体
    [SerializeField] private GameObject m_ChatRecordButtonPrefab; // 聊天记录按钮的预制体

    public bool isProcessing = false; // 表示是否正在处理请求

    // 更新按钮状态
    public void UpdateButtonState(bool isProcessing)
    {
        this.isProcessing = isProcessing;
        if (m_SendButton != null)
        {
            Text buttonText = m_SendButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = isProcessing ? "停止" : "发送";
            }
        }
    }

    // 显示用户消息
    public void ShowUserMessage(string message)
    {
        ChatPrefab chat = Instantiate(m_PostChatPrefab, m_rootTrans.transform);
        chat.SetText(message);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
        StartCoroutine(TurnToLastLine());
    }

    // 显示AI回复
    public void ShowAIReply(string reply, string review)
    {
        ChatPrefab chat = Instantiate(m_RobotChatPrefab, m_rootTrans.transform);
        chat.SetText(reply);
        chat.m_ReviewBubble.SetActive(false); // 初始化时隐藏审查气泡
        chat.SetReviewText(review);
        chat.SetupButton();
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
        StartCoroutine(TurnToLastLine());
    }

    // 清除当前聊天显示
    public void ClearChatDisplay()
    {
        foreach (Transform child in m_rootTrans.transform)
        {
            Destroy(child.gameObject);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
    }

    // 加载聊天记录按钮
    public void LoadChatRecordsButtons(Dictionary<string, ChatHistoryManager.ChatSessionWrapper> chatSessions, 
        UnityAction<string> onClick)
    {
        // 清除现有的按钮
        foreach (Transform child in m_ChatRecordsPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // 为每个聊天记录创建按钮
        foreach (var session in chatSessions.Values)
        {
            GameObject buttonObj = Instantiate(m_ChatRecordButtonPrefab, m_ChatRecordsPanel.transform);
            Button uiButton = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            
            // 直接绑定点击事件
            uiButton.onClick.AddListener(() => onClick(session.sessionId));
            
            // 设置按钮文本
            buttonText.text = session.messages.Count > 0 
                ? (session.messages[0].content.Length > 20 
                    ? session.messages[0].content.Substring(0, 20) + "..." 
                    : session.messages[0].content)
                : "新对话";
        }
    }

    // 聊天记录按钮点击事件
    public void OnChatRecordButtonClicked(string chatId)
    {
        // 清除当前聊天显示
        ClearChatDisplay();

        // 加载选中的聊天记录
        // 这里需要 AI.cs 调用并传递 chatHistoryManager 和 chatId
        // 然后根据 chatId 加载对应的聊天记录并显示
    }

    // 滚动到最新消息
    private IEnumerator TurnToLastLine()
    {
        yield return new WaitForEndOfFrame();
        // 滚动到最近的消息
        m_ScroTectObject.verticalNormalizedPosition = 0;
    }

    // 添加新变量
    [SerializeField] private GameObject m_KnowledgeCardPrefab;
    [SerializeField] private Transform m_KnowledgeCardsPanel;
    
    // 添加新方法
    public void LoadKnowledgeCards(Dictionary<string, ChatHistoryManager.ChatSessionWrapper> chatSessions)
    {
        // 清除现有卡片
        foreach (Transform child in m_KnowledgeCardsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    
        // 为每个有知识点的对话创建卡片
        foreach (var session in chatSessions.Values)
        {
            if (!string.IsNullOrEmpty(session.knowledgePoints) && session.isCardGenerated)
            {
                GameObject cardObj = Instantiate(m_KnowledgeCardPrefab, m_KnowledgeCardsPanel);
                Text cardText = cardObj.GetComponentInChildren<Text>();
                cardText.text = session.knowledgePoints;
            }
        }
    }
}