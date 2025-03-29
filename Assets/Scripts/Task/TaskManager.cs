using UnityEngine;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

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
        if (currentTask == null)
        {
            Debug.Log("CanCompleteCurrentTask: No current task available.");
            return false;
        }

        foreach (var requirement in currentTask.requirements)
        {
            if (!InventoryManager.instance.HasEnoughItems(requirement.requiredItem.itemName, requirement.amount))
            {
                Debug.Log($"CanCompleteCurrentTask: Insufficient items for {requirement.requiredItem.itemName}. Required: {requirement.amount}, Available: {InventoryManager.instance.GetItemCount(requirement.requiredItem.itemName)}");
                return false;
            }
        }
        Debug.Log("CanCompleteCurrentTask: All requirements met.");
        return true;
    }

    // 完成当前任务
    public bool CompleteCurrentTask()
    {
        TaskData.TaskInfo currentTask = GetCurrentTask();
        if (currentTask == null || !CanCompleteCurrentTask()) 
            return false;
    
        // 扣除需求物品
        foreach (var requirement in currentTask.requirements)
        {
            InventoryManager.instance.RemoveItem(
                requirement.requiredItem.itemName, 
                requirement.amount);
        }
    
        // 发放奖励物品（类似合成系统）
        var item = new Item { itemData = currentTask.rewardItem };
        InventoryManager.instance.Add(item);
    
        // 更新任务状态
        currentTask.isCompleted = true;
        currentTaskIndex++;
    
        return true;
    }
}