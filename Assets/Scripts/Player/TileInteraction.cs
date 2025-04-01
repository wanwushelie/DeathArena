using UnityEngine; // 引入UnityEngine命名空间，包含Unity游戏开发的核心功能
using UnityEngine.Tilemaps; // 引入UnityEngine.Tilemaps命名空间，用于处理瓦片地图
using System.Collections; // 引入System.Collections命名空间，包含常用的集合类

public class TileInteraction : MonoBehaviour
{
    private Player player; // 玩家对象引用
    private TileManager tileManager; // 瓦片管理器对象引用
    private Vector3Int targetPosition; // 目标瓦片的位置
    private Vector3Int[] validTiles; // 有效瓦片位置数组
    private bool isValidTile = false; // 标记当前鼠标位置是否为有效瓦片
    private StateManager stateManager; // 状态管理器对象引用
    [Header("栅栏工具设置")]
    public Tilemap targetTilemap;  // 要绘制RuleTile的目标Tilemap层
    public RuleTile ruleTile;      // 要绘制的RuleTile资源
    [Header("水渠工具设置")]
    public ClickToDrawRuleTile waterFlowManager; // 新增对水流管理脚本的引用
    public Tilemap canalTilemap;  // 水渠Tilemap层
    public RuleTile wateredTile; // 水渠RuleTile
    public TileBase dryTile; // 干涸的水渠瓦片

    /// <summary>
    /// 初始化TileInteraction类的实例。
    /// </summary>
    /// <param name="playerRef">玩家对象引用。</param>
    /// <param name="tileManagerRef">瓦片管理器对象引用。</param>
    /// <param name="stateManagerRef">状态管理器对象引用。</param>
    public void Initialize(Player playerRef, TileManager tileManagerRef, StateManager stateManagerRef)
    {
        player = playerRef; // 初始化玩家对象引用
        tileManager = tileManagerRef; // 初始化瓦片管理器对象引用
        stateManager = stateManagerRef; // 初始化状态管理器对象引用
    }

    /// <summary>
    /// 更新瓦片交互逻辑。
    /// </summary>
    public void UpdateTileInteraction()
    {
        if (!stateManager.CanMove() || !stateManager.CanInteract()) // 检查玩家是否可以移动和交互
            return; // 如果不满足条件，直接返回

        if (tileManager == null) // 检查瓦片管理器是否为空
            return; // 如果为空，直接返回

        if (player.inventoryManager == null || player.inventoryManager.toolbar == null || player.inventoryManager.toolbar.selectedSlot == null) // 检查玩家库存管理器、工具栏和选中的物品槽是否为空
            return; // 如果为空，直接返回

        if (player.inventoryManager.toolbar.selectedSlot.itemName == null) // 检查选中物品槽中的物品名称是否为空
            return; // 如果为空，直接返回

        CheckValidTiles(); // 检查当前鼠标位置是否为有效瓦片

        if (isValidTile) // 如果是有效瓦片
        {
            MouseSelect mouseSelect = player.GetComponentInChildren<MouseSelect>(); // 获取玩家子对象中的MouseSelect组件
            if (mouseSelect != null) // 检查MouseSelect组件是否存在
            {
                mouseSelect.SetTargetPosition(targetPosition); // 设置MouseSelect组件的目标位置
            }

            if (Input.GetMouseButtonDown(0)) // 检查是否按下鼠标左键
            {
                // 添加栅栏工具处理
                if (player.inventoryManager.toolbar.selectedSlot.itemName == "栅栏")
                {
                    if (!targetTilemap.HasTile(targetPosition))
                    {
                        targetTilemap.SetTile(targetPosition, ruleTile);
                    }
                    return;
                }

                // 新增水渠工具处理
                if (player.inventoryManager.toolbar.selectedSlot.itemName == "铲子")
                {
                    if (!canalTilemap.HasTile(targetPosition))
                    {
                        canalTilemap.SetTile(targetPosition, dryTile);
                        player.anim.SetTrigger("isHoeing"); // 使用耕地动画
                        SoundManager.Instance.Play("EFFECT/Plow", SoundType.EFFECT);
                        waterFlowManager.UpdateWaterFlow(); // 新增水流更新
                    }
                    return;
                }

                // 新增泥土工具处理
                if (player.inventoryManager.toolbar.selectedSlot.itemName == "泥土")
                {
                    if (canalTilemap.GetTile(targetPosition) == wateredTile)
                    {
                        canalTilemap.SetTile(targetPosition, dryTile);
                        player.anim.SetTrigger("isPicking"); // 使用浇水动画
                        SoundManager.Instance.Play("EFFECT/Watering", SoundType.EFFECT);
                        waterFlowManager.UpdateWaterFlow(); // 新增水流更新
                    }
                    return;
                }
                SetAnimationDirection(targetPosition); // 设置玩家动画方向

                string tileName = tileManager.GetTileName(targetPosition); // 获取目标瓦片的名称
                string tileState = tileManager.GetTileState(targetPosition); // 获取目标瓦片的状态

                if (tileName != null) // 检查瓦片名称是否为空
                {
                    HandleHoeing(tileName, tileState); // 处理耕地操作
                    HandlePlanting(tileName); // 处理种植操作
                    HandleWatering(tileName); // 处理浇水操作
                    RemoveFence(tileName); // 处理移除栅栏操作
                }
            }
        }
    }

