using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantGrowthManager : MonoBehaviour
{
    private Dictionary<Vector3Int, PlantData> plantDataDict = new Dictionary<Vector3Int, PlantData>(); // 타일 위치별로 심어진 식물의 데이터 저장
    private Dictionary<Vector3Int, int> plantGrowthDays = new Dictionary<Vector3Int, int>(); // 씨앗 심은 날 저장
    private Dictionary<Vector3Int, int> currentGrowthStages = new Dictionary<Vector3Int, int>(); // 현재 성장 단계 저장
    private List<PlantSaveData> plantSaveDataList = new List<PlantSaveData>(); // 저장&로드에 사용할 식물 데이터 리스트
    private TimeManager timeManager;


    private void Start()
    {
        timeManager = GameManager.instance.timeManager;
        if (timeManager != null)
        {
            timeManager.OnDayEnd += OnDayEnd;
        }
    }

    private void OnDestroy()
    {
        if (timeManager != null)
        {
            timeManager.OnDayEnd -= OnDayEnd;
        }
    }

    // 种植种子
    public void PlantSeed(Vector3Int position, PlantData plantData)
    {
        if (GameManager.instance.tileManager.DoesTileExist(position) && GameManager.instance.tileManager.GetTileName(position) == "PlowedTile")
        {
            SoundManager.Instance.Play("EFFECT/Seeded", SoundType.EFFECT);

            // 更改瓷砖状态，设置为已播种的瓷砖
            GameManager.instance.tileManager.SetTileState(position, "Seeded");
            GameManager.instance.tileManager.seedMap.SetTile(position, GameManager.instance.tileManager.plantedTile);

            plantDataDict[position] = plantData;
            Debug.Log(plantDataDict[position].plantName);
            plantGrowthDays[position] = 0;
            currentGrowthStages[position] = 0;
        }
    }

    // 植物生长
    private IEnumerator GrowPlant(Vector3Int position)
    {
        if (!GameManager.instance.tileManager.DoesTileExist(position) || GameManager.instance.tileManager.GetTileState(position) == "Grown")
        {
            yield break;
        }

        PlantData plantData = plantDataDict[position];
        int currentStage = currentGrowthStages[position];
        int currentGrowthDay = plantGrowthDays[position];

        // 检查每个生长阶段是否有效
        if (currentStage >= 0 && currentStage < plantData.growthStagesTiles.Length)
        {
            currentStage++;
            currentGrowthDay++;

            // 更改为下一个生长阶段的瓷砖
            GameManager.instance.tileManager.seedMap.SetTile(position, plantData.growthStagesTiles[currentStage - 1]);
            plantData.growthStagesTiles[currentStage - 1].colliderType = Tile.ColliderType.Sprite;

            currentGrowthStages[position] = currentStage;

            // 如果完成了所有生长阶段，则将状态更改为“Grown已生长”
            if (currentGrowthStages[position] >= plantData.growthStagesTiles.Length)
            {
                GameManager.instance.tileManager.SetTileState(position, "Grown");
            }
            else
            {
                GameManager.instance.tileManager.SetTileState(position, "Growing");
            }
        }

        plantGrowthDays[position] = currentGrowthDay;

        yield return null;
    }

    // 收获植物
    public void HarvestPlant(Vector3Int position)
    {
        PlantData plantData = GetPlantData(position);

        if (plantData != null)
        {
            // GetCellCenterWorld() : 返回该瓷砖位置中心的世界坐标
            Vector3 spawnPosition = GameManager.instance.tileManager.interactableMap.GetCellCenterWorld(position);
            GameObject plant = Instantiate(plantData.plantPrefab, spawnPosition, Quaternion.identity);

            Rigidbody2D rb = plant.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                StartCoroutine(FloatAndLand(plant));
            }
        }
        // 从保存数据中也删除
        plantSaveDataList.RemoveAll(data => data.position == position);
        RemovePlantData(position);
    }

    private IEnumerator FloatAndLand(GameObject plant)
    {
        float floatDuration = 0.5f;
        float landDuration = 0.5f;
        float smoothTime = 0.2f; // 平滑移动所需的时间
        Vector2 velocity = Vector2.zero; // 用于管理速度的变量

        Vector2 initialPosition = plant.transform.position;
        Vector2 floatTargetPosition = initialPosition + new Vector2(0, 0.5f); // 稍微向上浮动的目标位置

        float elapsedTime = 0;

        Item interactable = plant.GetComponent<Item>();
        if (interactable != null)
            interactable.canInteract = false;

        // 向上平滑浮动的动画
        while (elapsedTime < floatDuration)
        {
            plant.transform.position = Vector2.SmoothDamp(plant.transform.position, floatTargetPosition, ref velocity, smoothTime);
            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        // 设置为精确位置
        plant.transform.position = floatTargetPosition;

        // 稍作等待
        yield return new WaitForSeconds(0.1f);

        // 着陆时再次初始化速度
        velocity = Vector2.zero;
        elapsedTime = 0;

        // 向下平滑着陆的动画
        while (elapsedTime < landDuration)
        {
            if (plant != null)
            {
                plant.transform.position = Vector2.SmoothDamp(plant.transform.position, initialPosition, ref velocity, smoothTime);
                elapsedTime += Time.deltaTime;
            }
            if (interactable != null)
            {
                interactable.canInteract = true;
            }

            yield return null; // 等待下一帧
        }

        // 最后设置为精确的着陆位置
        plant.transform.position = initialPosition;
    }

    void OnDayEnd()
    {
        // 预先复制wateredTiles的键并存储在列表中
        List<Vector3Int> wateredTilesKey = GameManager.instance.tileManager.GetWateredTilesKeys();

        // 只让浇水的植物生长
        foreach (var position in wateredTilesKey)
        {
            if (GameManager.instance.tileManager.GetWateringTile(position) && plantGrowthDays.ContainsKey(position))
            {
                StartCoroutine(GrowPlant(position));
                GameManager.instance.tileManager.SetWateringTile(position, false);
            }

            TileBase tile = GameManager.instance.tileManager.interactableMap.GetTile(position);
            if (tile != null)
                GameManager.instance.tileManager.interactableMap.SetColor(position, Color.white);
        }
    }

    // 保存植物的状态
    public List<PlantSaveData> SavePlantDataList()
    {
        plantSaveDataList.Clear(); // 初始化现有的保存数据

        foreach (var position in plantDataDict.Keys)
        {
            string plantName = plantDataDict[position].plantName;
            int growthStage = currentGrowthStages[position];
            int growthDay = plantGrowthDays[position];
            string currentState = GameManager.instance.tileManager.GetTileState(position);
            bool isWatered = GameManager.instance.tileManager.GetWateringTile(position);

            plantSaveDataList.Add(new PlantSaveData(plantName, position, growthStage, growthDay, currentState, isWatered));
        }
        return plantSaveDataList;
    }

    // 根据保存的植物数据设置瓷砖和相关信息
    public void SetTilePlantSaveData(List<PlantSaveData> plantSaveDataList)
    {
        foreach (var saveData in plantSaveDataList)
        {
            Vector3Int position = saveData.position;
            PlantData plantData = saveData.plantData;
            int currentGrowthStage = saveData.growthStage;
            int currentGrowthDay = saveData.growthDay;
            string currentState = saveData.currentState;
            bool isWatered = saveData.isWatered;

            if (GameManager.instance.tileManager == null && GameManager.instance.tileManager.seedMap == null)
                return;

            GameManager.instance.tileManager.SetTileState(position, currentState);
            GameManager.instance.tileManager.interactableMap.SetTile(position, GameManager.instance.tileManager.interactedTile);

            if (currentGrowthStage - 1 >= 0 && currentGrowthStage < plantData.growthStagesTiles.Length)
            {
                GameManager.instance.tileManager.seedMap.SetTile(position, plantData.growthStagesTiles[currentGrowthStage - 1]);
            }

            plantDataDict[position] = plantData;
            plantGrowthDays[position] = currentGrowthDay;
            currentGrowthStages[position] = currentGrowthStage;
            GameManager.instance.tileManager.SetWateringTile(position, isWatered);
        }
    }

    public PlantData GetPlantData(Vector3Int position)
    {
        if (plantDataDict.ContainsKey(position))
            return plantDataDict[position];
        return null;
    }

    public void RemovePlantData(Vector3Int position)
    {
        plantDataDict.Remove(position);
        currentGrowthStages.Remove(position);
        plantGrowthDays.Remove(position);
    }

    public void ClearPlantSaveData()
    {
        plantSaveDataList.Clear();
    }
}



