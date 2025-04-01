//加载聊天记录按钮：根据已有的聊天记录会话，生成并显示相应的按钮。
//处理按钮点击事件：当用户点击某个聊天记录按钮时，加载并显示对应的聊天记录。
using UnityEngine;
using UnityEngine.Events;

public class ChatRecordButton : MonoBehaviour
{
    public string ChatId;
    public UnityAction<string> OnButtonClick;

    public void Setup(string chatId, UnityAction<string> onClick)
    {
        ChatId = chatId;
        OnButtonClick = onClick;
    }

    public void OnClick()
    {
        if (OnButtonClick != null)
        {
            OnButtonClick(ChatId);
        }
    }
}