using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ShoppingCartManager : MonoBehaviour
{
    public static ShoppingCartManager Instance;

    private List<Item> cartItems = new List<Item>();
    private int totalCost = 0;
    public Player player; // 玩家对象

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 购买物品的方法
    public void BuyItem(Item item)
    {
        if (player.money >= item.itemData.price) // 如果玩家金钱足够
        {
            player.money -= item.itemData.price; // 扣除玩家金钱
            // 启动更新金钱特效的协程
            StartCoroutine(InGameUI.instance.UpdateMoneyEffect(player.money + item.itemData.price, player.money));
            player.inventoryManager.Add(item); // 将物品添加到玩家库存
            SoundManager.Instance.Play("EFFECT/Click", SoundType.EFFECT); // 播放点击音效
        }
        else
        {
            InGameUI.instance.ShakingText(); // 金钱不足时抖动文本
        }
        // 启动重置购买状态的协程
        StartCoroutine(ResetBuyState());
    }

    // 重置购买状态的协程
    private IEnumerator ResetBuyState()
    {
        yield return new WaitForSeconds(0.3f); // 等待0.3秒
    }

    // 更新购物车UI
    private void UpdateCartUI()
    {
        // 这里可以调用UI管理类来更新购物车界面
    }
}