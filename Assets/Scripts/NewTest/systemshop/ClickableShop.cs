using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ClickableShop : MonoBehaviour
{
    public Button buyButton; // 购买按钮
    public Text priceText; // 价格文本
    public GameObject shopPanel; // 商店面板
    public Button openShopButton; // 打开商店的按钮
    public Transform itemsParent; // 商品列表的父对象
    public GameObject shopItemPrefab; // 商品项的预制体
    public ShopData shopData; // 商店数据引用
    public Image knowledgeImage; // 知识图像

    private ShopItemData selectedShopItemData; // 当前选中的商品数据
    private bool isShopPanelOpen = false; // 用于记录面板是否打开，默认为关闭

    void Start()
    {
        // 默认关闭商店面板
        shopPanel.SetActive(false);

        // 为打开商店按钮添加点击事件
        openShopButton.onClick.AddListener(ToggleShop);

        // 为购买按钮添加点击事件
        buyButton.onClick.AddListener(BuySelectedShopItem);

        // 初始化商店界面
        InitializeShopUI();
    }

    // 切换商店面板的显示状态
    private void ToggleShop()
    {
        // 如果面板当前是关闭的，则打开它
        if (!isShopPanelOpen)
        {
            OpenShop();
        }
        // 如果面板当前是打开的，则关闭它
        else
        {
            CloseShop();
        }
    }

    // 打开商店面板的方法
    private void OpenShop()
    {
        shopPanel.SetActive(true); // 激活商店面板
        isShopPanelOpen = true; // 更新面板状态为打开
    }

    // 关闭商店面板的方法
    private void CloseShop()
    {
        shopPanel.SetActive(false); // 关闭商店面板
        isShopPanelOpen = false; // 更新面板状态为关闭
    }

    // 购买选中的商品
    private void BuySelectedShopItem()
    {
        if (selectedShopItemData != null)
        {
            ShoppingCartManager.Instance.BuyItem(selectedShopItemData.item);
        }
    }

    // 更新选中的商品信息
    public void UpdateSelectedShopItem(ShopItemData itemData)
    {
        selectedShopItemData = itemData;

        // 更新价格文本
        priceText.text = itemData.price.ToString();

        // 显示知识面板
        knowledgeImage.sprite = itemData.knowledgeIntroductionImage;
    }

    // 初始化商店界面
    private void InitializeShopUI()
    {
        // 清空商品列表
        foreach (Transform child in itemsParent)
        {
            Destroy(child.gameObject);
        }

        // 动态生成商品项
        foreach (ShopItemData itemData in shopData.shopItems)
        {
            GameObject itemObj = Instantiate(shopItemPrefab, itemsParent);
            ShopItemUI itemUI = itemObj.GetComponent<ShopItemUI>();
            if (itemUI != null)
            {
                itemUI.Initialize(itemData, this); // 将ClickableShop实例传递给ShopItemUI
            }
        }
    }
}