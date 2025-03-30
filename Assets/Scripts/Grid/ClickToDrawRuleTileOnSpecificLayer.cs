using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ClickToDrawRuleTileOnSpecificLayer : MonoBehaviour
{
    [Header("Tilemap References")]
    public Tilemap targetTilemap;  // 要绘制RuleTile的目标Tilemap层

    [Header("Tile Assets")]
    public RuleTile ruleTile;      // 要绘制的RuleTile资源

    void Update()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            // 获取鼠标点击的世界坐标
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldMousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(mousePosition);
            worldMousePosition.z = 0;

            // 将世界坐标转换为Tilemap的单元格坐标
            Vector3Int targetPosition = targetTilemap.WorldToCell(worldMousePosition);

            // 在指定位置绘制或移除RuleTile
            if (!targetTilemap.HasTile(targetPosition))
            {
                targetTilemap.SetTile(targetPosition, ruleTile);
            }
        }
    }
}