using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemData", menuName = "Shop/Shop Item Data")]
public class ShopItemData : ScriptableObject
{
    public Item item; // 商品项
    public string category; // 商品分类
    public Sprite knowledgeIntroductionImage; // 知识介绍图片
    public bool isUnlocked; // 是否解锁
    public int price; // 商品价格
}