using System.Collections;  // 引入 System.Collections 命名空间，提供对集合类的支持
using System.Collections.Generic;  // 引入 System.Collections.Generic 命名空间，提供泛型集合类的支持
using UnityEditor;  // 引入 UnityEditor 命名空间，用于编辑器相关的功能
using UnityEngine;  // 引入 UnityEngine 命名空间，Unity 游戏开发的核心命名空间

namespace demo1
{
    public class DemoGrid : MonoBehaviour
    {
        static private DemoGrid instance;  // 静态私有变量，用于存储单例实例

        public static DemoGrid Instance { get => instance == null ? makeSingleton() : instance; }  // 公共静态属性，用于获取单例实例

        private void Awake()
        {
            instance = this;  // 在 Awake 方法中初始化单例实例
            GenerateGrid();  // 调用 GenerateGrid 方法生成网格
        }

        static private DemoGrid makeSingleton()
        {
            instance = new DemoGrid();  // 创建新的 DemoGrid 实例

            return instance;  // 返回新创建的实例
        }

        public bool[,] walkableMap;  // 二维布尔数组，用于存储网格中每个单元格是否可通行
        [SerializeField] private float gridWidth;  // 可序列化的私有变量，用于存储网格的宽度
        [SerializeField] private float gridHeight;  // 可序列化的私有变量，用于存储网格的高度
        [SerializeField] private float cellSize;  // 可序列化的私有变量，用于存储每个单元格的大小
        [SerializeField] private GameObject tilePrefab;  // 可序列化的私有变量，用于存储瓷砖预制体
        [SerializeField][Range(0f, 1f)] private float nonWalable;  // 可序列化的私有变量，范围在 0 到 1 之间，用于存储不可通行的阈值
        [SerializeField] float mapScale;  // 可序列化的私有变量，用于存储地图的缩放比例

        private Vector2 cellSpacing = Vector2.zero;  // 私有变量，用于存储单元格之间的间距
        private Vector2 topLeft = Vector2.zero;  // 私有变量，用于存储网格的左上角位置
        private Texture2D mapSprite;  // 私有变量，用于存储地图的纹理

