using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class TaskUI : MonoBehaviour
{
    public Text taskNameText;
    public Text taskDescriptionText;
    public Transform requirementsPanel;
    public GameObject requirementItemPrefab; // 确保这是 TaskRequirementItem 的预制体
    public Image rewardImage;
    public Text rewardText;
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
            Debug.Log("UpdateTaskUI: No more tasks available.");
            taskNameText.text = "No more tasks!";
            taskDescriptionText.text = "";
            requirementsPanel.gameObject.SetActive(false);
            completeButton.gameObject.SetActive(false);
            rewardImage.gameObject.SetActive(false);
            rewardText.gameObject.SetActive(false);
            return;
        }

        Debug.Log($"UpdateTaskUI: Updating UI for task {currentTask.taskName}");
        taskNameText.text = currentTask.taskName;
        taskDescriptionText.text = currentTask.taskDescription;

        // 清空之前的任务需求
        foreach (Transform child in requirementsPanel)
        {
            Debug.Log($"UpdateTaskUI: Clearing previous requirement item {child.gameObject.name}");
            Destroy(child.gameObject);
        }

        // 添加当前任务的需求
        foreach (var requirement in currentTask.requirements)
        {
            GameObject requirementItem = Instantiate(requirementItemPrefab, requirementsPanel);
            requirementItem.GetComponent<TaskRequirementItem>().Initialize(requirement);
            Debug.Log($"UpdateTaskUI: Added requirement item {requirement.requiredItem.itemName}");
        }

        rewardImage.sprite = currentTask.rewardItem.icon;
        rewardText.text = "数量：*" + currentTask.rewardAmount.ToString();

        // completeButton.interactable = TaskManager.Instance.CanCompleteCurrentTask();
        completeButton.onClick.AddListener(OnCompleteTaskClicked);
        Debug.Log($"UpdateTaskUI: Complete button state set to {completeButton.interactable}");
    }

    public void OnCompleteTaskClicked()
    {
        if (!TaskManager.Instance.CanCompleteCurrentTask()) return;
        
        // 播放音效
        SoundManager.Instance.Play("EFFECT/Click", SoundType.EFFECT);
        
        // 执行任务完成逻辑
        if (TaskManager.Instance.CompleteCurrentTask())
        {
            // 更新UI
            UpdateTaskUI();
            // 可以添加奖励特效等
        }
    }
}