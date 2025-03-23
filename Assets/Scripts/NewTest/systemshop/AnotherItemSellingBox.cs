using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 另一个物品售卖箱类，用于处理物品售卖相关逻辑
public class AnotherItemSellingBox : MonoBehaviour
{
    public GameObject sellingPanel; // 售卖面板
    public Image sellingIcon; // 售卖物品图标
    public TextMeshProUGUI priceText; // 价格文本
    public TextMeshProUGUI countText; // 数量文本
    public Button UIcontrolButton;   // UI控制按钮
    private bool isObjectActive = false; // 标记目标物体的激活状态
    public Button plusButton; // 加号按钮
    public Button minusButton; // 减号按钮
    public Button confirmButton; // 确认按钮
    public int totalSellingPrice = 0; // 售卖总价

    private int currentItemPrice = 0; // 物品当前价格
    private int currentItemCount = 0; // 物品当前售卖数量
    private Inventory.Slot selectedInventorySlot; // 选中的物品槽

    // 初始化方法，在脚本实例被启用时调用
    private void Start()
    {
        if (sellingPanel != null)
        {
            sellingPanel.SetActive(false); // 隐藏售卖面板
        }

        UIcontrolButton.onClick.AddListener(ToggleObject);// 为UI控制按钮添加点击事件监听器
        plusButton.onClick.AddListener(OnPlusButtonClicked); // 为加号按钮添加点击事件监听器
        minusButton.onClick.AddListener(OnMinusButtonClicked); // 为减号按钮添加点击事件监听器
        confirmButton.onClick.AddListener(OnConfirmButtonClicked); // 为确认按钮添加点击事件监听器

        SetupPanel(); // 初始化面板
    }

    private void ToggleObject()// 切换目标物体的激活状态
    {
        // 切换目标物体的激活状态
        isObjectActive = !isObjectActive;
        if (sellingPanel != null)
        {
            sellingPanel.SetActive(isObjectActive);
        }
    }

    // 初始化面板的方法
    private void SetupPanel()
    {
        sellingIcon.sprite = null; // 清空售卖图标
        currentItemPrice = 0; // 重置物品价格
        currentItemCount = 0; // 重置物品数量
    }

    // 更新面板显示的方法
    private void UpdatePanelDisplay()
    {
        if (selectedInventorySlot != null)
        {
            // 根据物品槽的状态设置售卖图标
            sellingIcon.sprite = selectedInventorySlot.currentCount > 0 && selectedInventorySlot.isSellable ? selectedInventorySlot.icon : null;
            // 根据物品槽的状态设置售卖图标的透明度
            sellingIcon.color = selectedInventorySlot.currentCount > 0 && selectedInventorySlot.isSellable ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);
            priceText.text = currentItemPrice.ToString(); // 更新价格文本
            countText.text = currentItemCount.ToString(); // 更新数量文本
        }
    }

    // 加号按钮点击事件处理方法
    private void OnPlusButtonClicked()
    {
        // SoundManager.Instance.Play("EFFECT/Click", SoundType.EFFECT); // 播放点击音效
        SoundManager.Instance.Play("控制物品售卖时的音效");
        // 如果选中的物品槽存在，物品数量大于0且当前售卖数量小于物品槽内物品数量
        if (selectedInventorySlot != null && selectedInventorySlot.currentCount > 0 && currentItemCount < selectedInventorySlot.currentCount)
        {
            if (selectedInventorySlot.isSellable)
            {
                currentItemCount++; // 增加售卖数量
                currentItemPrice = currentItemCount * selectedInventorySlot.price; // 计算新的物品价格
                UpdatePanelDisplay(); // 更新面板显示
            }
        }
    }

    // 减号按钮点击事件处理方法
    private void OnMinusButtonClicked()
    {
        // SoundManager.Instance.Play("EFFECT/Click", SoundType.EFFECT); // 播放点击音效
        SoundManager.Instance.Play("控制物品售卖时的音效");
        // 如果选中的物品槽存在，物品数量大于0且当前售卖数量大于0
        if (selectedInventorySlot != null && selectedInventorySlot.currentCount > 0 && currentItemCount > 0)
        {
            if (selectedInventorySlot.isSellable)
            {
                currentItemCount--; // 减少售卖数量
                currentItemPrice = currentItemCount * selectedInventorySlot.price; // 计算新的物品价格
                UpdatePanelDisplay(); // 更新面板显示
            }
        }
    }

    // 确认按钮点击事件处理方法
    private void OnConfirmButtonClicked()
    {
        // SoundManager.Instance.Play("EFFECT/Click", SoundType.EFFECT); // 播放点击音效
        SoundManager.Instance.Play("控制物品售卖时的音效");
        // 如果选中的物品槽存在，物品槽内物品数量大于等于0且当前售卖数量大于0
        if (selectedInventorySlot != null && selectedInventorySlot.currentCount >= 0 && currentItemCount > 0)
        {
            if (selectedInventorySlot.isSellable)
            {
                totalSellingPrice += currentItemPrice; // 累加售卖总价
                selectedInventorySlot.currentCount -= currentItemCount; // 减少物品槽内物品数量

                // 如果物品槽内物品数量为0，则清空物品槽
                if (selectedInventorySlot.currentCount == 0)
                {
                    selectedInventorySlot.ClearAll();
                }
                InGameUI.instance.RefreshInventoryUI("Toolbar"); // 刷新背包UI
                SetupPanel(); // 初始化面板
                // 调用 SellItems 方法更新玩家金钱
                SellItemsFromInventory();
            }
        }
    }

    // 售卖物品的方法
    public void SellItemsFromInventory()
    {
        int dailyIncome = GetTotalSellingPrice(); // 获取售卖总价
        int newTotalMoney = Player.Instance.money + dailyIncome; // 计算新的总金钱

        // 启动更新金钱效果的协程
        StartCoroutine(InGameUI.instance.UpdateMoneyEffect(Player.Instance.money, newTotalMoney));
    }

    // 获取售卖总价的方法
    public int GetTotalSellingPrice()
    {
        return totalSellingPrice;
    }

    // 重置售卖总价的方法
    public void ResetTotalSellingPrice()
    {
        totalSellingPrice = 0;
    }
}