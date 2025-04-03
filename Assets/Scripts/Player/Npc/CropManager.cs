using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class CropManager : MonoBehaviour
{
    [SerializeField] private Tilemap cropTilemap; // 在Inspector中拖拽你的农作物Tilemap到这里

    // 获取所有已种植位置的单元格坐标
    public List<Vector3Int> GetPlantedCellPositions()
    {
        List<Vector3Int> plantedPositions = new List<Vector3Int>();
        BoundsInt bounds = cropTilemap.cellBounds;

        foreach (Vector3Int cellPosition in bounds.allPositionsWithin)
        {
            if (cropTilemap.HasTile(cellPosition))
            {
                plantedPositions.Add(cellPosition);
            }
        }

        return plantedPositions;
    }

    // // 获取所有已种植位置的世界坐标（二维）
    // public List<Vector2> GetPlantedWorldPositions()
    // {
    //     List<Vector2> worldPositions = new List<Vector2>();
    //     BoundsInt bounds = cropTilemap.cellBounds;

    //     foreach (Vector3Int cellPosition in bounds.allPositionsWithin)
    //     {
    //         if (cropTilemap.HasTile(cellPosition))
    //         {
    //             Vector3 worldPos = cropTilemap.CellToWorld(cellPosition);
    //             worldPositions.Add(new Vector2(worldPos.x, worldPos.y));
    //         }
    //     }

    //     return worldPositions;
    // }
    // 修改获取世界坐标的方法，添加中心偏移
    public List<Vector2> GetPlantedWorldPositions()
    {
        List<Vector2> worldPositions = new List<Vector2>();
        BoundsInt bounds = cropTilemap.cellBounds;
        
        // 获取单元格尺寸
        Vector3 cellSize = cropTilemap.cellSize;
        
        foreach (Vector3Int cellPosition in bounds.allPositionsWithin)
        {
            if (cropTilemap.HasTile(cellPosition))
            {
                // 转换为世界坐标后添加中心偏移
                Vector3 worldPos = cropTilemap.CellToWorld(cellPosition);
                worldPos += new Vector3(cellSize.x * 0.5f, cellSize.y * 0.5f, 0);
                worldPositions.Add(new Vector2(worldPos.x, worldPos.y));
            }
        }
        
        return worldPositions;
    }
}