using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class RequirementItem : MonoBehaviour
{
    public Image itemImage;
    public Text itemNameText;
    public Text requiredAmountText;
    //玩家拥有的原材料数量
    public Text playerItemAmount;

    private CraftingData.Recipe.ItemRequirement _requirement; // 添加私有字段

    public void Initialize(CraftingData.Recipe.ItemRequirement requirement)
    {
        _requirement = requirement; // 初始化需求
        itemImage.sprite = requirement.requiredItem.icon;
        itemNameText.text = requirement.requiredItem.itemName;
        requiredAmountText.text = $"x{requirement.amount}";
        
        // 显示玩家拥有的数量
        int playerCount = InventoryManager.instance.GetItemCount(requirement.requiredItem.itemName);
        Debug.Log("玩家拥有的数量：" + playerCount);
        playerItemAmount.text = $"{playerCount}";
        
        // 根据数量设置颜色
        playerItemAmount.color = playerCount >= requirement.amount ? Color.green : Color.red;
    }

    public CraftingData.Recipe.ItemRequirement GetRequirement()
    {
        return _requirement;
    }
}