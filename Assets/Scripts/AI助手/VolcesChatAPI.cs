using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class VolcesChatAPI : MonoBehaviour
{
    private const string apiUrl = "https://ark.cn-beijing.volces.com/api/v3/chat/completions";
    private const string authorizationToken = "Bearer <TVdKallqWmtaVFUyWWpBeE5Ea3hORGszWldRNVlUSTBZV0k1WXpsak0yVQ==>";
    private const string modelName = "doubao-1.5-pro-32k-250115";

    private void Start()
    {
        StartCoroutine(SendChatRequest());
    }

    private IEnumerator SendChatRequest()
    {
        // 构建请求体
        ChatRequest requestBody = new ChatRequest
        {
            model = modelName,
            messages = new Message[]
            {
                new Message { role = "system", content = "You are a helpful assistant." },
                new Message { role = "user", content = "Hello!" }
            }
        };
        string jsonRequestBody = JsonUtility.ToJson(requestBody);

        // 创建请求
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            // 设置请求头
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", authorizationToken);

            // 设置请求体
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequestBody);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            // 发送请求并等待响应
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("API Response: " + responseText);
                // 这里可以根据API的响应格式进一步解析数据
            }
            else
            {
                Debug.LogError("Request Failed: " + request.error);
            }
        }
    }

    [System.Serializable]
    private class ChatRequest
    {
        public string model;
        public Message[] messages;
    }

    [System.Serializable]
    private class Message
    {
        public string role;
        public string content;
    }
}