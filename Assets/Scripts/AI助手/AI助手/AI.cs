using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

    // HttpRequestExample 脚本的引用
    private HttpRequestExample m_HttpRequestExample;

    void Awake()
    {
        // 获取 HttpRequestExample 脚本的引用
        m_HttpRequestExample = GetComponent<HttpRequestExample>();
    }

    // 发送信息
    public void SendData()
    {
        if (string.IsNullOrEmpty(m_InputWord.text))
            return;

        string _msg = m_InputWord.text;
        ChatPrefab _chat = Instantiate(m_PostChatPrefab, m_rootTrans.transform);
        _chat.SetText(_msg);
        // 重新计算容器尺寸
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
        StartCoroutine(TurnToLastLine());
        // 发送请求并处理响应
        m_HttpRequestExample.SendRequest(_msg, CallBack);
        m_InputWord.text = "";
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
    }

    // 滚动到最新消息
    private IEnumerator TurnToLastLine()
    {
        yield return new WaitForEndOfFrame();
        // 滚动到最近的消息
        m_ScroTectObject.verticalNormalizedPosition = 0;
    }
}

