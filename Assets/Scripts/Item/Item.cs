using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Item : MonoBehaviour
{
    public ItemData itemData;
    public PlantData plantData;
    public ShopItemData shopItemData; // 新增：可选的商店数据
    public FoodData foodData;       // 食物数据
    public bool canInteract;
    public bool isDropped;
    public int droppedCount;

    [HideInInspector] public Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        canInteract = true;
    }
    public void SetDroppedItemCount(int count)
    {
        isDropped = true;
        droppedCount = count;
    }
    public int GetDroppedItemCount()
    {
        return isDropped ? droppedCount : 1;
    }
}