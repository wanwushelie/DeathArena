using System;
using System.Collections.Generic;
using UnityEngine;

/* 游戏内各种库存的管理
    背包、工具栏 */

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public static event Action OnInventoryChanged;

    public Dictionary<string, Inventory> inventoryByName = new Dictionary<string, Inventory>();

    [Header("BackPack")]
    public Inventory backpack;
    public int backpackSlotCount;

    [Header("Toolbar")]
    public Inventory toolbar;
    public int toolbarSlotCount;
    public List<Item> startItems;

    void Awake()
    {
        if (!instance)
            instance = this;

        backpack = new Inventory(backpackSlotCount);
        toolbar = new Inventory(toolbarSlotCount);

        inventoryByName.Add("Backpack", backpack);
        inventoryByName.Add("Toolbar", toolbar);

    }

    public void AddStartItem()
    {
        foreach (var item in startItems)
        {
            toolbar.Add(item);
        }
    }

    public void Add(Item item)
    {
        if(toolbar.Add(item))
        {
            return;
        }
        
        if(backpack.Add(item))
        {
            return;
        }
    }

    public void AddInventory(string inventoryName, Item item)
    {
        if (inventoryByName.ContainsKey(inventoryName))
        {
            inventoryByName[inventoryName].Add(item);
        }
    }

    public Inventory GetInventoryByName(string inventoryName)
    {
        if (inventoryByName.ContainsKey(inventoryName))
        {
            return inventoryByName[inventoryName];
        }
        return null;
    }

    public string GetInventoryName(Inventory inventory)
    {
        foreach (var entry in inventoryByName)
        {
            if (entry.Value == inventory)
            {
                return entry.Key;
            }
        }
        return null;
    }

    public void ClearInventory()
    {
        foreach (var inventory in inventoryByName.Values)
        {
            inventory.Clear();
        }
    }

    /// <summary>
    /// 检查是否有足够材料
    /// </summary>
    public bool HasEnoughMaterials(List<CraftingData.Recipe.ItemRequirement> requirements)
    {
        foreach (var requirement in requirements)
        {
            if (!HasEnoughItems(requirement.requiredItem.itemName, requirement.amount))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 扣除合成材料
    /// </summary>
    public bool RemoveMaterials(List<CraftingData.Recipe.ItemRequirement> requirements)
    {
        if (!HasEnoughMaterials(requirements)) return false;

        foreach (var requirement in requirements)
        {
            RemoveItem(requirement.requiredItem.itemName, requirement.amount);
        }
        return true;
    }

    // 在Add/Remove方法最后调用
    private void NotifyInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }

    public int GetItemCount(string itemName)
    {
        int count = 0;
        count += backpack.GetItemCount(itemName);
        count += toolbar.GetItemCount(itemName);
        return count;
    }

    public bool HasEnoughItems(string itemName, int requiredAmount)
    {
        return GetItemCount(itemName) >= requiredAmount;
    }

    public bool RemoveItem(string itemName, int amount = 1)
    {
        if (toolbar.RemoveItem(itemName, amount))
            return true;
        return backpack.RemoveItem(itemName, amount);
    }
}
