using UnityEngine;

public class PlantGrowthTimer : MonoBehaviour
{
    private TimeManager timeManager;
    public float growthInterval = 300f; // 默认5分钟
    private float timer = 0f;

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
            //timeManager.OnDayEnd?.Invoke();
            timeManager.TriggerDayEnd(); // 调用公共方法
        }
    }

    public void SetGrowthInterval(float minutes)
    {
        growthInterval = Mathf.Max(60f, minutes * 60f); // 最小1分钟
    }
}