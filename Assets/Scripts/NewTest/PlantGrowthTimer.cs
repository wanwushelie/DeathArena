using UnityEngine;
using System.Collections;

public class PlantGrowthTimer : MonoBehaviour
{
    private TimeManager timeManager;
    public float growthInterval = 300f; // 默认5分钟
    private float timer = 0f;
    //引用ClickToDrawRuleTile脚本中自动浇水的方法，拖拽赋值
    public ClickToDrawRuleTile clickToDrawRuleTile;

    private void Start()
    {
        timeManager = GameManager.instance.timeManager;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= growthInterval)
        {
            timer = 0f;
            // 先更新水渠状态再触发日结
            clickToDrawRuleTile.UpdateWaterFlow(); 
            timeManager.TriggerDayEnd();
            
            // 添加延迟确保状态重置完成
            StartCoroutine(DelayedWaterUpdate());
        }
    }

    private IEnumerator DelayedWaterUpdate()
    {
        yield return new WaitForSeconds(0.1f);
        // 再次更新水渠确保灌溉
        clickToDrawRuleTile.UpdateWaterFlow();
    }

    public void SetGrowthInterval(float minutes)
    {
        growthInterval = Mathf.Max(60f, minutes * 60f); // 最小1分钟
    }
}