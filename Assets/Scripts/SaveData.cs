// 引入System.Collections.Generic命名空间，用于使用泛型集合，如List和Dictionary
using System.Collections.Generic;
// 引入UnityEngine命名空间，Unity引擎核心功能，如游戏对象、组件等
using UnityEngine;
// 引入System.IO命名空间，用于文件和目录操作
using System.IO;

/// <summary>
/// 可序列化的游戏存档数据类，包含玩家数据、背包数据、工具栏数据和植物数据
/// </summary>
[System.Serializable]
public class GameSaveData
{
    public PlayerData playerData; // 玩家数据
    public InventoryData backpackData; // 背包数据
    public InventoryData toolbarData; // 工具栏数据
    public PlantDataWrapper plantData; // 植物数据包装类
}

/// <summary>
/// 可序列化的玩家数据类，包含金钱、当前日期、日期索引和售价
/// </summary>
[System.Serializable]
public class PlayerData
{
    public int money; // 玩家拥有的金钱数量
    public int currentDay; // 当前游戏日期
    public int currentDayIndex; // 当前日期的索引
    public int sellingPrice; // 物品售价

    /// <summary>
    /// 构造函数，初始化玩家数据
    /// </summary>
    /// <param name="money">玩家金钱</param>
    /// <param name="currentDay">当前日期</param>
    /// <param name="currentDayIndex">当前日期索引</param>
    /// <param name="sellingPrice">物品售价</param>
    public PlayerData(int money, int currentDay, int currentDayIndex, int sellingPrice)
    {
        this.money = money;
        this.currentDay = currentDay;
        this.currentDayIndex = currentDayIndex;
        this.sellingPrice = sellingPrice;
    }
}

/// <summary>
/// 可序列化的库存槽数据类，包含物品名称、当前数量和植物名称
/// </summary>
[System.Serializable]
public class InventorySlotData
{
    public string itemName; // 物品名称
    public int currentCount; // 物品当前数量
    public string plantName; // 植物名称
}

/// <summary>
/// 可序列化的库存数据类，包含多个库存槽数据
/// </summary>
[System.Serializable]
public class InventoryData
{
    public List<InventorySlotData> slots = new List<InventorySlotData>(); // 库存槽数据列表
}

/// <summary>
/// 可序列化的植物存档数据类，包含植物数据、名称、位置、生长阶段、生长天数、当前状态和是否浇水
/// </summary>
[System.Serializable]
public class PlantSaveData
{
    public PlantData plantData; // 植物数据
    public string plantName; // 植物名称
    public Vector3Int position; // 植物位置
    public int growthStage; // 植物生长阶段
    public int growthDay; // 植物生长天数
    public string currentState; // 植物当前状态
    public bool isWatered; // 植物是否浇水

    /// <summary>
    /// 构造函数，初始化植物存档数据
    /// </summary>
    /// <param name="plantName">植物名称</param>
    /// <param name="position">植物位置</param>
    /// <param name="growthStage">植物生长阶段</param>
    /// <param name="growthDay">植物生长天数</param>
    /// <param name="currentState">植物当前状态</param>
    /// <param name="isWatered">植物是否浇水</param>
    public PlantSaveData(string plantName, Vector3Int position, int growthStage, int growthDay, string currentState, bool isWatered)
    {
        this.plantName = plantName;
        this.position = position;
        this.growthStage = growthStage;
        this.growthDay = growthDay;
        this.currentState = currentState;
        this.isWatered = isWatered;
    }
}

/// <summary>
/// 可序列化的植物数据包装类，包含多个植物存档数据
/// </summary>
[System.Serializable]
public class PlantDataWrapper
{
    public List<PlantSaveData> plants = new List<PlantSaveData>(); // 植物存档数据列表
}

/// <summary>
/// 保存和加载游戏数据的单例类
/// </summary>
public class SaveData : MonoBehaviour
{
    public static SaveData instance; // 单例实例
    public Inventory inventoryToSave = null; // 要保存的库存

    private static Dictionary<int, ItemData> allItem = new Dictionary<int, ItemData>(); // 所有物品数据字典
    private static Dictionary<int, PlantData> allPlant = new Dictionary<int, PlantData>(); // 所有植物数据字典

    /// <summary>
    /// 唤醒时初始化单例实例，并创建物品和植物数据字典
    /// </summary>
    void Awake()
    {
        if (!instance) // 如果实例为空
        {
            instance = this; // 将当前实例赋值给单例实例
        }
        CreateItemDictionary(); // 创建物品数据字典
        CreatePlantDataDictionary(); // 创建植物数据字典
    }

