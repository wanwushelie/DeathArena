using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Props : MonoBehaviour
{
    public GameObject[] propPrefabs;
    public GameObject[] grassPrefabs;
    public GameObject[] oneTilePlantsPrefabs; // 占一格的植物
    public GameObject[] twoTilePlantsPrefabs; // 占二格的植物
    public GameObject[] treePrefabs; // 树
    
    
    public GameObject[] oneTilePropPrefabs; // 占一格的物品，周围不长草
    public GameObject[] oneTilePropWithGrassPrefabs; // 占一格的物品，周围长草
    public GameObject[] twoTilePropPrefabs; // 占二格的物品，周围不长草
    public GameObject[] twoTilePropWithGrassPrefabs; // 占二格的物品，周围长草

    public GameObject[] sixTilePropPrefabs; // 占6格大块区域

    public GameObject[] chairPropPrefabs; // 椅子，比较特殊单拎出来
    
    public GameObject[] layerProps; // 第一层物体
    public GameObject[] grassLayers; // 草地
    
    
    /// <summary>
    /// 在指定位置实例化一个物体
    /// </summary>
    /// <param name="objNum"> 物体编号，也可使用TypeConstants类中定义的数字</param>
    /// <param name="x"> x坐标</param>
    /// <param name="y"> y坐标</param>
    public void GenerayePropAt(int objNum, float x, float y)
    {
        GameObject instance = Instantiate(propPrefabs[0], layerProps[0].transform, true);
        instance.transform.position = new Vector3(x, y, 0);
        instance.layer = layerProps[0].layer;
    }
    
    /// <summary>
    /// 在指定位置生成草
    /// </summary>
    /// <param name="x"> x坐标</param>
    /// <param name="y"> y坐标</param>
    /// <param name="layer"> 生成的层级</param>
    /// <param name="random"> 随机数</param>
    public void GenerateGrassAt(float x, float y, int layer, Random random)
    {
        if (grassPrefabs.Length == 0 || layer > grassLayers.Length)
        {
            return;
        }
        GameObject instance = Instantiate(grassPrefabs[random.Next(0, grassPrefabs.Length)], grassLayers[layer - 1].transform, true);
        instance.transform.position = new Vector3((float)(x + random.NextDouble()), (float)(y + random.NextDouble()), 0);
        instance.layer = grassLayers[layer - 1].layer;
        instance.GetComponent<SpriteRenderer>().sortingLayerName = LayerMask.LayerToName(grassLayers[layer - 1].layer);
    }
    
    /// <summary>
    /// 在指定位置生成单格植株
    /// </summary>
    /// <param name="x"> x坐标</param>
    /// <param name="y"> y坐标</param>
    /// <param name="layer"> 生成的层级</param>
    /// <param name="random"> 随机数</param>
    public void GenerateOneTilePlantAt(float x, float y, int layer, Random random)
    {
        if (oneTilePlantsPrefabs.Length == 0 || layer > layerProps.Length)
        {
            return;
        }
        GameObject instance = Instantiate(oneTilePlantsPrefabs[random.Next(0, oneTilePlantsPrefabs.Length)], layerProps[layer - 1].transform, true);
        instance.transform.position = new Vector3((float)(x + random.NextDouble()), (float)(y + random.NextDouble()), 0);
        instance.layer = layerProps[layer - 1].layer;
        instance.GetComponent<SpriteRenderer>().sortingLayerName = LayerMask.LayerToName(layerProps[layer - 1].layer);
    }
    
    /// <summary>
    /// 在指定位置生成1x1物品
    /// </summary>
    /// <param name="x"> x坐标</param>
    /// <param name="y"> y坐标</param>
    /// <param name="layer"> 生成的层级</param>
    /// <param name="random"> 随机数</param>
    /// <param name="withGrass"> 周围是否长草</param>
    public void Generate1X1TilePropAt(float x, float y, int layer, Random random, bool withGrass = false)
    {
        GameObject[] objects = withGrass ? oneTilePropWithGrassPrefabs : oneTilePropPrefabs;
        if (objects.Length == 0 || layer > layerProps.Length)
        {
            return;
        }
        GameObject instance = Instantiate(objects[random.Next(0, objects.Length)], layerProps[layer - 1].transform, true);
        instance.transform.position = new Vector3((float)(x + random.NextDouble()), (float)(y + random.NextDouble()), 0);
        instance.layer = layerProps[layer - 1].layer;
        instance.GetComponent<SpriteRenderer>().sortingLayerName = LayerMask.LayerToName(layerProps[layer - 1].layer);
        // 生点草
        if (withGrass && grassPrefabs.Length != 0)
        {
            GameObject grass = Instantiate(grassPrefabs[random.Next(0, grassPrefabs.Length)], grassLayers[layer - 1].transform, true);
            float xPos = (float)(1 - random.NextDouble() / 10) * ((random.Next() & 2) - 1);
            float yPos = (float)(1 - random.NextDouble() / 10) * ((random.Next() & 2) - 1);
            grass.transform.position = new Vector3(instance.transform.position.x + xPos, instance.transform.position.y + yPos, 0);
            grass.layer = grassLayers[layer - 1].layer;
            grass.GetComponent<SpriteRenderer>().sortingLayerName = LayerMask.LayerToName(grassLayers[layer - 1].layer);
        }
    }
    
    /// <summary>
    /// 在指定位置生成2x2格物品
    /// </summary>
    /// <param name="x"> x坐标</param>
    /// <param name="y"> y坐标</param>
    /// <param name="layer"> 生成的层级</param>
    /// <param name="random"> 随机数</param>
    /// <param name="withGrass"> 周围是否长草</param>
    public void Generate2X2TilePropAt(float x, float y, int layer, Random random, bool withGrass = false)
    {
        GameObject[] objects = withGrass ? twoTilePropWithGrassPrefabs : twoTilePropPrefabs;
        if (objects.Length == 0 || layer > layerProps.Length)
        {
            return;
        }
        GameObject instance = Instantiate(objects[random.Next(0, objects.Length)], layerProps[layer - 1].transform, true);
        instance.transform.position = new Vector3((float)(x + random.NextDouble()), (float)(y + random.NextDouble()), 0);
        instance.layer = layerProps[layer - 1].layer;
        instance.GetComponent<SpriteRenderer>().sortingLayerName = LayerMask.LayerToName(layerProps[layer - 1].layer);
        // 生点草
        if (withGrass && grassPrefabs.Length != 0)
        {
            GameObject grass = Instantiate(grassPrefabs[random.Next(0, grassPrefabs.Length)], grassLayers[layer - 1].transform, true);
            float xPos = (float)(1 - random.NextDouble() / 10) * ((random.Next() & 2) - 1);
            float yPos = (float)(1 - random.NextDouble() / 10) * ((random.Next() & 2) - 1);
            grass.transform.position = new Vector3(instance.transform.position.x + xPos, instance.transform.position.y + yPos, 0);
            grass.layer = grassLayers[layer - 1].layer;
            grass.GetComponent<SpriteRenderer>().sortingLayerName = LayerMask.LayerToName(grassLayers[layer - 1].layer);
        }
    }
    
    /// <summary>
    /// 在指定位置生成6x6格物品，固定位置
    /// </summary>
    /// <param name="x"> x坐标</param>
    /// <param name="y"> y坐标</param>
    /// <param name="layer"> 生成的层级</param>
    /// <param name="random"> 随机数</param>
    public void Generate6X6TilePropAt(float x, float y, int layer, Random random)
    {
        GameObject[] objects = sixTilePropPrefabs;
        if (objects.Length == 0 || layer > layerProps.Length)
        {
            return;
        }
        GameObject instance = Instantiate(objects[random.Next(0, objects.Length)], layerProps[layer - 1].transform, true);
        instance.transform.position = new Vector3(x, y, 0);
        instance.layer = layerProps[layer - 1].layer;
        var sprites = instance.GetComponentsInChildren<SpriteRenderer>();
        foreach (var render in sprites)
        {
            render.sortingLayerName = LayerMask.LayerToName(layerProps[layer - 1].layer);
        }
    }
    
    /// <summary>
    /// 在指定位置生成椅子
    /// </summary>
    /// <param name="x"> x坐标</param>
    /// <param name="y"> y坐标</param>
    /// <param name="layer"> 生成的层级</param>
    /// <param name="random"> 随机数</param>
    public void GenerateChairAt(float x, float y, int layer, Random random)
    {
        GameObject[] objects = chairPropPrefabs;
        if (objects.Length == 0 || layer > layerProps.Length)
        {
            return;
        }
        GameObject instance = Instantiate(objects[random.Next(0, objects.Length)], layerProps[layer - 1].transform, true);
        instance.transform.position = new Vector3(x, y, 0);
        instance.layer = layerProps[layer - 1].layer;
        instance.GetComponent<SpriteRenderer>().sortingLayerName = LayerMask.LayerToName(layerProps[layer - 1].layer);
    }
    
    /// <summary>
    /// 在指定位置生成二格植株
    /// </summary>
    /// <param name="x"> x坐标</param>
    /// <param name="y"> y坐标</param>
    /// <param name="layer"> 生成的层级</param>
    /// <param name="random"> 随机数</param>
    public void GenerateTwoTilePlantAt(float x, float y, int layer, Random random)
    {
        if (twoTilePlantsPrefabs.Length == 0 || layer > layerProps.Length)
        {
            return;
        }
        GameObject instance = Instantiate(twoTilePlantsPrefabs[random.Next(0, twoTilePlantsPrefabs.Length)], layerProps[layer - 1].transform, true);
        instance.transform.position = new Vector3((float)(x + random.NextDouble()), (float)(y + random.NextDouble()), 0);
        instance.layer = layerProps[layer - 1].layer;
        instance.GetComponent<SpriteRenderer>().sortingLayerName = LayerMask.LayerToName(layerProps[layer - 1].layer);
    }
    
    /// <summary>
    /// 在指定位置生成树
    /// </summary>
    /// <param name="x"> x坐标</param>
    /// <param name="y"> y坐标</param>
    /// <param name="layer"> 生成的层级</param>
    /// <param name="random"> 随机数</param>
    public void GenerateTreeAt(float x, float y, int layer, Random random)
    {
        if (treePrefabs.Length == 0 || layer > layerProps.Length)
        {
            return;
        }
        GameObject instance = Instantiate(treePrefabs[random.Next(0, treePrefabs.Length)], layerProps[layer - 1].transform, true);
        instance.transform.position = new Vector3((float)(x + random.NextDouble()), (float)(y + random.NextDouble()), 0);
        instance.layer = layerProps[layer - 1].layer;
        instance.transform.Find("Upper").GetComponent<SpriteRenderer>().sortingLayerName = "Layer " + (layer + 1);
        instance.transform.Find("Lower").GetComponent<SpriteRenderer>().sortingLayerName =  LayerMask.LayerToName(layerProps[layer - 1].layer);
        instance.transform.Find("Shadow").GetComponent<SpriteRenderer>().sortingLayerName =  LayerMask.LayerToName(layerProps[layer - 1].layer);
    }
}
