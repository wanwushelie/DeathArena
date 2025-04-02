using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChatHistoryManager : MonoBehaviour
{
    // 聊天消息数据结构
    [System.Serializable]
    public class ChatMessage
    {
        public string role;
        public string content;
        public string review; // 新增审查字段

        public ChatMessage(string role, string content, string review = "")
        {
            this.role = role;
            this.content = content;
            this.review = review;
        }
    }

    // 聊天记录包装类
    [System.Serializable]
    public class ChatSessionWrapper
    {
        public string sessionId;
        public DateTime lastModified;
        public List<ChatMessage> messages;
    }

    public Dictionary<string, ChatSessionWrapper> chatSessions = new Dictionary<string, ChatSessionWrapper>();

    // 加载所有对话会话
    public void LoadAllChatSessions()
    {
        string directory = Path.Combine(Application.dataPath, "JSON");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            return;
        }
      
        var files = Directory.GetFiles(directory, "chat_*.json");
      
        foreach (var file in files)
        {
            // 需要跳过临时文件（Windows系统可能会有）
            if (file.EndsWith(".meta")) continue;
            string json = File.ReadAllText(file);
            ChatSessionWrapper wrapper = null; // 在外部声明变量
            // 需要处理可能的反序列化错误
            try 
            {
                wrapper = JsonUtility.FromJson<ChatSessionWrapper>(json);
            } 
            catch 
            {
                continue;
            }
            // 只有反序列化成功时才处理
            if (wrapper != null)
            {
                chatSessions[wrapper.sessionId] = wrapper;
            }
        }
    }

    // 加载指定ID的聊天记录
    public ChatSessionWrapper LoadChatSession(string chatId)
    {
        if (chatSessions.ContainsKey(chatId))
        {
            return chatSessions[chatId];
        }
        return null;
    }

    // 保存聊天记录
    public void SaveChatSession(ChatSessionWrapper session)
    {
        string json = JsonUtility.ToJson(session);
        string fileName = $"chat_{session.sessionId}.json";
        // 确保JSON目录存在
        string jsonFolder = Path.Combine(Application.dataPath, "JSON");
        if (!Directory.Exists(jsonFolder))
        {
            Directory.CreateDirectory(jsonFolder);
        }
        string filePath = Path.Combine(jsonFolder, fileName);
        Debug.Log($"保存聊天数据到: {filePath}");
        File.WriteAllText(filePath, json);
      
        // 更新缓存
        chatSessions[session.sessionId] = session;
    }

    // 保存当前活跃聊天ID
    public void SaveActiveChatId(string chatId)
    {
        string activeChatPath = Path.Combine(Application.dataPath, "JSON", "active_chat.txt");
        File.WriteAllText(activeChatPath, chatId);
    }

    // 获取当前活跃聊天ID
    public string GetActiveChatId()
    {
        string activeChatPath = Path.Combine(Application.dataPath, "JSON", "active_chat.txt");
        if (File.Exists(activeChatPath))
        {
            return File.ReadAllText(activeChatPath);
        }
        return null;
    }
}