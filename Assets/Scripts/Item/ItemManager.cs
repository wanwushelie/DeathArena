using System.Collections.Generic;
using UnityEngine;

// 物品管理
//ItemManager.cs 是一个用于管理游戏内所有物品的类，主要负责将物品数据（Item 对象）进行集中管理，并提供快速检索功能。

public class ItemManager : MonoBehaviour
{
    public Item[] items; // 可收集的物品数组

    // 用于将物品名称映射到物品实例的字典
    private Dictionary<string, Item> nameToItemDict = new Dictionary<string, Item>();

    private void Awake()
    {
        foreach (Item item in items)
        {
            AddItem(item);
        }
    }

    public void AddItem(Item item)
    {
        if (!nameToItemDict.ContainsKey(item.itemData.itemName))
        {
            nameToItemDict.Add(item.itemData.itemName, item);
        }
    }

    // 通过物品名称检索对应的物品实例，如果字典中存在该名称的物品，返回对应的 Item 对象。
    public Item GetItemByName(string key)
    {
        if (nameToItemDict.ContainsKey(key))
        {
            return nameToItemDict[key]; // 返回对应名称的物品
        }

        return null;
    }

    public FoodData GetFoodData(string key)
    {
        if (nameToItemDict.ContainsKey(key))
        {
            Item item = nameToItemDict[key];
            if (item.GetComponent<FoodData>() != null)
            {
                return item.GetComponent<FoodData>();
            }
        }
        return null;
    }
}
