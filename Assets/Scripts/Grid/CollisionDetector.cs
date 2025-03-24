using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    [Header("Base Settings")]
    public Vector2 detectionAreaCenter = Vector2.zero;
    public Vector2 detectionAreaSize = new Vector2(20, 20);
    
    [Header("Precision Settings")]
    [Tooltip("必须与Tilemap单元格大小完全一致")]
    public float baseGridSize = 1f;
    [Range(1, 4)] public int subdivisions = 1;

    [Header("Runtime Data")]
    public bool[,] collisionGrid;
    public Vector2 gridWorldOrigin;
    public float actualCellSize { get; private set; }

    void Start()
    {
        ValidateSettings();
        InitializeCollisionGrid();
    }

    void ValidateSettings()
    {
        // 确保细分级别是2的幂次
        subdivisions = Mathf.ClosestPowerOfTwo(subdivisions);
        subdivisions = Mathf.Clamp(subdivisions, 1, 4);
    }

    void InitializeCollisionGrid()
    {
        actualCellSize = baseGridSize / subdivisions;
        
        // 严格对齐的网格计算
        Vector2Int gridDimensions = new Vector2Int(
            Mathf.FloorToInt(detectionAreaSize.x / actualCellSize),
            Mathf.FloorToInt(detectionAreaSize.y / actualCellSize)
        );

        // 重新计算实际检测区域确保对齐
        Vector2 alignedSize = new Vector2(
            gridDimensions.x * actualCellSize,
            gridDimensions.y * actualCellSize
        );

        collisionGrid = new bool[gridDimensions.x, gridDimensions.y];
        gridWorldOrigin = detectionAreaCenter - alignedSize / 2;

        // 精确的单元格检测
        for (int x = 0; x < gridDimensions.x; x++)
        {
            for (int y = 0; y < gridDimensions.y; y++)
            {
                Vector2 cellCenter = gridWorldOrigin + 
                    new Vector2(
                        x * actualCellSize + actualCellSize/2, 
                        y * actualCellSize + actualCellSize/2
                    );
                
                Collider2D hit = Physics2D.OverlapBox(
                    cellCenter, 
                    Vector2.one * actualCellSize * 0.98f, // 减少间隙
                    0, 
                    LayerMask.GetMask("Obstacles") // 根据实际层修改
                );
                
                collisionGrid[x, y] = (hit != null);
            }
        }
    }

    public Vector2Int WorldToGridPosition(Vector2 worldPosition)
    {
        Vector2 localPos = worldPosition - gridWorldOrigin;
        return new Vector2Int(
            Mathf.FloorToInt(localPos.x / actualCellSize),
            Mathf.FloorToInt(localPos.y / actualCellSize)
        );
    }
}