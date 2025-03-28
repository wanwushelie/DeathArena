using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class HttpRequestExample : MonoBehaviour
{
    // API 基本信息
    private string apiUrl = "https://open.bigmodel.cn/api/paas/v4/chat/completions";   // 替换为实际的 API 端点
    private string apiKey = "a1f4615270a8e363ec8ab7eb735f108b.2p9I0mFwPXzsuT6E";  // 替换为你的 API 密钥

    // 公共方法，用于发送请求
    public void SendRequest(string message, System.Action<string> callback)
    {
        StartCoroutine(SendRequestCoroutine(message, callback));
    }

    private IEnumerator SendRequestCoroutine(string message, System.Action<string> callback)
    {
        // 请求的头部信息
        var headers = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {apiKey}" },
            { "Content-Type", "application/json" }
        };

        // 请求数据
        string jsonData = "{\"model\": \"glm-4-flash\", \"messages\": [{\"role\": \"user\", \"content\": \"" + message + "\"}]}";

        using (var request = new UnityWebRequest(apiUrl, "POST"))
        {
            // 转换 JSON 数据为字节数组
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            // 发送请求并等待响应
            yield return request.SendWebRequest();

            // 处理请求结果
            if (request.result == UnityWebRequest.Result.Success)
            {
                // 解析响应数据
                var result = JsonUtility.FromJson<ApiResponse>(request.downloadHandler.text);
                // 保存需要的内容
                string responseContent = result.choices[0].message.content;
                // 调用回调函数
                callback(responseContent); // 调用回调，传递响应内容
            }
            else
            {
                // 请求失败时的处理
                callback(request.error); // 调用回调，传递错误信息
            }
        }
    }

    // 用于解析 API 响应的类定义
    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }

    [System.Serializable]
    public class ApiResponse
    {
        public Choice[] choices;
    }
}