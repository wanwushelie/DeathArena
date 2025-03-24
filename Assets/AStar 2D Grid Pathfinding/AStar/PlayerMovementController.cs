using UnityEngine;
using AStar;
using demo2;
using System.Threading.Tasks;
using System.Collections;

public class PlayerMovementController : MonoBehaviour
{
    private Vector3 targetPosition;
    private (int, int)[] path;
    private int currentPathIndex = 0;
    public float moveSpeed = 5f;
    private GridManager gridManager;

    void Start()
    {
        gridManager = GridManager.Instance;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 获取鼠标点击的世界坐标
            Vector3 mousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // 确保在2D平面内

            // 获取网格坐标
            GridCell gridCell = gridManager.GetGridCell(mousePosition);
            if (gridCell == null)
            {
                Debug.LogError("Clicked position is out of grid bounds.");
                return;
            }
            (int, int) gridPosition = (gridCell.x, gridCell.y);

            // 计算路径
            StartCoroutine(CalculatePathAsync(gridPosition.Item1, gridPosition.Item2));
        }

        // 如果有路径且未到达目标位置，则移动角色
        if (path != null && currentPathIndex < path.Length)
        {
            Vector3 nextPosition = gridManager.GetGridCell(path[currentPathIndex].Item1, path[currentPathIndex].Item2).getTileObject().transform.position;
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);

            // 检查是否到达下一个路径点
            if (Vector3.Distance(transform.position, nextPosition) < 0.1f)
            {
                currentPathIndex++;
            }
        }
    }

    private IEnumerator CalculatePathAsync(int targetX, int targetY)
    {
        // 获取当前角色位置的网格坐标
        GridCell gridCell = gridManager.GetGridCell(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        if (gridCell == null)
        {
            Debug.LogError("Player position is out of grid bounds.");
            yield break;
        }
        (int, int) currentGridPosition = (gridCell.x, gridCell.y);

        // 从 GridManager 中提取网格数据
        bool[,] walkableMap = new bool[gridManager.Height, gridManager.Width];
        for (int y = 0; y < gridManager.Height; y++)
        {
            for (int x = 0; x < gridManager.Width; x++)
            {
                GridCell cell = gridManager.GetGridCell(x, y);
                walkableMap[y, x] = cell.getWalkable();
            }
        }

        // 使用 AStarPathfinding 生成路径
        path = null;
        yield return new WaitUntil(() => path != null);
        path = AStarPathfinding.GeneratePath(currentGridPosition.Item1, currentGridPosition.Item2, targetX, targetY, walkableMap).Result;

        // 重置路径索引
        currentPathIndex = 0;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 处理与2D精灵的碰撞
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // 如果碰到障碍物，重新计算路径
            Vector3 mousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            GridCell gridCell = gridManager.GetGridCell(mousePosition);
            if (gridCell == null)
            {
                Debug.LogError("Clicked position is out of grid bounds.");
                return;
            }
            (int, int) gridPosition = (gridCell.x, gridCell.y);
            StartCoroutine(CalculatePathAsync(gridPosition.Item1, gridPosition.Item2));
        }
    }
}