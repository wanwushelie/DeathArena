using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CraftingListManager : MonoBehaviour
{
    public static CraftingListManager Instance { get; private set; } // 添加单例

    public ScrollRect scrollView;
    public Transform contentPanel;
    public GameObject craftingItemPrefab;
    public CraftingData craftingData;
    public GameObject craftingPanel; // 添加合成面板引用

    private bool isPanelActive = false;
    public bool isCraftingPanelOpen = false; // 添加面板状态标志

    void Start()
    {
        if (craftingPanel == null)
        {
            Debug.LogError("CraftingPanel is not assigned in the inspector!");
            enabled = false;
            return;
        }
        
        craftingPanel.SetActive(false);
        GenerateCraftingItems();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("Y key pressed"); // 添加调试日志
            ToggleCraftingPanel();
        }
    }

    public void ToggleCraftingPanel()
    {
        isCraftingPanelOpen = !isCraftingPanelOpen;
        Debug.Log($"Toggling crafting panel. New state: {isCraftingPanelOpen}");
        craftingPanel.SetActive(isCraftingPanelOpen);

        if (isCraftingPanelOpen)
        {
            // 移除时间缩放设置
            if (InGameUI.instance != null)
            {
                InGameUI.instance.ToggleInventoryUI();
            }
            else
            {
                Debug.LogWarning("InGameUI instance is null!");
            }

            // 刷新 RequirementItem 的 UI
            RefreshRequirementItems();
        }
    }

    private void RefreshRequirementItems()
    {
        // 遍历 contentPanel 下的所有 CraftingRecipeItem
        foreach (Transform child in contentPanel)
        {
            var craftingItem = child.GetComponent<CraftingRecipeItem>();
            if (craftingItem != null)
            {
                // 遍历 CraftingRecipeItem 下的所有 RequirementItem
                foreach (Transform requirementChild in craftingItem.ingredientsPanel)
                {
                    var requirementItem = requirementChild.GetComponent<RequirementItem>();
                    if (requirementItem != null)
                    {
                        // 重新初始化 RequirementItem
                        requirementItem.Initialize(requirementItem.GetRequirement());
                    }
                }
            }
        }
    }

    void GenerateCraftingItems()
    {
        // 清空之前的合成项
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // 生成新的合成项
        foreach (CraftingData.Recipe recipe in craftingData.recipes)
        {
            // 如果配方未解锁则跳过
            if (!recipe.isUnlocked) continue;

            GameObject newItem = Instantiate(craftingItemPrefab, contentPanel);
            var craftingItem = newItem.GetComponent<CraftingRecipeItem>();
            craftingItem.Initialize(recipe);
        }
    }

    // 刷新合成列表
    public void RefreshCraftingList()
    {
        GenerateCraftingItems();
    }

    /// <summary>
    /// 尝试合成物品
    /// </summary>
    public bool HasEnoughMaterials(List<CraftingData.Recipe.ItemRequirement> requirements)
    {
        foreach (var requirement in requirements)
        {
            if (!InventoryManager.instance.HasEnoughItems(requirement.requiredItem.itemName, requirement.amount))
                return false;
        }
        return true;
    }

    public bool TryCraft(CraftingData.Recipe recipe)
    {
        if (!HasEnoughMaterials(recipe.requirements))
            return false;

        foreach (var requirement in recipe.requirements)
        {
            InventoryManager.instance.RemoveItem(requirement.requiredItem.itemName, requirement.amount);
        }

        // 添加合成结果到库存
        var item = new Item { itemData = recipe.resultItem };
        InventoryManager.instance.Add(item);

        return true;
    }

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
}