    /// <summary>
    /// 检查当前鼠标位置是否为有效瓦片。
    /// </summary>
    private void CheckValidTiles()
    {
        Vector3 mousePosition = Input.mousePosition; // 获取鼠标在屏幕上的位置
        Vector3 worldMousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(mousePosition); // 将屏幕坐标转换为世界坐标
        worldMousePosition.z = 0; // 将Z坐标设置为0

        targetPosition = new Vector3Int(Mathf.FloorToInt(worldMousePosition.x), Mathf.FloorToInt(worldMousePosition.y), 0); // 将世界坐标转换为整数坐标

        Vector3 playerPosition = player.transform.position; // 获取玩家的位置
        Vector3Int gridPlayerPosition = new Vector3Int(Mathf.FloorToInt(playerPosition.x), Mathf.FloorToInt(playerPosition.y), 0); // 将玩家位置转换为整数坐标

        validTiles = new Vector3Int[]
        {
            gridPlayerPosition, // 玩家当前位置
            gridPlayerPosition + new Vector3Int(0, 1, 0), // 玩家上方位置
            gridPlayerPosition + new Vector3Int(0, -1, 0), // 玩家下方位置
            gridPlayerPosition + new Vector3Int(1, 0, 0), // 玩家右方位置
            gridPlayerPosition + new Vector3Int(-1, 0, 0), // 玩家左方位置
        };

        isValidTile = false; // 初始化有效瓦片标记为false

        foreach (var tile in validTiles) // 遍历有效瓦片数组
        {
            if (tile == targetPosition) // 检查当前鼠标位置是否为有效瓦片
            {
                isValidTile = true; // 如果是，将有效瓦片标记设置为true
                break; // 跳出循环
            }
        }
    }

    /// <summary>
    /// 设置玩家动画方向。
    /// </summary>
    /// <param name="targetPosition">目标位置。</param>
    private void SetAnimationDirection(Vector3 targetPosition)
    {
        Vector3 playerPosition = player.transform.position; // 获取玩家的位置
        Vector3Int gridPlayerPosition = new Vector3Int(Mathf.FloorToInt(playerPosition.x), Mathf.FloorToInt(playerPosition.y), 0); // 将玩家位置转换为整数坐标

        Vector3 direction = (targetPosition - gridPlayerPosition).normalized; // 计算玩家到目标位置的方向向量

        player.anim.SetFloat("Horizontal", direction.x); // 设置玩家动画的水平方向
        player.anim.SetFloat("Vertical", direction.y); // 设置玩家动画的垂直方向

        stateManager.LastMoveDirection = direction; // 更新状态管理器中的最后移动方向
    }

