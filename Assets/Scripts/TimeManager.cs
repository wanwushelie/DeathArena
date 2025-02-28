using System;
using System.Collections;
using TMPro;
using UnityEngine;

/* 时间流逝管理 */

public class TimeManager : MonoBehaviour
{
    public event Action OnDayEnd; // 每天结束时触发的事件
    public TextMeshProUGUI dayText; // 显示日期的 UI 元素
    public TextMeshProUGUI timeText; // 显示时间的 UI 元素
    public bool isDayEnding = false; // 标记是否正在结束一天
    public int gameHour = 9; // 当前游戏小时
    public int gameMinute = 0; // 当前游戏分钟
    public int currentDayIndex = 0; // 当前星期几的索引
    public int day = 1; // 当前天数

    private string[] daysOfWeek = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" }; // 星期数组
    private float timePerGameMinute = 10f; // 每分钟的时间流逝速度
    private float currentTime = 0f; // 当前累计时间
    private ItemSellingBox itemBox; // 物品出售箱引用

    private void Start()
    {
        itemBox = GameManager.instance.itemBox;
        UpdateTimeUI();
    }

    private void Update()
    {
        if (isDayEnding) return;

        currentTime += Time.deltaTime;

        if (currentTime >= timePerGameMinute)
        {
            currentTime = 0f;
            gameMinute += 10;

            if (gameMinute >= 60)
            {
                gameMinute = 0;
                gameHour++;
            }

            if (gameHour >= 24)
            {
                StartCoroutine(EndDay());
            }

            UpdateTimeUI();
        }
    }

    private void NextDay()
    {
        gameHour = 9;
        gameMinute = 0;
        currentDayIndex = (currentDayIndex + 1) % daysOfWeek.Length;
        day++;
        isDayEnding = false;

        itemBox.SellItems();
        itemBox.ResetSellingPrice();

        UpdateTimeUI();
        Player.Instance.SetPosition();
        Toolbar_UI toolbar_UI = FindObjectOfType<Toolbar_UI>();
        toolbar_UI.SelectSlot(0);
        
        OnDayEnd?.Invoke();

    }

    public IEnumerator EndDay()
    {
        isDayEnding = true;
        Player.Instance.anim.enabled = false;
        yield return StartCoroutine(FadeEffect.instance.FadeScreen(1f));
        NextDay();
        yield return StartCoroutine(FadeEffect.instance.FadeScreen(0f));
        isDayEnding = false;
    }

    public void UpdateTimeUI()
    {
        dayText.text = $"{daysOfWeek[currentDayIndex]}\n{day}";
        timeText.text = $"{gameHour:D2} : {gameMinute:D2}";
    }
}
