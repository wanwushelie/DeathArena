using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

// 商店类，处理商店的逻辑
public class Shop : MonoBehaviour
{
    public GameObject shopPanel; // 商店面板
    public Button riceBuyBtn; // 大米购买按钮
    public Button tomatoBuyBtn; // 西红柿购买按钮
    public List<Item> sellItems; // 出售的物品列表

    private bool isPlayerInRange = false; // 玩家是否在范围内
    private bool isBuying = false; // 是否正在购买
    public bool isOpenShopPanel = false; // 商店面板是否打开
    private Player player; // 玩家对象

    // 游戏开始时调用
    private void Start()
    {
        player = FindObjectOfType<Player>(); // 查找玩家对象

        // 为大米购买按钮添加点击事件
        riceBuyBtn.onClick.AddListener(() => BuyItem(sellItems[0]));
        // 为西红柿购买按钮添加点击事件
        tomatoBuyBtn.onClick.AddListener(() => BuyItem(sellItems[1]));
    }

    // 购买物品的方法
    public void BuyItem(Item item)
    {
        if (isBuying) return; // 如果正在购买，直接返回

        isBuying = true; // 标记为正在购买
        if (sellItems.Contains(item)) // 如果出售列表中包含该物品
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
        }
        // 启动重置购买状态的协程
        StartCoroutine(ResetBuyState());
    }

    // 重置购买状态的协程
    private IEnumerator ResetBuyState()
    {
        yield return new WaitForSeconds(0.3f); // 等待0.3秒
        isBuying = false; // 标记为不在购买
    }

    // 玩家进入商店触发区域时调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player") // 如果进入的是玩家
        {
            isPlayerInRange = true; // 标记玩家在范围内
            shopPanel.SetActive(true); // 激活商店面板
            isOpenShopPanel = true; // 标记商店面板已打开
            InGameUI.instance.ToggleInventoryUI(); // 切换库存UI
        }
    }

    // 玩家离开商店触发区域时调用
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player") // 如果离开的是玩家
        {
            isPlayerInRange = false; // 标记玩家不在范围内
            shopPanel.SetActive(false); // 关闭商店面板
            isOpenShopPanel = false; // 标记商店面板已关闭
            InGameUI.instance.ToggleInventoryUI(); // 切换库存UI
        }
    }
}