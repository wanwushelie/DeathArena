using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    public string recipeID; // 配方唯一标识
    public Item resultItem; // 合成结果物品
    public int resultAmount = 1; // 合成结果数量
    public List<ItemRequirement> requirements; // 所需材料
    public bool isUnlocked = true; // 是否已解锁
}

[System.Serializable]
public class ItemRequirement
{
    public Item requiredItem; // 所需物品
    public int amount; // 所需数量
}

public class RecipeSystem : MonoBehaviour
{
    public static RecipeSystem Instance { get; private set; }

    [SerializeField]
    private List<Recipe> allRecipes = new List<Recipe>(); // 所有配方

    private Dictionary<string, Recipe> recipeDictionary = new Dictionary<string, Recipe>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeRecipeSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 初始化配方系统
    private void InitializeRecipeSystem()
    {
        foreach (var recipe in allRecipes)
        {
            if (!recipeDictionary.ContainsKey(recipe.recipeID))
            {
                recipeDictionary.Add(recipe.recipeID, recipe);
            }
        }
    }

    // 获取所有可用配方
    public List<Recipe> GetAvailableRecipes()
    {
        List<Recipe> availableRecipes = new List<Recipe>();
        foreach (var recipe in allRecipes)
        {
            if (recipe.isUnlocked)
            {
                availableRecipes.Add(recipe);
            }
        }
        return availableRecipes;
    }

    // 根据ID获取配方
    public Recipe GetRecipeByID(string recipeID)
    {
        if (recipeDictionary.TryGetValue(recipeID, out Recipe recipe))
        {
            return recipe;
        }
        return null;
    }

    // 解锁配方
    public void UnlockRecipe(string recipeID)
    {
        if (recipeDictionary.TryGetValue(recipeID, out Recipe recipe))
        {
            recipe.isUnlocked = true;
        }
    }

    // 获取合成结果图标
    public Sprite GetResultIcon(string recipeID)
    {
        if (recipeDictionary.TryGetValue(recipeID, out Recipe recipe))
        {
            return recipe.resultItem.itemData.icon;
        }
        return null;
    }

    // 获取所需材料图标
    public List<Sprite> GetRequirementIcons(string recipeID)
    {
        List<Sprite> icons = new List<Sprite>();
        if (recipeDictionary.TryGetValue(recipeID, out Recipe recipe))
        {
            foreach (var req in recipe.requirements)
            {
                icons.Add(req.requiredItem.itemData.icon);
            }
        }
        return icons;
    }
}