using UnityEngine; // 引入UnityEngine命名空间，包含Unity游戏开发的核心功能
using UnityEngine.SceneManagement; // 引入Unity场景管理命名空间，用于场景的加载和管理

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 静态单例实例，确保整个游戏中只有一个GameManager实例

    public ItemManager itemManager; // 物品管理类的引用
    public TimeManager timeManager; // 时间管理类的引用
    public TileManager tileManager; // 瓦片管理类的引用
    public PlantGrowthManager plantGrowthManager; // 植物生长管理类的引用
    public ItemSellingBox itemBox; // 物品售卖箱类的引用
    public SaveData inventorySave; // 保存数据类的引用

    void Awake()
    {
        if (!instance) // 如果单例实例还未被初始化
        {
            instance = this; // 将当前实例赋值给单例实例
        }
        else
        {
            Destroy(gameObject); // 如果已经存在实例，则销毁当前游戏对象
        }

        DontDestroyOnLoad(this.gameObject); // 确保在场景切换时不销毁当前游戏对象

        itemManager = GetComponent<ItemManager>(); // 获取当前游戏对象上的ItemManager组件
        timeManager = GetComponent<TimeManager>(); // 获取当前游戏对象上的TimeManager组件
        inventorySave = GetComponent<SaveData>(); // 获取当前游戏对象上的SaveData组件
        plantGrowthManager = GetComponent<PlantGrowthManager>(); // 获取当前游戏对象上的PlantGrowthManager组件
        tileManager = GetComponent<TileManager>(); // 获取当前游戏对象上的TileManager组件
        itemBox = FindObjectOfType<ItemSellingBox>(); // 查找场景中唯一的ItemSellingBox组件

        SceneManager.sceneLoaded += OnSceneLoaded; // 订阅场景加载完成事件
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "InGameScene") // 如果加载的场景是游戏内场景
        {
            // 确保SoundManager已初始化
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.FadeIn(1f, "BGM/InGame"); // 在1秒内淡入游戏内背景音乐
            }

            if (OutGameUI.instance.isNewGame) // 如果是新游戏
            {
                Player.Instance.inventoryManager.ClearInventory(); // 清空玩家的物品栏
                plantGrowthManager.ClearPlantSaveData(); // 清空植物的保存数据
                SaveData.instance.DeleteSavedFiles(); // 删除保存的游戏文件
                Player.Instance.money = 0; // 重置玩家的金钱为0
                timeManager.day = 1; // 重置游戏天数为1
                timeManager.currentDayIndex = 0; // 重置当前天数索引为0
            }
            else
            {
                SaveData.instance.LoadGameData(); // 加载已保存的游戏数据
                // plantGrowthManager.LoadPlantsData(); // 注释掉的代码，用于加载植物数据
            }
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 取消订阅场景加载完成事件
    }
}
