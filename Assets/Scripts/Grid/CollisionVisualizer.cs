using UnityEngine;

public class CollisionVisualizer : MonoBehaviour
{
    public CollisionDetector detector;
    public bool showGridInfo = true;
    
    void OnDrawGizmosSelected()
    {
        if (!ShouldDraw()) return;

        DrawBaseGrid();
        DrawCollisionGrid();
    }

    bool ShouldDraw()
    {
        return Application.isPlaying && 
               detector != null && 
               detector.collisionGrid != null &&
               detector.collisionGrid.GetLength(0) > 0 &&
               detector.collisionGrid.GetLength(1) > 0;
    }

    void DrawBaseGrid()
    {
        Gizmos.color = Color.green;
        Vector2 origin = detector.gridWorldOrigin;
        float baseSize = detector.baseGridSize;

        int cols = Mathf.FloorToInt(detector.detectionAreaSize.x / baseSize);
        int rows = Mathf.FloorToInt(detector.detectionAreaSize.y / baseSize);

        // 垂直网格线
        for (int x = 0; x <= cols; x++)
        {
            Vector2 start = origin + new Vector2(x * baseSize, 0);
            Vector2 end = start + new Vector2(0, rows * baseSize);
            Gizmos.DrawLine(start, end);
        }

        // 水平网格线
        for (int y = 0; y <= rows; y++)
        {
            Vector2 start = origin + new Vector2(0, y * baseSize);
            Vector2 end = start + new Vector2(cols * baseSize, 0);
            Gizmos.DrawLine(start, end);
        }
    }

    void DrawCollisionGrid()
    {
        float cellSize = detector.actualCellSize;
        Vector2 origin = detector.gridWorldOrigin;
        int gridWidth = detector.collisionGrid.GetLength(0);
        int gridHeight = detector.collisionGrid.GetLength(1);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2 center = origin + new Vector2(
                    x * cellSize + cellSize/2,
                    y * cellSize + cellSize/2
                );

                // 增强颜色对比度
                Gizmos.color = detector.collisionGrid[x, y] 
                    ? new Color(1, 0, 0, 0.8f)  // 红色更明显
                    : new Color(0, 0, 1, 0.5f);  // 蓝色半透明
                
                Gizmos.DrawCube(center, Vector3.one * cellSize * 0.98f);
            }
        }
    }

    void OnGUI()
    {
        if (showGridInfo && Application.isPlaying)
        {
            GUI.Label(new Rect(10, 10, 300, 20), 
                $"Base Grid: {detector.baseGridSize}  Sub: x{detector.subdivisions}");
            GUI.Label(new Rect(10, 30, 300, 20), 
                $"Actual Cell: {detector.actualCellSize:F2}");
            GUI.Label(new Rect(10, 50, 300, 20), 
                $"Grid Size: {detector.collisionGrid.GetLength(0)}x{detector.collisionGrid.GetLength(1)}");
        }
    }
}