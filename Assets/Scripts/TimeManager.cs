using System;
using System.Collections;
using TMPro;
using UnityEngine;

// 时间流逝管理
public class TimeManager : MonoBehaviour
{
    // 一天结束时触发的事件
    public event Action OnDayEnd;
    // 显示日期的文本组件
    public TextMeshProUGUI dayText;
    // 显示时间的文本组件
    public TextMeshProUGUI timeText;
    // 指示是否正在结束一天
    public bool isDayEnding = false;
    // 游戏中的小时数
    public int gameHour = 9;
    // 游戏中的分钟数
    public int gameMinute = 0;
    // 当前日期在星期数组中的索引
    public int currentDayIndex = 0;
    // 当前的天数
    public int day = 1;

    // 星期数组
    private string[] daysOfWeek = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
    // 游戏中每分钟对应的实际时间（秒）
    private float timePerGameMinute = 10f;
    // 当前已经过去的时间
    private float currentTime = 0f;
    // 物品出售箱的引用
    private ItemSellingBox itemBox;

    private void Start()
    {
        // 获取物品出售箱的引用
        itemBox = GameManager.instance.itemBox;
        // 更新时间UI显示
        UpdateTimeUI();
    }

    private void Update()
    {
        // 如果正在结束一天，则不进行时间更新
        if (isDayEnding) return;

        // 累加当前时间
        currentTime += Time.deltaTime;

        // 如果已经过去的时间达到了游戏中一分钟对应的时间
        if (currentTime >= timePerGameMinute)
        {
            // 重置已经过去的时间
            currentTime = 0f;
            // 游戏中的分钟数增加10
            gameMinute += 10;

            // 如果分钟数达到60
            if (gameMinute >= 60)
            {
                // 分钟数重置为0
                gameMinute = 0;
                // 小时数加1
                gameHour++;
            }

            // 如果小时数达到24
            if (gameHour >= 24)
            {
                // 开始一天结束的协程
                StartCoroutine(EndDay());
            }

            // 更新时间UI显示
            UpdateTimeUI();
        }
    }

    private void NextDay()
    {
        // 小时数重置为9
        gameHour = 9;
        // 分钟数重置为0
        gameMinute = 0;
        // 更新当前日期在星期数组中的索引
        currentDayIndex = (currentDayIndex + 1) % daysOfWeek.Length;
        // 天数加1
        day++;
        // 标记为不在结束一天
        isDayEnding = false;

        // 出售物品
        itemBox.SellItems();
        // 重置出售价格
        itemBox.ResetSellingPrice();

        // 更新时间UI显示
        UpdateTimeUI();
        // 设置玩家的位置
        Player.Instance.SetPosition();
        // 找到工具栏UI组件
        Toolbar_UI toolbar_UI = FindObjectOfType<Toolbar_UI>();
        // 选择第一个槽位
        toolbar_UI.SelectSlot(0);

        // 触发一天结束的事件
        //OnDayEnd?.Invoke();
    }

    public IEnumerator EndDay()
    {
        // 标记为正在结束一天
        isDayEnding = true;
        // 禁用玩家的动画组件
        Player.Instance.anim.enabled = false;
        // 开始淡入屏幕的协程
        yield return StartCoroutine(FadeEffect.instance.FadeScreen(1f));
        // 处理进入下一天的逻辑
        NextDay();
        // 开始淡出屏幕的协程
        yield return StartCoroutine(FadeEffect.instance.FadeScreen(0f));
        // 标记为不在结束一天
        isDayEnding = false;
    }

    public void UpdateTimeUI()
    {
        // 更新日期文本显示
        dayText.text = $"{daysOfWeek[currentDayIndex]}\n{day}";
        // 更新时间文本显示
        timeText.text = $"{gameHour:D2} : {gameMinute:D2}";
    }
    public void TriggerDayEnd()
    {
        OnDayEnd?.Invoke();
    }
}
