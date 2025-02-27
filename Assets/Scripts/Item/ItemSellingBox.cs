using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 物品售卖箱类，用于处理物品售卖相关逻辑
public class ItemSellingBox : MonoBehaviour
{
    public Animator anim; // 动画控制器
    public GameObject sellingPanel; // 售卖面板
    public Image sellingIcon; // 售卖物品图标
    public TextMeshProUGUI priceText; // 价格文本
    public TextMeshProUGUI countText; // 数量文本
    public Button minusBtn; // 减号按钮
    public Button plusBtn; // 加号按钮
    public Button checkBtn; // 确认按钮
    public bool isBoxOpen = false; // 售卖箱是否打开
    public int sellingPrice = 0; // 售卖总价

    private bool isPlayerInRange = false; // 玩家是否在范围内
    private int itemPrice = 0; // 物品当前价格
    private int itemCount = 0; // 物品当前售卖数量
    private Inventory.Slot selectedSlot; // 选中的物品槽
    public bool isOpenItemSellingBox = false; // 是否打开了物品售卖箱（和上面重复了）

    // 初始化方法，在脚本实例被启用时调用
    private void Start()
    {
        anim = GetComponent<Animator>(); // 获取动画控制器组件
        if (sellingPanel != null)
        {
            sellingPanel.SetActive(false); // 隐藏售卖面板
        }

        plusBtn.onClick.AddListener(OnPlusButtonClick); // 为加号按钮添加点击事件监听器
        minusBtn.onClick.AddListener(OnMinusButtonClick); // 为减号按钮添加点击事件监听器
        checkBtn.onClick.AddListener(OnCheckButtonClick); // 为确认按钮添加点击事件监听器

        InitializePanel(); // 初始化面板
    }

    // 每帧更新方法
    private void Update()
    {
        // 如果玩家在范围内，点击鼠标左键且售卖箱未打开，则打开售卖箱
        if (isPlayerInRange && Input.GetMouseButtonDown(0) && !isBoxOpen)
        {
            OpenItemBox();
        }

        // 检查当前选中的物品槽是否发生变化
        var currentSlot = Player.Instance.inventoryManager.toolbar.selectedSlot;
        if (currentSlot != selectedSlot)
        {
            selectedSlot = currentSlot;
            InitializePanel(); // 物品槽变化时初始化面板
        }
        UpdatePanel(); // 更新面板显示
    }

    // 打开物品售卖箱的方法
    private void OpenItemBox()
    {
        isBoxOpen = true; // 标记售卖箱已打开
        anim.SetBool("isOpen", isBoxOpen); // 设置动画状态
        sellingPanel.SetActive(true); // 显示售卖面板
        isOpenItemSellingBox = true;

        InitializePanel(); // 初始化面板

        // 如果游戏内UI存在且背包未打开，则打开背包
        if (InGameUI.instance != null && !InGameUI.instance.isInventoryOpen)
        {
            InGameUI.instance.ToggleInventoryUI();
        }
    }

    // 关闭物品售卖箱的方法
    private void CloseItemBox()
    {
        isBoxOpen = false; // 标记售卖箱已关闭
        anim.SetBool("isOpen", isBoxOpen); // 设置动画状态
        sellingPanel.SetActive(false); // 隐藏售卖面板
        isOpenItemSellingBox = false;

        // 如果游戏内UI存在且背包已打开，则关闭背包
        if (InGameUI.instance != null && InGameUI.instance.isInventoryOpen)
        {
            InGameUI.instance.ToggleInventoryUI();
        }
    }

    // 初始化面板的方法
    private void InitializePanel()
    {
        sellingIcon.sprite = null; // 清空售卖图标
        itemPrice = 0; // 重置物品价格
        itemCount = 0; // 重置物品数量
    }

    // 更新面板显示的方法
    private void UpdatePanel()
    {
        if (selectedSlot != null)
        {
            // 根据物品槽的状态设置售卖图标
            sellingIcon.sprite = selectedSlot.currentCount > 0 && selectedSlot.isSellable ? selectedSlot.icon : null;
            // 根据物品槽的状态设置售卖图标的透明度
            sellingIcon.color = selectedSlot.currentCount > 0 && selectedSlot.isSellable ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);
            priceText.text = itemPrice.ToString(); // 更新价格文本
            countText.text = itemCount.ToString(); // 更新数量文本
        }
    }

    // 加号按钮点击事件处理方法
    private void OnPlusButtonClick()
    {
        SoundManager.Instance.Play("EFFECT/Click", SoundType.EFFECT); // 播放点击音效
        // 如果选中的物品槽存在，物品数量大于0且当前售卖数量小于物品槽内物品数量
        if (selectedSlot != null && selectedSlot.currentCount > 0 && itemCount < selectedSlot.currentCount)
        {
            if (selectedSlot.isSellable)
            {
                itemCount++; // 增加售卖数量
                itemPrice = itemCount * selectedSlot.price; // 计算新的物品价格
                UpdatePanel(); // 更新面板显示
            }
        }
    }

    // 减号按钮点击事件处理方法
    private void OnMinusButtonClick()
    {
        SoundManager.Instance.Play("EFFECT/Click", SoundType.EFFECT); // 播放点击音效
        // 如果选中的物品槽存在，物品数量大于0且当前售卖数量大于0
        if (selectedSlot != null && selectedSlot.currentCount > 0 && itemCount > 0)
        {
            if (selectedSlot.isSellable)
            {
                itemCount--; // 减少售卖数量
                itemPrice = itemCount * selectedSlot.price; // 计算新的物品价格
                UpdatePanel(); // 更新面板显示
            }
        }
    }

    // 确认按钮点击事件处理方法
    private void OnCheckButtonClick()
    {
        SoundManager.Instance.Play("EFFECT/Click", SoundType.EFFECT); // 播放点击音效
        // 如果选中的物品槽存在，物品槽内物品数量大于等于0且当前售卖数量大于0
        if (selectedSlot != null && selectedSlot.currentCount >= 0 && itemCount > 0)
        {
            if (selectedSlot.isSellable)
            {
                sellingPrice += itemPrice; // 累加售卖总价
                selectedSlot.currentCount -= itemCount; // 减少物品槽内物品数量

                // 如果物品槽内物品数量为0，则清空物品槽
                if (selectedSlot.currentCount == 0)
                {
                    selectedSlot.ClearAll();
                }
                InGameUI.instance.RefreshInventoryUI("Toolbar"); // 刷新背包UI
                InitializePanel(); // 初始化面板
                // 调用 SellItems 方法更新玩家金钱
                SellItems();
            }
        }
    }

    // 当其他碰撞器进入触发器时调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            isPlayerInRange = true; // 标记玩家进入范围内
        }
    }

    // 当其他碰撞器离开触发器时调用
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            isPlayerInRange = false; // 标记玩家离开范围内
            if (isBoxOpen)
            {
                CloseItemBox(); // 如果售卖箱打开，则关闭它
            }
        }
    }

    // 售卖物品的方法
    public void SellItems()
    {
        int dailyEarnings = GetSellingPrice(); // 获取售卖总价
        int newTotalMoney = Player.Instance.money + dailyEarnings; // 计算新的总金钱

        // 启动更新金钱效果的协程
        StartCoroutine(InGameUI.instance.UpdateMoneyEffect(Player.Instance.money, newTotalMoney));
    }

    // 获取售卖总价的方法
    public int GetSellingPrice()
    {
        return sellingPrice;
    }

    // 重置售卖总价的方法
    public void ResetSellingPrice()
    {
        sellingPrice = 0;
    }
}