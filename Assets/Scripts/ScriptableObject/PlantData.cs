using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Plant Data", menuName = "Data/Plant Data")]
public class PlantData : ScriptableObject
{
    public string plantName = "Plant Name";
    public GameObject plantPrefab;
    public Tile[] growthStagesTiles;
    public int[] growthTimes;
}
