using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Image itemIcon;
    public Text itemNameText;

    private Item item;
    private ClickableShop clickableShop;

    public void Initialize(Item item, ClickableShop shop)
    {
        this.item = item;
        clickableShop = shop;

        // 设置商品图标
        if (itemIcon != null)
        {
            itemIcon.sprite = item.itemData.icon;
        }

        // 设置商品名称
        if (itemNameText != null)
        {
            itemNameText.text = item.itemData.itemName;
        }

        // 为预制体添加点击事件
        GetComponent<Button>().onClick.AddListener(() =>
        {
            clickableShop.UpdateSelectedItem(item);
        });
    }
}