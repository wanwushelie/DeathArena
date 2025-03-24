using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[RequireComponent(typeof(Tilemap))]
public class CanalSystem : MonoBehaviour
{
    [Header("Tilemap References")]
    public Tilemap riverTilemap;
    public Tilemap canalTilemap;

    [Header("Tile Assets")]
    public TileBase wateredTile;
    public TileBase dryTile;

    private Vector3Int[] directions = {
        new Vector3Int(0, 1, 0),   // 上
        new Vector3Int(1, 0, 0),   // 右
        new Vector3Int(0, -1, 0),  // 下
        new Vector3Int(-1, 0, 0)   // 左
    };

    // 手动调用更新水流
    public void UpdateWaterFlow()
    {
        HashSet<Vector3Int> wateredPositions = GetConnectedWaterTiles();
        UpdateTilemap(wateredPositions);
    }

    // 获取所有带水瓦片位置
    private HashSet<Vector3Int> GetConnectedWaterTiles()
    {
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        Queue<Vector3Int> queue = new Queue<Vector3Int>();

        // 初始化队列：找到所有与河流相邻的水渠瓦片
        foreach (Vector3Int riverPos in GetAllRiverPositions())
        {
            foreach (Vector3Int dir in directions)
            {
                Vector3Int canalPos = riverPos + dir;
                if (IsValidCanalPosition(canalPos) && !visited.Contains(canalPos))
                {
                    visited.Add(canalPos);
                    queue.Enqueue(canalPos);
                }
            }
        }

        // BFS遍历
        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            
            foreach (Vector3Int dir in directions)
            {
                Vector3Int neighbor = current + dir;
                if (IsValidCanalPosition(neighbor) && !visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return visited;
    }

    // 更新Tilemap显示
    private void UpdateTilemap(HashSet<Vector3Int> wateredPositions)
    {
        foreach (Vector3Int pos in canalTilemap.cellBounds.allPositionsWithin)
        {
            if (!canalTilemap.HasTile(pos)) continue;

            if (wateredPositions.Contains(pos))
            {
                canalTilemap.SetTile(pos, wateredTile);
            }
            else
            {
                canalTilemap.SetTile(pos, dryTile);
            }
        }
    }

    // 获取所有河流位置
    private List<Vector3Int> GetAllRiverPositions()
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        foreach (Vector3Int pos in riverTilemap.cellBounds.allPositionsWithin)
        {
            if (riverTilemap.HasTile(pos))
            {
                positions.Add(pos);
            }
        }
        return positions;
    }

    // 验证有效水渠位置
    private bool IsValidCanalPosition(Vector3Int pos)
    {
        return canalTilemap.HasTile(pos);
    }
}