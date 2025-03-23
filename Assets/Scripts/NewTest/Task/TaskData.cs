using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Task Data", menuName = "Task/Task Data")]
public class TaskData : ScriptableObject
{
    [System.Serializable]
    public class TaskInfo
    {    
        public string taskName; // 任务名称
        public string taskDescription; // 任务描述
        public List<ItemRequirement> requirements; // 完成任务所需的物品
        public ItemData rewardItem; // 完成任务后获得的奖励物品
        public int rewardAmount; // 奖励物品数量
        public bool isCompleted; // 任务是否已完成

        [System.Serializable]
        public class ItemRequirement
        {
            public ItemData requiredItem; // 所需物品数据
            public int amount; // 所需数量
        }
    }

    public List<TaskInfo> tasks = new List<TaskInfo>(); // 配方列表
}
