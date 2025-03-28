using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

/// <summary>
/// 该类用于管理UI中的物品槽。
/// </summary>
public class Slot_UI : MonoBehaviour
{
    public int slotID; // 物品槽的ID
    public Inventory inventory; // 关联的物品库存
    public Image itemIcon; // 物品图标显示组件
    public Text quantityText; // 物品数量显示文本组件

    [SerializeField] private GameObject highlight; // 物品槽高亮显示的游戏对象

    /// <summary>
    /// 设置物品槽显示的物品信息。
    /// </summary>
    /// <param name="slot">要显示的物品槽信息。</param>
    public void SetItem(Inventory.Slot slot)
    {
        if (slot != null) // 检查物品槽是否为空
        {
            // 检查物品箱是否打开且物品不可售卖
            if (GameManager.instance.itemBox != null && GameManager.instance.itemBox.isBoxOpen && !slot.isSellable)
            {
                itemIcon.sprite = slot.icon; // 设置物品图标
                itemIcon.color = new Color(0.4f, 0.4f, 0.4f, 0.7f); // 设置图标颜色为半透明灰色
            }
            else
            {
                itemIcon.sprite = slot.icon; // 设置物品图标
                itemIcon.color = new Color(1, 1, 1, 1); // 设置图标颜色为不透明白色

                if (slot.currentCount == 0) // 检查物品数量是否为0
                {
                    slot.RemoveItem(); // 移除物品
                    EmptyItem(); // 清空物品槽显示
                }
                else
                {
                    // 如果物品数量为1则不显示数量，否则显示数量
                    quantityText.text = slot.currentCount == 1 ? "" : slot.currentCount.ToString();
                }
            }
        }
    }

    /// <summary>
    /// 清空物品槽的显示信息。
    /// </summary>
    public void EmptyItem()
    {
        itemIcon.sprite = null; // 清空物品图标
        itemIcon.color = new Color(1, 1, 1, 0); // 设置图标颜色为透明
        quantityText.text = ""; // 清空物品数量文本
    }

    /// <summary>
    /// 设置物品槽的高亮显示状态。
    /// </summary>
    /// <param name="isOn">是否开启高亮显示。</param>
    public void SetHighlight(bool isOn)
    {
        highlight.SetActive(isOn); // 设置高亮显示游戏对象的激活状态
    }
}