        private void destoyOldGrid()
        {
            for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);  // 销毁当前游戏对象的所有子对象
            }
        }

        public void createGrid()
        {
            destoyOldGrid();  // 调用 destoyOldGrid 方法销毁旧的网格

            topLeft = new Vector2(gameObject.transform.position.x - gridWidth / 2, gameObject.transform.position.y + gridHeight / 2);  // 计算网格的左上角位置

            cellSpacing = new Vector2(cellSize, cellSize);  // 计算单元格之间的间距

            int cellAmountX = (int)((gridWidth + 0.001f) / cellSpacing.x);  // 计算网格在 X 轴上的单元格数量
            int cellAmountY = (int)((gridHeight + 0.001f) / cellSpacing.y);  // 计算网格在 Y 轴上的单元格数量
            walkableMap = new bool[cellAmountY, cellAmountX];  // 初始化可通行地图数组

            demo2.NoiseGenerator noiseGenerator = new demo2.NoiseGenerator(true, 1 / mapScale);  // 创建一个 NoiseGenerator 实例

            for (int y = 0; y < walkableMap.GetLength(0); y++)
            {
                float gridObjectYpos = topLeft.y - cellSpacing.y * y - cellSpacing.y / 2;  // 计算当前单元格在 Y 轴上的位置

                for (int x = 0; x < walkableMap.GetLength(1); x++)
                {
                    float gridObjectXPos = topLeft.x + cellSpacing.x * x + cellSpacing.x / 2;  // 计算当前单元格在 X 轴上的位置

                    bool walkable = noiseGenerator.GetPerlinNoise(gridObjectXPos, gridObjectYpos) > nonWalable ? true : false;  // 根据噪声值判断当前单元格是否可通行
                    walkableMap[y, x] = walkable;  // 将判断结果存储到可通行地图数组中
                }
            }
        }

        private void colorTile(int tileIndexX, int tileIndexY, Color color)
        {

            for (int y = 0; y < 100 * cellSize; y++)
            {
                for (int x = 0; x < 100 * cellSize; x++)
                {
                    mapSprite.SetPixel(tileIndexX * Mathf.RoundToInt(cellSize * 100) + x, mapSprite.height - tileIndexY * Mathf.RoundToInt(cellSize * 100) - y, color);  // 设置地图纹理中指定像素的颜色
                }
            }
        }

        private void generateMap()
        {
            int cellAmountX = (int)((gridWidth + 0.001f) / cellSpacing.x);  // 计算网格在 X 轴上的单元格数量
            int cellAmountY = (int)((gridHeight + 0.001f) / cellSpacing.y);  // 计算网格在 Y 轴上的单元格数量

            mapSprite = new Texture2D((int)(gridWidth * 100), (int)(gridHeight * 100));  // 创建一个新的纹理对象

            for (int y = 0; y < walkableMap.GetLength(0); y++)
            {
                float gridObjectYpos = topLeft.y - cellSpacing.y * y - cellSpacing.y / 2;  // 计算当前单元格在 Y 轴上的位置

                for (int x = 0; x < walkableMap.GetLength(1); x++)
                {
                    Color tileColor = walkableMap[y, x] ? Color.white : Color.black;  // 根据可通行地图数组中的值设置当前单元格的颜色

                    colorTile(x, y, tileColor);  // 调用 colorTile 方法设置地图纹理中指定单元格的颜色
                }
            }

            mapSprite.Apply();  // 应用纹理的更改

            var sprite = Sprite.Create(mapSprite, new Rect(0, 0, gridWidth * 100, gridHeight * 100), new Vector2(0.5f, 0.5f));  // 创建一个新的精灵对象

            GetComponent<SpriteRenderer>().sprite = sprite;  // 将创建的精灵对象赋值给当前游戏对象的 SpriteRenderer 组件
        }

        public void GenerateGrid()
        {
            createGrid();  // 调用 createGrid 方法创建网格
            generateMap();  // 调用 generateMap 方法生成地图
        }

        public bool GetGridCell(int xCodrinate, int yCordinate)
        {
            return walkableMap[yCordinate, xCodrinate];  // 返回可通行地图数组中指定位置的单元格是否可通行
        }

        public bool GetGridCell(Vector3 worldPos)
        {
            cellSpacing = new Vector2(cellSize, cellSize);  // 计算单元格之间的间距

            int xCord = Mathf.RoundToInt((worldPos.x - topLeft.x) / cellSpacing.x);  // 计算世界坐标在网格中的 X 坐标
            int yCord = Mathf.RoundToInt((-worldPos.y + topLeft.y) / cellSpacing.y);  // 计算世界坐标在网格中的 Y 坐标

            return walkableMap[yCord, xCord];  // 返回可通行地图数组中指定位置的单元格是否可通行
        }

        public Vector3 cordinateToWorldSpace(int xCord, int yCord)
        {
            float worldPosX = topLeft.x + cellSpacing.x * xCord + cellSpacing.x / 2;  // 计算网格坐标对应的世界坐标的 X 坐标
            float worldPosY = topLeft.y - cellSpacing.y * yCord - cellSpacing.y / 2;  // 计算网格坐标对应的世界坐标的 Y 坐标

            return new Vector3(worldPosX, worldPosY, 0);  // 返回计算得到的世界坐标
        }

        public (int, int) worldSpaceToCordinate(Vector2 worldPos)
        {
            int xCord = Mathf.RoundToInt((worldPos.x - topLeft.x - cellSpacing.x / 2) / cellSpacing.x);  // 计算世界坐标对应的网格坐标的 X 坐标
            int yCord = Mathf.RoundToInt((worldPos.y - topLeft.y + cellSpacing.y / 2) / -cellSpacing.y);  // 计算世界坐标对应的网格坐标的 Y 坐标

            return (xCord, yCord);  // 返回计算得到的网格坐标
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;  // 设置 Gizmos 的颜色为蓝色
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWidth, gridHeight, 0));  // 在场景视图中绘制一个蓝色的线框立方体，表示网格的边界
        }
    }
}

#if (UNITY_EDITOR)
namespace demo1
{
    [CustomEditor(typeof(demo1.DemoGrid))]
    public class DemoGrid2Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();  // 调用基类的 OnInspectorGUI 方法
            demo1.DemoGrid DemoGrid2 = target as demo1.DemoGrid;  // 获取当前编辑的 DemoGrid 实例

            if (GUILayout.Button("Generate Grid"))  // 在 Inspector 面板中添加一个按钮
            {
                DemoGrid2.GenerateGrid();  // 当按钮被点击时，调用 DemoGrid 实例的 GenerateGrid 方法
            }
        }
    }
}
#endif