    /// <summary>
    /// 处理耕地操作。
    /// </summary>
    /// <param name="tileName">瓦片名称。</param>
    /// <param name="tileState">瓦片状态。</param>
    private void HandleHoeing(string tileName, string tileState)
    {
        if (player.inventoryManager.toolbar.selectedSlot.itemName == "锄头") // 检查玩家选中的物品是否为锄头
        {
            if (tileName == "InteractableTile") // 检查目标瓦片是否为可交互瓦片
            {
                stateManager.IsHoeing = true; // 设置耕地状态为true
                player.anim.SetTrigger("isHoeing"); // 触发玩家的耕地动画
                SoundManager.Instance.Play("EFFECT/Plow", SoundType.EFFECT); // 播放耕地音效
                tileManager.SetInteracted(targetPosition); // 设置目标瓦片为已交互状态
                StartCoroutine(ResetHoeingState()); // 启动协程，在0.2秒后重置耕地状态
            }

            if (tileState == "Grown") // 检查目标瓦片是否为已生长状态
            {
                tileManager.RemoveTile(targetPosition); // 移除目标瓦片
                player.anim.SetTrigger("isHavesting"); // 触发玩家的收获动画
                SoundManager.Instance.Play("EFFECT/Plow", SoundType.EFFECT); // 播放收获音效
                GameManager.instance.plantGrowthManager.HarvestPlant(targetPosition); // 收获目标位置的植物
            }
        }
    }

    /// <summary>
    /// 处理种植操作。
    /// </summary>
    /// <param name="tileName">瓦片名称。</param>
    private void HandlePlanting(string tileName)
    {
        if (tileName == "PlowedTile") // 检查目标瓦片是否为已耕地瓦片
        {
            if (player.inventoryManager.toolbar.selectedSlot.itemName == "RiceSeed" || player.inventoryManager.toolbar.selectedSlot.itemName == "TomatoSeed") // 检查玩家选中的物品是否为种子
            {
                Sowing(); // 执行播种操作
            }
        }
    }

    /// <summary>
    /// 清除当前位置的栅栏
    /// </summary>
    private void RemoveFence(string tileName)
    {
        if (tileName != "栅栏") // 检查目标瓦片是否为栅栏瓦片
        {
            if (player.inventoryManager.toolbar.selectedSlot.itemName == "斧头")
            {
                if (targetTilemap.HasTile(targetPosition))
                {
                    targetTilemap.SetTile(targetPosition, null); // 清除栅栏
                    // SoundManager.Instance.Play("EFFECT/Watering", SoundType.EFFECT, 1, 1); // 播放浇水音效
                }
            }
        }

    }

    /// <summary>
    /// 处理浇水操作。
    /// </summary>
    /// <param name="tileName">瓦片名称。</param>
    private void HandleWatering(string tileName)
    {
        if (tileName == "土地") // 检查目标瓦片是否为已耕地瓦片
        {
            if (player.inventoryManager.toolbar.selectedSlot.itemName == "水壶") // 检查玩家选中的物品是否为浇水工具
            {
                stateManager.IsWatering = true; // 设置浇水状态为true
                player.anim.SetTrigger("isWatering"); // 触发玩家的浇水动画
                SoundManager.Instance.Play("EFFECT/Watering", SoundType.EFFECT, 1, 1); // 播放浇水音效
                tileManager.WaterTile(targetPosition); // 给目标瓦片浇水
                StartCoroutine(ResetWateringState()); // 启动协程，在0.2秒后重置浇水状态
            }
        }
    }

    /// <summary>
    /// 执行播种操作。
    /// </summary>
    private void Sowing()
    {
        PlantData plantData = player.inventoryManager.toolbar.selectedSlot.plantData; // 获取选中物品槽中的植物数据
        player.inventoryManager.toolbar.selectedSlot.RemoveItem(); // 移除选中物品槽中的物品

        if (player.inventoryManager.toolbar.selectedSlot.isEmpty) // 检查选中物品槽是否为空
        {
            player.inventoryManager.toolbar.selectedSlot = null; // 如果为空，将选中物品槽设置为null
        }

        GameManager.instance.plantGrowthManager.PlantSeed(targetPosition, plantData); // 在目标位置种植种子
    }

    /// <summary>
    /// 重置耕地状态协程。
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetHoeingState()
    {
        yield return new WaitForSeconds(0.2f); // 等待0.2秒
        stateManager.IsHoeing = false; // 重置耕地状态为false
    }

    /// <summary>
    /// 重置浇水状态协程。
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetWateringState()
    {
        yield return new WaitForSeconds(0.2f); // 等待0.2秒
        stateManager.IsWatering = false; // 重置浇水状态为false
    }
}