    /// <summary>
    /// 从Resources文件夹中查找所有ItemData对象，并将其添加到allItem字典中
    /// </summary>
    private void CreateItemDictionary()
    {
        ItemData[] allItems = Resources.FindObjectsOfTypeAll<ItemData>(); // 查找所有ItemData对象

        foreach (ItemData i in allItems) // 遍历所有ItemData对象
        {
            int key = Animator.StringToHash(i.itemName); // 将物品名称转换为哈希值作为键

            if (!allItem.ContainsKey(key)) // 如果字典中不包含该键
                allItem.Add(key, i); // 将物品数据添加到字典中
        }
    }

    /// <summary>
    /// 从Resources文件夹中查找所有PlantData对象，并将其添加到allPlant字典中
    /// </summary>
    private void CreatePlantDataDictionary()
    {
        PlantData[] allPlants = Resources.FindObjectsOfTypeAll<PlantData>(); // 查找所有PlantData对象

        foreach (PlantData i in allPlants) // 遍历所有PlantData对象
        {
            int key = Animator.StringToHash(i.plantName); // 将植物名称转换为哈希值作为键

            if (!allPlant.ContainsKey(key)) // 如果字典中不包含该键
                allPlant.Add(key, i); // 将植物数据添加到字典中
        }
    }

    /// <summary>
    /// 删除所有保存的JSON文件
    /// </summary>
    public void DeleteSavedFiles()
    {
        // 获取持久数据路径下所有的.json文件
        string[] jsonFilePaths = Directory.GetFiles(Application.persistentDataPath, "*.json");

        // 遍历每个文件并删除
        foreach (string filePath in jsonFilePaths)
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// 保存游戏数据到JSON文件
    /// </summary>
    /// <param name="backpack">背包库存</param>
    /// <param name="toolbar">工具栏库存</param>
    /// <param name="currentMoney">当前金钱</param>
    /// <param name="currentDay">当前日期</param>
    /// <param name="currentDayIndex">当前日期索引</param>
    /// <param name="sellingPrice">物品售价</param>
    /// <param name="plant">植物存档数据列表</param>
    public void SaveGameData(
        Inventory backpack, Inventory toolbar,
        int currentMoney, int currentDay, int currentDayIndex, int sellingPrice,
        List<PlantSaveData> plant)
    {
        string filePath = Application.persistentDataPath + $"/GameData.json"; // 保存文件的路径

        GameSaveData gameSaveData = new GameSaveData // 创建游戏存档数据对象
        {
            playerData = new PlayerData(currentMoney, currentDay, currentDayIndex, sellingPrice), // 初始化玩家数据
            backpackData = new InventoryData(), // 初始化背包数据
            toolbarData = new InventoryData(), // 初始化工具栏数据
            plantData = new PlantDataWrapper() // 初始化植物数据包装类
        };

        foreach (var slot in backpack.GetSlots) // 遍历背包的所有槽位
        {
            if (!slot.isEmpty) // 如果槽位不为空
            {
                gameSaveData.backpackData.slots.Add(new InventorySlotData // 将槽位数据添加到背包数据中
                {
                    itemName = slot.itemName, // 物品名称
                    currentCount = slot.currentCount, // 物品当前数量
                    plantName = slot.item?.plantData?.plantName // 植物名称
                });
            }
        }

        foreach (var slot in toolbar.GetSlots) // 遍历工具栏的所有槽位
        {
            if (!slot.isEmpty) // 如果槽位不为空
            {
                gameSaveData.toolbarData.slots.Add(new InventorySlotData // 将槽位数据添加到工具栏数据中
                {
                    itemName = slot.itemName, // 物品名称
                    currentCount = slot.currentCount, // 物品当前数量
                    plantName = slot.item?.plantData?.plantName // 植物名称
                });
            }
        }
        gameSaveData.plantData.plants = plant; // 将植物存档数据列表赋值给植物数据包装类

        string json = JsonUtility.ToJson(gameSaveData, true); // 将游戏存档数据转换为JSON字符串
        File.WriteAllText(filePath, json); // 将JSON字符串写入文件
    }

    /// <summary>
    /// 从JSON文件中加载游戏数据
    /// </summary>
    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/GameData.json"; // 加载文件的路径

        if (File.Exists(filePath)) // 如果文件存在
        {
            string json = File.ReadAllText(filePath); // 读取文件内容
            GameSaveData gameSaveData = JsonUtility.FromJson<GameSaveData>(json); // 将JSON字符串转换为游戏存档数据对象

            if (gameSaveData.playerData != null) // 如果玩家数据不为空
            {
                Player.Instance.money = gameSaveData.playerData.money; // 设置玩家金钱
                GameManager.instance.timeManager.day = gameSaveData.playerData.currentDay; // 设置当前日期
                GameManager.instance.timeManager.currentDayIndex = gameSaveData.playerData.currentDayIndex; // 设置当前日期索引
                GameManager.instance.itemBox.sellingPrice = gameSaveData.playerData.sellingPrice; // 设置物品售价

                if (gameSaveData.playerData.sellingPrice > 0) // 如果物品售价大于0
                {
                    GameManager.instance.itemBox.SellItems(); // 出售物品
                    GameManager.instance.itemBox.ResetSellingPrice(); // 重置物品售价
                }
            }
            LoadInventoryFromData(InventoryManager.instance.backpack, gameSaveData.backpackData); // 从数据中加载背包库存
            LoadInventoryFromData(InventoryManager.instance.toolbar, gameSaveData.toolbarData); // 从数据中加载工具栏库存
            LoadPlantFromData(gameSaveData.plantData); // 从数据中加载植物数据
        }
    }

