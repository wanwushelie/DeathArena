using UnityEngine;
using UnityEngine.UI;

public class TaskPanelTest : MonoBehaviour
{
    public GameObject taskPanel; //  任务面板的引用
    public Button taskButton; // 控制任务面板显示和隐藏的按钮
    public TaskUI taskUI; // 任务UI的引用

    private bool isPanelActive = false; // 任务面板是否激活

    private void Start()
    {
        if (taskPanel == null)
        {
            Debug.LogError("TaskPanel is not assigned in the inspector!");
            enabled = false;
            return;
        }

        if (taskButton == null)
        {
            Debug.LogError("TaskButton is not assigned in the inspector!");
            enabled = false;
            return;
        }

        if (taskUI == null)
        {
            Debug.LogError("TaskUI is not assigned in the inspector!");
            enabled = false;
            return;
        }

        taskPanel.SetActive(false); // 初始时隐藏任务面板
        taskButton.onClick.AddListener(ToggleTaskPanel); // 添加按钮点击事件
    }

    // 切换任务面板的显示状态
    public void ToggleTaskPanel()
    {
        isPanelActive = !isPanelActive;
        taskPanel.SetActive(isPanelActive);

        if (isPanelActive)
        {
            // 当任务面板显示时，更新任务信息
            taskUI.UpdateTaskUI();
        }
    }
}