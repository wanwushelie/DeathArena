using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class TaskRequirementItem : MonoBehaviour
{
    public Image itemImage;
    public Text itemNameText;
    public Text requiredAmountText;
    public Text playerItemAmount;

    private TaskData.TaskInfo.ItemRequirement _requirement; // 任务需求

    public void Initialize(TaskData.TaskInfo.ItemRequirement requirement)
    {
        _requirement = requirement; // 初始化需求
        Debug.Log($"Initialize: Initializing requirement for {requirement.requiredItem.itemName}");
        itemImage.sprite = requirement.requiredItem.icon;
        itemNameText.text = requirement.requiredItem.itemName;
        requiredAmountText.text = $"x{requirement.amount}";

        // 显示玩家拥有的数量
        int playerCount = InventoryManager.instance.GetItemCount(requirement.requiredItem.itemName);
        playerItemAmount.text = $"{playerCount}";
        Debug.Log($"Initialize: Player has {playerCount} of {requirement.requiredItem.itemName}");

        // 根据数量设置颜色
        playerItemAmount.color = playerCount >= requirement.amount ? Color.green : Color.red;
    }

    public TaskData.TaskInfo.ItemRequirement GetRequirement()
    {
        return _requirement;
    }
}