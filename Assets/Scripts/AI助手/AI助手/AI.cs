using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class AI : MonoBehaviour
{
    // 输入的信息
    [SerializeField] private InputField m_InputWord;
    // 聊天文本放置的层
    [SerializeField] private RectTransform m_rootTrans;
    // 发送聊天气泡
    [SerializeField] private ChatPrefab m_PostChatPrefab;
    // 滚动条
    [SerializeField] private ScrollRect m_ScroTectObject;
    // 回复的聊天气泡
    [SerializeField] private ChatPrefab m_RobotChatPrefab;
    // 发送按钮
    [SerializeField] private Button m_SendButton;

    // HttpRequestExample 脚本的引用
    private HttpRequestExample m_HttpRequestExample;
    private bool isProcessing = false; // 表示是否正在处理请求
    private Coroutine currentCoroutine; // 存储当前正在运行的协程

    void Awake()
    {
        // 获取 HttpRequestExample 脚本的引用
        m_HttpRequestExample = GetComponent<HttpRequestExample>();
        // 初始化按钮状态
        UpdateButtonState();
        m_SendButton.onClick.AddListener(SendData);
    }

    // 发送信息
    public void SendData()
    {
        if (isProcessing)
        {
            // 如果正在处理，取消请求
            StopCoroutine(currentCoroutine);
            isProcessing = false;
            UpdateButtonState();
            return;
        }

        if (string.IsNullOrEmpty(m_InputWord.text))
            return;

        string _msg = m_InputWord.text;
        ChatPrefab _chat = Instantiate(m_PostChatPrefab, m_rootTrans.transform);
        _chat.SetText(_msg);
        // 重新计算容器尺寸
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
        StartCoroutine(TurnToLastLine());

        // 开始请求
        isProcessing = true;
        UpdateButtonState();
        currentCoroutine = m_HttpRequestExample.SendRequest(_msg, CallBack);
        m_InputWord.text = "";
        // StartCoroutine(currentCoroutine);
    }

    // 回调函数，处理API响应
    private void CallBack(string _callback)
    {
        if (!string.IsNullOrEmpty(_callback))
        {
            ChatPrefab _chat = Instantiate(m_RobotChatPrefab, m_rootTrans.transform);
            _chat.SetText(_callback);
            // 重新计算容器尺寸
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
            StartCoroutine(TurnToLastLine());
        }

        // 请求完成，更新按钮状态
        isProcessing = false;
        UpdateButtonState();
    }

    // 滚动到最新消息
    private IEnumerator TurnToLastLine()
    {
        yield return new WaitForEndOfFrame();
        // 滚动到最近的消息
        m_ScroTectObject.verticalNormalizedPosition = 0;
    }

    // 更新按钮状态
    private void UpdateButtonState()
    {
        if (m_SendButton != null)
        {
            Text buttonText = m_SendButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = isProcessing ? "停止" : "发送";
            }
        }
    }
}