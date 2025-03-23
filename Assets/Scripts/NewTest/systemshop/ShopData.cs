using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopData", menuName = "Shop/Shop Data")]
public class ShopData : ScriptableObject
{
    public List<ShopItemData> shopItems = new List<ShopItemData>(); // 商店中的商品列表
}