using UnityEngine;
using UnityEngine.UI; // 引入UI命名空间以便使用UI组件

public class KeywordTrigger : MonoBehaviour
{
    // 公开变量，用于在Inspector中赋值
    public InputField inputField; // UI文本输入字段
    public string keyword; // 要检测的关键词
    public Button button; // UI按钮

    void Update()
    {
        // 检查InputField是否非空
        if (inputField != null)
        {
            string inputText = inputField.text; // 从InputField获取文本
            if (!string.IsNullOrEmpty(inputText) && inputText.Contains(keyword))
            {
                // 如果检测到关键词，执行按钮点击事件
                Debug.Log("关键词被触发");
                button.onClick.Invoke(); // 调用按钮的OnClick事件
            }
        }
    }
}
