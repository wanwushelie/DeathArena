using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemData", menuName = "Data/Shop Item Data")]
public class ShopItemData : ScriptableObject
{
    public string category; //  商品分类
    public string itemIntroduce; //  商品介绍
    public string itemStory; //  商品典故

    // public Sprite knowledgeIntroductionImage; //  知识介绍图片
    public bool isUnlocked; // 是否解锁
}