    /// <summary>
    /// 从植物数据包装类中加载植物数据
    /// </summary>
    /// <param name="plantsDataWrapper">植物数据包装类</param>
    private void LoadPlantFromData(PlantDataWrapper plantsDataWrapper)
    {
        List<PlantSaveData> plantSaveDataList = new List<PlantSaveData>(); // 创建植物存档数据列表
        plantSaveDataList.Clear(); // 清空列表

        foreach (var plantsData in plantsDataWrapper.plants) // 遍历植物数据包装类中的所有植物数据
        {
            int plantKey = Animator.StringToHash(plantsData.plantName); // 将植物名称转换为哈希值作为键
            allPlant.TryGetValue(plantKey, out PlantData plantData); // 从字典中获取植物数据
            plantsData.plantData = plantData; // 将植物数据赋值给植物存档数据
        }
        plantSaveDataList = plantsDataWrapper.plants; // 将植物数据包装类中的植物存档数据列表赋值给新列表

        GameManager.instance.plantGrowthManager.SetTilePlantSaveData(plantSaveDataList); // 设置植物生长管理器的植物存档数据
    }

    /// <summary>
    /// 从库存数据中加载库存
    /// </summary>
    /// <param name="inventory">要加载的库存</param>
    /// <param name="inventoryData">库存数据</param>
    private void LoadInventoryFromData(Inventory inventory, InventoryData inventoryData)
    {
        if (inventory != null) // 如果库存不为空
        {
            inventory.Clear(); // 清空库存
            foreach (var slotsData in inventoryData.slots) // 遍历库存数据中的所有槽位数据
            {
                int key = Animator.StringToHash(slotsData.itemName); // 将物品名称转换为哈希值作为键
                if (allItem.TryGetValue(key, out var itemData)) // 如果字典中包含该键
                {
                    PlantData plantData = null; // 初始化植物数据

                    if (!string.IsNullOrEmpty(slotsData.plantName)) // 如果植物名称不为空
                    {
                        int plantKey = Animator.StringToHash(slotsData.plantName); // 将植物名称转换为哈希值作为键
                        allPlant.TryGetValue(plantKey, out plantData); // 从字典中获取植物数据
                    }

                    for (int i = 0; i < slotsData.currentCount; i++) // 循环添加物品到库存
                    {
                        Item item = new GameObject("Item").AddComponent<Item>(); // 创建物品对象
                        item.itemData = itemData; // 设置物品数据
                        item.plantData = plantData; // 设置植物数据
                        inventory.Add(item); // 将物品添加到库存中
                    }
                }
            }
        }
    }

    /// <summary>
    /// 检查是否存在保存的数据
    /// </summary>
    /// <returns>如果存在保存的数据返回true，否则返回false</returns>
    public bool HasSavedData()
    {
        string saveDataPath = Application.persistentDataPath + "/GameData.json"; // 保存数据的路径

        return File.Exists(saveDataPath); // 检查文件是否存在
    }
}
