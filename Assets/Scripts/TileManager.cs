// 引入泛型集合命名空间，用于使用Dictionary等集合类型
using System.Collections.Generic;
// 引入Unity引擎核心命名空间，提供基础的游戏开发功能
using UnityEngine;
// 引入Unity引擎中Tilemap相关命名空间，用于处理地图瓦片
using UnityEngine.Tilemaps;

/// <summary>
/// TileManager类负责管理游戏中的瓦片地图，包括瓦片状态、交互和浇水等操作。
/// </summary>
public class TileManager : MonoBehaviour
{
    // 可交互的瓦片地图
    public Tilemap interactableMap;
    // 种子瓦片地图
    public Tilemap seedMap;
    // 隐藏的可交互瓦片
    public Tile hiddenInteractableTile;
    // 已交互的瓦片
    public TileBase interactedTile;
    // 已种植的瓦片
    public Tile plantedTile;

    // 存储每个瓦片位置及其状态的字典
    private Dictionary<Vector3Int, string> tileStates = new Dictionary<Vector3Int, string>();
    // 存储每个瓦片位置及其是否被浇水的字典
    private Dictionary<Vector3Int, bool> wateredTiles = new Dictionary<Vector3Int, bool>();

    /// <summary>
    /// 在游戏开始时调用，初始化可交互瓦片的显示。
    /// </summary>
    void Start()
    {
        // 遍历可交互地图的所有单元格位置
        foreach (var position in interactableMap.cellBounds.allPositionsWithin)
        {
            // 获取指定位置的瓦片
            TileBase tile = interactableMap.GetTile(position);
            // 检查瓦片是否存在且名称为"Interactable_Visible"
            if (tile != null && tile.name == "Interactable_Visible")
            {
                // 将可交互地图和种子地图中该位置的瓦片设置为隐藏的可交互瓦片
                interactableMap.SetTile(position, hiddenInteractableTile);
                // seedMap.SetTile(position, hiddenInteractableTile);
            }
        }
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return seedMap.WorldToCell(worldPosition);
    }

    /// <summary>
    /// 将指定位置的瓦片设置为已交互状态。
    /// </summary>
    /// <param name="position">瓦片的位置。</param>
    public void SetInteracted(Vector3Int position)
    {
        // 将指定位置的瓦片设置为已交互瓦片
        interactableMap.SetTile(position, interactedTile);
        // 记录该位置的瓦片状态为"Plowed"
        tileStates[position] = "Plowed";
    }

    /// <summary>
    /// 保存所有已播种的瓦片位置。
    /// </summary>
    /// <returns>已播种瓦片的位置列表。</returns>
    public List<Vector3Int> SaveSeededTiles()
    {
        // 用于存储已播种瓦片位置的列表
        List<Vector3Int> seededTilePositions = new List<Vector3Int>();

        // 遍历所有瓦片状态
        foreach (var tile in tileStates)
        {
            // 检查瓦片状态是否为"Seeded"
            if (tile.Value == "Seeded")
            {
                // 将已播种的瓦片位置添加到列表中
                seededTilePositions.Add(tile.Key);
            }
        }
        // 返回已播种瓦片的位置列表
        return seededTilePositions;
    }

    /// <summary>
    /// 获取指定位置瓦片的名称。
    /// </summary>
    /// <param name="position">瓦片的位置。</param>
    /// <returns>瓦片的名称，如果不存在则返回空字符串。</returns>
    public string GetTileName(Vector3Int position)
    {
        // 检查可交互地图是否存在
        if (interactableMap != null)
        {
            // 获取指定位置的瓦片
            TileBase tile = interactableMap.GetTile(position);
            // 检查瓦片是否存在
            if (tile != null)
                // 返回瓦片的名称
                return tile.name;
        }
        // 如果瓦片不存在，返回空字符串
        return "";
    }

    /// <summary>
    /// 获取指定位置瓦片的状态。
    /// </summary>
    /// <param name="position">瓦片的位置。</param>
    /// <returns>瓦片的状态，如果不存在则返回空字符串。</returns>
    public string GetTileState(Vector3Int position)
    {
        // 检查字典中是否包含该位置的瓦片状态
        if (tileStates.ContainsKey(position))
            // 返回该位置的瓦片状态
            return tileStates[position];

        // 如果不存在，返回空字符串
        return "";
    }

