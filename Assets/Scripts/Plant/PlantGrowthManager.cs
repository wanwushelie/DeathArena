using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantGrowthManager : MonoBehaviour
{
    private Dictionary<Vector3Int, PlantData> plantDataDict = new Dictionary<Vector3Int, PlantData>(); // 按瓷砖位置存储种植植物的数据
    private Dictionary<Vector3Int, int> plantGrowthDays = new Dictionary<Vector3Int, int>(); // 存储播种日期
    private Dictionary<Vector3Int, int> currentGrowthStages = new Dictionary<Vector3Int, int>(); // 存储当前生长阶段
    private List<PlantSaveData> plantSaveDataList = new List<PlantSaveData>(); // 用于保存和加载的植物数据列表
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

    /// <summary>
    /// 播种，在指定位置播种指定植物的种子。
    /// </summary>
    /// <param name="position">播种的瓷砖位置。</param>
    /// <param name="plantData">要播种的植物数据。</param>
    public void PlantSeed(Vector3Int position, PlantData plantData)
    {
        // 检查该位置是否存在瓷砖且瓷砖名称为 "PlowedTile"
        if (GameManager.instance.tileManager.DoesTileExist(position) && GameManager.instance.tileManager.GetTileName(position) == "PlowedTile")
        {
            // 播放播种音效
            // SoundManager.Instance.Play("EFFECT/Seeded", SoundType.EFFECT);
            SoundManager.Instance.Play("播种音效");

            // 更改瓷砖状态，设置为已播种瓷砖
            GameManager.instance.tileManager.SetTileState(position, "Seeded");
            GameManager.instance.tileManager.seedMap.SetTile(position, GameManager.instance.tileManager.plantedTile);

            // 将植物数据存储到字典中
            plantDataDict[position] = plantData;
            // 输出当前播种的植物名称
            Debug.Log(plantDataDict[position].plantName);
            // 初始化该位置植物的生长天数为 0
            plantGrowthDays[position] = 0;
            // 初始化该位置植物的当前生长阶段为 0
            currentGrowthStages[position] = 0;
        }
    }

    // 植物生长
    private IEnumerator GrowPlant(Vector3Int position)
    {
        // 检查瓷砖是否存在且植物未成熟
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

            // 更改为下一生长阶段的瓷砖
            GameManager.instance.tileManager.seedMap.SetTile(position, plantData.growthStagesTiles[currentStage - 1]);
            plantData.growthStagesTiles[currentStage - 1].colliderType = Tile.ColliderType.Sprite;//瓷砖的碰撞体将基于瓷砖的精灵形状生成

            currentGrowthStages[position] = currentStage;

            // 如果完成所有生长阶段，则更改为“已成熟”状态
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
            // GetCellCenterWorld() : 返回该瓷砖位置中心对应的世界坐标
            Vector3 spawnPosition = GameManager.instance.tileManager.interactableMap.GetCellCenterWorld(position);
            GameObject plant = Instantiate(plantData.plantPrefab, spawnPosition, Quaternion.identity);

            Rigidbody2D rb = plant.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                StartCoroutine(FloatAndLand(plant));
            }
        }
        // 从保存数据中删除
        plantSaveDataList.RemoveAll(data => data.position == position);
        RemovePlantData(position);
    }

    private IEnumerator FloatAndLand(GameObject plant)
    {
        float floatDuration = 0.5f;
        float landDuration = 0.5f;
        float smoothTime = 0.2f; // 平滑移动时间
        Vector2 velocity = Vector2.zero; // 用于管理速度的变量

        Vector2 initialPosition = plant.transform.position;
        Vector2 floatTargetPosition = initialPosition + new Vector2(0, 0.5f); // 稍微向上漂浮的目标位置

        float elapsedTime = 0;

        Item interactable = plant.GetComponent<Item>();
        if (interactable != null)
            interactable.canInteract = false;

        // 向上平滑漂浮动画
        while (elapsedTime < floatDuration)
        {
            plant.transform.position = Vector2.SmoothDamp(plant.transform.position, floatTargetPosition, ref velocity, smoothTime);
            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        // 设置为准确位置
        plant.transform.position = floatTargetPosition;

        // 稍微等待
        yield return new WaitForSeconds(0.1f);

        // 着陆时再次初始化速度
        velocity = Vector2.zero;
        elapsedTime = 0;

        // 向下平滑着陆动画
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

        // 最后设置为准确的着陆位置
        plant.transform.position = initialPosition;
    }

    /// <summary>
    /// 当一天结束时触发的方法，处理浇水植物的生长和瓷砖颜色恢复。
    /// </summary>
    void OnDayEnd()
    {
        // 预先将wateredTiles的键复制到List中，避免在迭代过程中修改字典导致异常
        List<Vector3Int> wateredTilesKey = GameManager.instance.tileManager.GetWateredTilesKeys();

        // 仅让浇水的植物生长
        foreach (var position in wateredTilesKey)
        {
            // 检查该位置是否已浇水且存在植物生长数据
            if (GameManager.instance.tileManager.GetWateringTile(position) && plantGrowthDays.ContainsKey(position))
            {
                // 启动植物生长协程
                StartCoroutine(GrowPlant(position));
                // 重置该位置的浇水状态为未浇水
                GameManager.instance.tileManager.SetWateringTile(position, false);
            }

            // 获取该位置的瓷砖
            TileBase tile = GameManager.instance.tileManager.interactableMap.GetTile(position);
            // 如果瓷砖存在，将其颜色设置为白色
            if (tile != null)
                GameManager.instance.tileManager.interactableMap.SetColor(position, Color.white);
        }
    }

    // 保存植物状态
    public List<PlantSaveData> SavePlantDataList()
    {
        plantSaveDataList.Clear(); // 初始化现有保存数据

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

    // 根据保存的植物数据设置瓷砖相关信息
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
