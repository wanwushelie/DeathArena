using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Crafting Data", menuName = "Crafting/Crafting Data")]
public class CraftingData : ScriptableObject
{
    [System.Serializable]
    public class Recipe
    {
        public string recipeID; // 配方唯一标识
        public ItemData resultItem; // 合成结果物品数据
        public int resultAmount = 1; // 合成结果数量
        public List<ItemRequirement> requirements; // 所需材料
        public bool isUnlocked = true; // 是否已解锁

        [System.Serializable]
        public class ItemRequirement
        {
            public ItemData requiredItem; // 所需物品数据
            public int amount; // 所需数量
        }
    }

    public List<Recipe> recipes = new List<Recipe>(); // 配方列表
}