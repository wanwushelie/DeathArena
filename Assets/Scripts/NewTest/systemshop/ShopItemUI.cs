using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Image itemIcon;
    public Text itemNameText;

    private ShopItemData itemData;
    private ClickableShop clickableShop;

    public void Initialize(ShopItemData data, ClickableShop shop)
    {
        itemData = data;
        clickableShop = shop;

        // 设置商品图标
        if (itemIcon != null)
        {
            itemIcon.sprite = data.item.itemData.icon;
        }

        // 设置商品名称
        if (itemNameText != null)
        {
            itemNameText.text = data.item.itemData.itemName;
        }

        // 为预制体添加点击事件
        GetComponent<Button>().onClick.AddListener(() =>
        {
            clickableShop.UpdateSelectedShopItem(itemData);
        });
    }
}