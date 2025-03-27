using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemData", menuName = "Data/Shop Item Data")]
public class ShopItemData : ScriptableObject
{
    public string category; //  商品分类
    public Sprite knowledgeIntroductionImage; //  知识介绍图片
    public bool isUnlocked; // 是否解锁
}