using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ClickToDrawRuleTile : MonoBehaviour
{
    [Header("Tilemap References")]
    public Tilemap tilemap;       // 引用水渠Tilemap组件
    public Tilemap riverTilemap;  // 引用河流Tilemap组件

    [Header("Tile Assets")]
    public RuleTile ruleTile;     // 引用RuleTile资源（水渠瓦片）
    public TileBase wateredTile;  // 带水的水渠瓦片
    public TileBase dryTile;      // 干涸的水渠瓦片

    private Vector3Int[] directions = {
        new Vector3Int(0, 1, 0),   // 上
        new Vector3Int(1, 0, 0),   // 右
        new Vector3Int(0, -1, 0),  // 下
        new Vector3Int(-1, 0, 0)   // 左
    };

    void Update()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            // 获取鼠标点击的世界坐标
            Vector3 mousePosition = Input.mousePosition;
            // Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3 worldMousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(mousePosition);
            worldMousePosition.z = 0;

            // 将世界坐标转换为Tilemap的单元格坐标
            Vector3Int targetPosition = tilemap.WorldToCell(worldMousePosition);

            // 在指定位置绘制RuleTile（水渠瓦片）
            if (!tilemap.HasTile(targetPosition))
            {
                tilemap.SetTile(targetPosition, ruleTile);
            }
            else
            {
                tilemap.SetTile(targetPosition, null); // 如果已经存在瓦片，则清除
            }

            // 更新水渠连通性
            UpdateWaterFlow();
        }
    }

    // 更新水渠连通性
    private void UpdateWaterFlow()
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
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos)) continue;

            if (wateredPositions.Contains(pos))
            {
                tilemap.SetTile(pos, wateredTile);
            }
            else
            {
                tilemap.SetTile(pos, dryTile);
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
        return tilemap.HasTile(pos);
    }
}