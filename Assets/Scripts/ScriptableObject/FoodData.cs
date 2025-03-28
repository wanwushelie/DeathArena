using UnityEngine;

[CreateAssetMenu(fileName = "Food Data", menuName = "Data/Food Data")]
public class FoodData : ScriptableObject
{
    public float satiationRecovery; //  恢复的饱腹值
    public float healthRecovery;    // 恢复的血量
}