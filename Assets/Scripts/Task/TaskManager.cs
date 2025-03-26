using UnityEngine;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }
    public TaskData taskData; // 任务数据引用
    private int currentTaskIndex = 0; // 当前任务索引

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (taskData == null)
        {
            Debug.LogError("TaskData is not assigned in the inspector!");
            enabled = false;
            return;
        }
    }

    // 获取当前任务
    public TaskData.TaskInfo GetCurrentTask()
    {
        if (currentTaskIndex < taskData.tasks.Count)
        {
            return taskData.tasks[currentTaskIndex];
        }
        return null;
    }

    // 检查是否可以完成当前任务
    public bool CanCompleteCurrentTask()
    {
        TaskData.TaskInfo currentTask = GetCurrentTask();
        if (currentTask == null) return false;

        foreach (var requirement in currentTask.requirements)
        {
            if (!InventoryManager.instance.HasEnoughItems(requirement.requiredItem.itemName, requirement.amount))
            {
                return false;
            }
        }
        return true;
    }

    // 完成当前任务
    public void CompleteCurrentTask()
    {
        TaskData.TaskInfo currentTask = GetCurrentTask();
        if (currentTask == null || !CanCompleteCurrentTask()) return;

        foreach (var requirement in currentTask.requirements)
        {
            InventoryManager.instance.RemoveItem(requirement.requiredItem.itemName, requirement.amount);
        }

        // 发放奖励
        Item rewardItem = new Item { itemData = currentTask.rewardItem };
        InventoryManager.instance.Add(rewardItem);

        currentTask.isCompleted = true;

        // 移动到下一个任务
        currentTaskIndex++;
    }
}