    /// <summary>
    /// 设置指定位置瓦片的状态。
    /// </summary>
    /// <param name="position">瓦片的位置。</param>
    /// <param name="state">瓦片的新状态。</param>
    public void SetTileState(Vector3Int position, string state)
    {
        // 检查字典中是否已经包含该位置的瓦片状态
        if (tileStates.ContainsKey(position))
        {
            // 如果包含，更新该位置的瓦片状态
            tileStates[position] = state;
        }
        else
        {
            // 如果不包含，添加该位置的瓦片状态
            tileStates.Add(position, state);
        }
    }

    /// <summary>
    /// 获取指定位置瓦片是否被浇水。
    /// </summary>
    /// <param name="position">瓦片的位置。</param>
    /// <returns>如果瓦片被浇水返回true，否则返回false。</returns>
    public bool GetWateringTile(Vector3Int position)
    {
        // 检查字典中是否包含该位置的瓦片浇水状态
        if (wateredTiles.ContainsKey(position))
            // 返回该位置的瓦片浇水状态
            return wateredTiles[position];

        // 如果不存在，返回false
        return false;
    }

    /// <summary>
    /// 设置指定位置瓦片的浇水状态。
    /// </summary>
    /// <param name="position">瓦片的位置。</param>
    /// <param name="iswatered">是否浇水的状态。</param>
    public void SetWateringTile(Vector3Int position, bool iswatered)
    {
        // 检查字典中是否已经包含该位置的瓦片浇水状态
        if (wateredTiles.ContainsKey(position))
            // 如果包含，更新该位置的瓦片浇水状态
            wateredTiles[position] = iswatered;
    }

    /// <summary>
    /// 检查指定位置的瓦片是否存在。
    /// </summary>
    /// <param name="position">瓦片的位置。</param>
    /// <returns>如果瓦片存在返回true，否则返回false。</returns>
    public bool DoesTileExist(Vector3Int position)
    {
        // 检查字典中是否包含该位置的瓦片状态
        return tileStates.ContainsKey(position);
    }

    /// <summary>
    /// 移除指定位置的瓦片并重置其状态。
    /// </summary>
    /// <param name="position">瓦片的位置。</param>
    public void RemoveTile(Vector3Int position)
    {
        // 将种子地图中该位置的瓦片设置为null
        seedMap.SetTile(position, null);
        // 将种子地图中该位置的瓦片设置为隐藏的可交互瓦片
        seedMap.SetTile(position, hiddenInteractableTile);
        // 记录该位置的瓦片状态为"Plowed"
        tileStates[position] = "Plowed";
    }

    /// <summary>
    /// 给指定位置的瓦片浇水。
    /// </summary>
    /// <param name="position">瓦片的位置。</param>
    public void WaterTile(Vector3Int position)
    {
        // 标记该位置的瓦片已浇水
        wateredTiles[position] = true;

        // 获取指定位置的瓦片
        TileBase tile = interactableMap.GetTile(position);
        // 检查瓦片是否存在
        if (tile != null)
        {
            // 将图块的基本属性标志设置为None
            interactableMap.SetTileFlags(position, TileFlags.None);
            // 将该位置的瓦片颜色设置为指定颜色
            interactableMap.SetColor(position, new Color(150f / 255f, 150f / 255f, 150f / 255f));
        }
    }

    /// <summary>
    /// 获取所有已浇水瓦片的位置。
    /// </summary>
    /// <returns>已浇水瓦片的位置列表。</returns>
    public List<Vector3Int> GetWateredTilesKeys()
    {
        // 返回已浇水瓦片的位置列表
        return new List<Vector3Int>(wateredTiles.Keys);
    }

    /// <summary>
    /// 检查并自动浇水周围5x5范围内的土地瓦片
    /// </summary>
    /// <param name="canalPosition">水渠的中心位置</param>
    [Header("水渠浇水设置")]
    [Tooltip("水渠浇水范围(单侧距离)")]
    public int canalWaterRange = 2; // 默认2格，即5x5范围
    
    public void AutoWaterAroundCanal(Vector3Int canalPosition)
    {
        for (int x = -canalWaterRange; x <= canalWaterRange; x++)
        {
            for (int y = -canalWaterRange; y <= canalWaterRange; y++)
            {
                Vector3Int tilePosition = new Vector3Int(
                    canalPosition.x + x,
                    canalPosition.y + y,
                    canalPosition.z
                );
                
                // 检查是否是有效土地瓦片且名称为"土地"
                if (DoesTileExist(tilePosition) &&
                    GetTileName(tilePosition) == "土地")
                {
                    WaterTile(tilePosition);
                }
            }
        }
    }
}