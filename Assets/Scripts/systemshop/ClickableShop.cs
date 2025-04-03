using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ClickableShop : MonoBehaviour
{
    public GameObject shopPanel; // 商店面板
    public Button openShopButton; // 打开商店的按钮
    public Transform itemsParent; // 商品列表的父对象
    public GameObject shopItemPrefab; // 商品项的预制体
    public Transform categoryButtonsParent; // 分类按钮的父对象
    public GameObject categoryButtonPrefab; // 分类按钮的预制体
    public ItemManager itemManager; // 新增：物品管理器引用

    private Item selectedItem; // 当前选中的物品
    private bool isShopPanelOpen = false; // 用于记录面板是否打开，默认为关闭
    private string currentCategory = "全部"; //  当前选中的分类，默认为“全部”

    //左侧面板
    [Header("左侧面板")]
    //TMP文本
    public TMPro.TMP_Text name; // 名称
    public TMPro.TMP_Text categary; // 分类
    public TMPro.TMP_Text introduce; // 介绍
    public TMPro.TMP_Text story; // 典故
    public Image knowledgeImage; // 知识图像
    public Button buyButton; // 购买按钮
    public Text priceText; // 价格文本

    void Start()
    {
        // 默认关闭商店面板
        shopPanel.SetActive(false);

        // 为打开商店按钮添加点击事件
        openShopButton.onClick.AddListener(ToggleShop);

        // 为购买按钮添加点击事件
        buyButton.onClick.AddListener(BuySelectedItem);

        // 初始化商店界面
        InitializeShopUI();

        // 初始化分类按钮
        InitializeCategoryButtons();
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
        UpdateShopItemsDisplay(); // 更新商品显示
    }

    // 关闭商店面板的方法
    private void CloseShop()
    {
        shopPanel.SetActive(false); // 关闭商店面板
        isShopPanelOpen = false; // 更新面板状态为关闭
    }

    // 购买选中的物品
    private void BuySelectedItem()
    {
        if (selectedItem != null)
        {
            ShoppingCartManager.Instance.BuyItem(selectedItem);
        }
    }

    // 更新选中的物品信息
    public void UpdateSelectedItem(Item item)
    {
        selectedItem = item;

        // 更新价格文本
        priceText.text = item.itemData.price.ToString();

        // 显示知识面板
        // knowledgeImage.sprite = item.shopItemData.knowledgeIntroductionImage;
        knowledgeImage.sprite = item.itemData.icon;
        name.text = item.itemData.itemName;
        categary.text = item.shopItemData.category;
        introduce.text = item.shopItemData.itemIntroduce;
        story.text = item.shopItemData.itemStory;
    }

    // 初始化商店界面
    private void InitializeShopUI()
    {
        // 清空商品列表
        foreach (Transform child in itemsParent)
        {
            Destroy(child.gameObject);
        }
        Debug.Log($"Total items: {itemManager.items.Length}");  // Changed from .Count to .Length
        if (itemManager == null || itemManager.items == null) return; // 防止空引用
        // 动态生成商品项
        foreach (Item item in itemManager.items)
        {
            Debug.Log($"Item: {item.itemData?.itemName}, ShopData: {item.shopItemData != null}, Unlocked: {item.shopItemData?.isUnlocked}, Category: {item.shopItemData?.category}");
            if (item.shopItemData != null && item.shopItemData.isUnlocked && 
                (currentCategory == "全部" || item.shopItemData.category == currentCategory))
            {
                GameObject itemObj = Instantiate(shopItemPrefab, itemsParent);
                ShopItemUI itemUI = itemObj.GetComponent<ShopItemUI>();
                if (itemUI != null)
                {
                    itemUI.Initialize(item, this); // 将ClickableShop实例传递给ShopItemUI
                }
            }
        }
    }

    // 初始化分类按钮
    private void InitializeCategoryButtons()
    {
        // 清空分类按钮列表
        foreach (Transform child in categoryButtonsParent)
        {
            Destroy(child.gameObject);
        }

        // 添加“全部”分类按钮
        GameObject allCategoryButton = Instantiate(categoryButtonPrefab, categoryButtonsParent);
        Button allButton = allCategoryButton.GetComponent<Button>();
        if (allButton != null)
        {
            allButton.onClick.AddListener(() => SelectCategory("全部"));
            allCategoryButton.GetComponentInChildren<Text>().text = "全部";
        }

        // 根据商品数据动态生成分类按钮
        List<string> categories = new List<string>();
        foreach (Item item in itemManager.items)
        {
            if (item.shopItemData != null && !categories.Contains(item.shopItemData.category))
            {
                categories.Add(item.shopItemData.category);

                GameObject categoryButton = Instantiate(categoryButtonPrefab, categoryButtonsParent);
                Button categoryButtonComponent = categoryButton.GetComponent<Button>();
                if (categoryButtonComponent != null)
                {
                    categoryButtonComponent.onClick.AddListener(() => SelectCategory(item.shopItemData.category));
                    categoryButton.GetComponentInChildren<Text>().text = item.shopItemData.category;
                }
            }
        }
    }

    // 选择分类并更新商品显示
    private void SelectCategory(string category)
    {
        currentCategory = category;
        UpdateShopItemsDisplay();
    }

    // 更新商品显示
    private void UpdateShopItemsDisplay()
    {
        // 清空商品列表
        foreach (Transform child in itemsParent)
        {
            Destroy(child.gameObject);
        }

        // 根据当前选中的分类动态生成商品项
        foreach (Item item in itemManager.items)
        {
            if (item.shopItemData != null && item.shopItemData.isUnlocked &&
                (currentCategory == "全部" || item.shopItemData.category == currentCategory))
            {
                GameObject itemObj = Instantiate(shopItemPrefab, itemsParent);
                ShopItemUI itemUI = itemObj.GetComponent<ShopItemUI>();
                if (itemUI != null)
                {
                    itemUI.Initialize(item, this); // 将ClickableShop实例传递给ShopItemUI
                }
            }
        }
    }
}