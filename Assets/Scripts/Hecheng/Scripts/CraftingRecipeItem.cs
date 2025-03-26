using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeItem : MonoBehaviour
{
    public Image craftingIcon;//背景图片
    public Image resultItemImage;
    public Text resultItemNameText;
    public Text resultAmountText;
    public Transform ingredientsPanel;//合成区
    public GameObject ingredientItemPrefab;//合成原料
    public Button craftButton;

    private CraftingData.Recipe currentRecipe; // 添加当前配方字段

    public void Initialize(CraftingData.Recipe recipe)
    {
        // 设置合成结果信息
        resultItemImage.sprite = recipe.resultItem.icon;
        resultItemNameText.text = recipe.resultItem.itemName;
        resultAmountText.text = $"x{recipe.resultAmount}";

        // 清空原料面板
        foreach (Transform child in ingredientsPanel)
        {
            Destroy(child.gameObject);
        }

        // 添加原料信息
        foreach (var requirement in recipe.requirements)
        {
            var ingredientItem = Instantiate(ingredientItemPrefab, ingredientsPanel);
            var requirementItem = ingredientItem.GetComponent<RequirementItem>();
            requirementItem.Initialize(requirement);
        }

        // 设置合成按钮点击事件
        craftButton.onClick.AddListener(() => CraftRecipe(recipe));
    }

    private void CraftRecipe(CraftingData.Recipe recipe)
    {
        var craftingListManager = FindObjectOfType<CraftingListManager>();
        if (craftingListManager != null && craftingListManager.TryCraft(recipe))
        {
            Debug.Log($"成功合成 {recipe.resultItem.itemName}");
            
            UpdateCraftButtonState();
            
            foreach (Transform child in ingredientsPanel)
            {
                var requirementItem = child.GetComponent<RequirementItem>();
                requirementItem.Initialize(requirementItem.GetRequirement()); // 使用GetRequirement方法
            }
        }
        else
        {
            Debug.LogWarning($"合成失败：材料不足");
        }
    }

    private void UpdateCraftButtonState()
    {
        var craftingListManager = FindObjectOfType<CraftingListManager>();
        if (craftingListManager != null && currentRecipe != null)
        {
            craftButton.interactable = craftingListManager.HasEnoughMaterials(currentRecipe.requirements);
        }
    }
}