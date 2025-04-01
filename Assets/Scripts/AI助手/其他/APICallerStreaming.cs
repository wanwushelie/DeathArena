using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class APICaller : MonoBehaviour
{
    // 请替换为你的API Key
    private string apiKey = "sk-9bc69ef7b0e444838ffddff1d1323371";
    private string url = "https://dashscope.aliyuncs.com/api/v1/services/aigc/text-generation/generation";

    // void Start()
    // {
    //     StartCoroutine(MakeRequest());
    // }

    // 添加一个公共方法用于发起请求
    public void MakeRequest(string inputText)
    {
        StartCoroutine(SendRequest(inputText));
    }

    // IEnumerator MakeRequest()
    IEnumerator SendRequest(string inputText)
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
                    // new Message { role = "user", content = "你好，哪个公园距离我最近？" }
                    new Message { role = "user", content = inputText } // 使用传入的文本
                }
            },
            parameters = new ParametersData
            {
                result_format = "message"
            }
        });

        // 创建UnityWebRequest对象
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
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
        }
        else
        {
            // 打印返回结果
            Debug.Log($"Response: {request.downloadHandler.text}");
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
}