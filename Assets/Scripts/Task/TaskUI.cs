using UnityEngine;
using UnityEngine.UI;

public class TaskUI : MonoBehaviour
{
    public Text taskNameText;
    public Text taskDescriptionText;
    public Transform requirementsPanel;
    public GameObject requirementItemPrefab; // 确保这是 TaskRequirementItem 的预制体
    public Button completeButton;

    private void Start()
    {
        UpdateTaskUI();
    }

    public void UpdateTaskUI()
    {
        TaskData.TaskInfo currentTask = TaskManager.Instance.GetCurrentTask();
        if (currentTask == null)
        {
            taskNameText.text = "No more tasks!";
            taskDescriptionText.text = "";
            requirementsPanel.gameObject.SetActive(false);
            completeButton.gameObject.SetActive(false);
            return;
        }

        taskNameText.text = currentTask.taskName;
        taskDescriptionText.text = currentTask.taskDescription;

        // 清空之前的任务需求
        foreach (Transform child in requirementsPanel)
        {
            Destroy(child.gameObject);
        }

        // 添加当前任务的需求
        foreach (var requirement in currentTask.requirements)
        {
            GameObject requirementItem = Instantiate(requirementItemPrefab, requirementsPanel);
            requirementItem.GetComponent<TaskRequirementItem>().Initialize(requirement);
        }

        completeButton.interactable = TaskManager.Instance.CanCompleteCurrentTask();
    }

    public void OnCompleteTaskClicked()
    {
        TaskManager.Instance.CompleteCurrentTask();
        UpdateTaskUI();
    }
}