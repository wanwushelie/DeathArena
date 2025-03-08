
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace MyMap
{
    public class Map : MonoBehaviour
    {
        private const int GRASS_TILE = 0;
        private const int UP_WALL_TILE = 1;
        private const int NORMAL_WALL_TILE = 2;
        private const int LEFT_UP_TILE = 3;
        private const int RIGHT_UP_TILE = 4;
        private const int OUT_LEFT_WALL_TILE = 5;
        private const int OUT_RIGHT_WALL_TILE = 6;
        private const int LEFT_TILE = 7;
        private const int RIGHT_TILE = 8;
        private const int OUT_TO_LEFT_TILE = 9;
        private const int OUT_TO_RIGHT_TILE = 10;
        private const int LEFT_BOTTOM_TILE = 11;
        private const int RIGHT_BOTTOM_TILE = 12;
        private const int SHADOW_WALL_TILE = 13;
        private const int WINDOW_TILE = 14;
        private const int GRASS_NO_LEFT_UP_TILE = 15;
        private const int GRASS_NO_RIGHT_UP_TILE = 16;
        private const int ROAD = 17;
        private const int GRASS_ROAD_TILE = 18;
        public Tilemap layer1Grass; // 第一层草地
        public Tilemap layer1Wall;  // 第一层墙体
        public Tilemap layer1Shadow;  // 第一层墙体
        public Tilemap layer2Grass; // 第二层草地
        public Tilemap layer2Wall;  // 第二层墙体
        public TileBase[] tiles; // TileBase对象的数组，每个元素代表一种瓦片
        public TileBase[] shadowTiles;  // 阴影瓦片
        public Props propsGenerator;
        
        
        // 偏移量必须大于等于0
        int offsetY = 10;
        int offsetX = 0;

        void GenerateLayer1Tiles(byte[,] map)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (map[x, y] == 255)
                    {
                        continue;
                    }

                    if (map[x, y] <= 254)
                    {
                        // 将瓦片放置到Tilemap中的对应位置
                        if (map[x, y] != TypeConstants.BOTTOM_WALL && map[x, y] != TypeConstants.BOTTOM_LEFT_WALL &&
                            map[x, y] != TypeConstants.BOTTOM_RIGHT_WALL)
                        {
                            layer1Grass.SetTile(new Vector3Int(x, y, 0), tiles[GRASS_TILE]);
                        }

                        switch (map[x, y])
                        {
                            case TypeConstants.UP_WALL:
                            {
                                layer1Wall.SetTile(new Vector3Int(x, y, 0), tiles[UP_WALL_TILE]);
                                break;
                            }
                            case TypeConstants.BOTTOM_WALL:
                            {
                                layer1Wall.SetTile(new Vector3Int(x, y, 0), tiles[NORMAL_WALL_TILE]);
                                layer1Wall.SetTile(new Vector3Int(x, y - 1, 0), tiles[NORMAL_WALL_TILE]);
                                break;
                            }
                            case TypeConstants.UP_RIGHT_WALL:
                            {
                                layer1Wall.SetTile(new Vector3Int(x, y, 0), tiles[RIGHT_UP_TILE]);
                                layer1Grass.SetTile(new Vector3Int(x, y, 0), tiles[GRASS_NO_RIGHT_UP_TILE]);
                                break;
                            }
                            case TypeConstants.UP_LEFT_WALL:
                            {
                                layer1Wall.SetTile(new Vector3Int(x, y, 0), tiles[LEFT_UP_TILE]);
                                layer1Grass.SetTile(new Vector3Int(x, y, 0), tiles[GRASS_NO_LEFT_UP_TILE]);
                                break;
                            }
                            case TypeConstants.BOTTOM_LEFT_WALL:
                            {
                                layer1Wall.SetTile(new Vector3Int(x, y, 0), tiles[OUT_LEFT_WALL_TILE]);
                                layer1Wall.SetTile(new Vector3Int(x, y - 1, 0), tiles[OUT_LEFT_WALL_TILE]);
                                break;
                            }
                            case TypeConstants.BOTTOM_RIGHT_WALL:
                            {
                                layer1Wall.SetTile(new Vector3Int(x, y, 0), tiles[OUT_RIGHT_WALL_TILE]);
                                layer1Wall.SetTile(new Vector3Int(x, y - 1, 0), tiles[OUT_RIGHT_WALL_TILE]);
                                break;
                            }
                            case TypeConstants.LEFT_WALL:
                            {
                                layer1Wall.SetTile(new Vector3Int(x, y, 0), tiles[LEFT_TILE]);
                                break;
                            }
                            case TypeConstants.RIGHT_WALL:
                            {
                                layer1Wall.SetTile(new Vector3Int(x, y, 0), tiles[RIGHT_TILE]);
                                break;
                            }
                            case TypeConstants.OUT_TO_LEFT_TILE:
                            {
                                layer1Wall.SetTile(new Vector3Int(x, y, 0), tiles[OUT_TO_LEFT_TILE]);
                                break;
                            }
                            case TypeConstants.OUT_TO_RIGHT_TILE:
                            {
                                layer1Wall.SetTile(new Vector3Int(x, y, 0), tiles[OUT_TO_RIGHT_TILE]);
                                break;
                            }
                            case TypeConstants.LEFT_BOTTOM_TILE:
                            {
                                layer1Wall.SetTile(new Vector3Int(x, y, 0), tiles[LEFT_BOTTOM_TILE]);
                                break;
                            }
                            case TypeConstants.RIGHT_BOTTOM_TILE:
                            {
                                layer1Wall.SetTile(new Vector3Int(x, y, 0), tiles[RIGHT_BOTTOM_TILE]);
                                break;
                            }
                            case TypeConstants.SHADOW_WALL_TILE:
                            {
                                layer1Wall.SetTile(new Vector3Int(x, y, 0), tiles[SHADOW_WALL_TILE]);
                                layer1Wall.SetTile(new Vector3Int(x, y - 1, 0), tiles[SHADOW_WALL_TILE]);
                                break;
                            }
                            case TypeConstants.WINDOW_TILE:
                            {
                                layer1Wall.SetTile(new Vector3Int(x, y, 0), tiles[WINDOW_TILE]);
                                layer1Wall.SetTile(new Vector3Int(x, y - 1, 0), tiles[WINDOW_TILE]);
                                break;
                            }
                            case TypeConstants.ROAD:
                            {
                                layer1Grass.SetTile(new Vector3Int(x, y - 1, 0), tiles[ROAD]);
                                break;
                            }
                            case TypeConstants.GRASS_ROAD_TILE:
                            {
                                layer1Grass.SetTile(new Vector3Int(x, y - 1, 0), tiles[GRASS_ROAD_TILE]);
                                break;
                            }
                        }
                        
                    }
                }
            }
        }

        void GenerateLayer2Tiles(byte[,] map)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (map[x, y] == 255)
                    {
                        continue;
                    }
                    if (map[x, y] <= 254)
                    {
                        // 将瓦片放置到Tilemap中的对应位置
                        if (map[x, y] != TypeConstants.BOTTOM_WALL && map[x, y] != TypeConstants.BOTTOM_LEFT_WALL &&
                            map[x, y] != TypeConstants.BOTTOM_RIGHT_WALL && map[x, y] != TypeConstants.STAIR_POINT &&
                            map[x, y] != TypeConstants.STAIR_AROUND_POINT)
                        {
                            layer2Grass.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[GRASS_TILE]);
                        }
                        
                        if (map[x, y] == 100)
                        {
                            propsGenerator.GenerayePropAt(TypeConstants.OBJECT_STAIR, x + offsetX + 1, y + offsetY + 1);
                            // layer2Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[NORMAL_WALL_TILE]);
                        }

                        switch (map[x, y])
                        {
                            //case 1:
                            case TypeConstants.UP_WALL:
                            {
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[UP_WALL_TILE]);
                                layer2Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[UP_WALL_TILE]);
                                break;
                            }
                            case TypeConstants.BOTTOM_WALL:
                            {
                                layer2Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[NORMAL_WALL_TILE]);
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[NORMAL_WALL_TILE]);
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY - 1, 0), tiles[NORMAL_WALL_TILE]);
                                break;
                            }
                            case TypeConstants.UP_RIGHT_WALL:
                            {
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[RIGHT_UP_TILE]);
                                layer2Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[RIGHT_UP_TILE]);
                                layer2Grass.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[GRASS_NO_RIGHT_UP_TILE]);
                                break;
                            }
                            case TypeConstants.UP_LEFT_WALL:
                            {
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[LEFT_UP_TILE]);
                                layer2Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[LEFT_UP_TILE]);
                                layer2Grass.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[GRASS_NO_LEFT_UP_TILE]);
                                break;
                            }
                            case TypeConstants.BOTTOM_LEFT_WALL:
                            {
                                layer2Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[OUT_LEFT_WALL_TILE]);
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[OUT_LEFT_WALL_TILE]);
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY - 1, 0), tiles[OUT_LEFT_WALL_TILE]);
                                break;
                            }
                            case TypeConstants.BOTTOM_RIGHT_WALL:
                            {
                                layer2Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[OUT_RIGHT_WALL_TILE]);
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[OUT_RIGHT_WALL_TILE]);
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY - 1, 0), tiles[OUT_RIGHT_WALL_TILE]);
                                break;
                            }
                            case TypeConstants.LEFT_WALL:
                            {
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[LEFT_TILE]);
                                layer2Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[LEFT_TILE]);
                                break;
                            }
                            case TypeConstants.RIGHT_WALL:
                            {
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[RIGHT_TILE]);
                                layer2Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[RIGHT_TILE]);
                                break;
                            }
                            case TypeConstants.OUT_TO_LEFT_TILE:
                            {
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[OUT_TO_LEFT_TILE]);
                                layer2Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[OUT_TO_LEFT_TILE]);
                                break;
                            }
                            case TypeConstants.OUT_TO_RIGHT_TILE:
                            {
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[OUT_TO_RIGHT_TILE]);
                                layer2Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[OUT_TO_RIGHT_TILE]);
                                break;
                            }
                            case TypeConstants.LEFT_BOTTOM_TILE:
                            {
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[LEFT_BOTTOM_TILE]);
                                layer2Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[LEFT_BOTTOM_TILE]);
                                break;
                            }
                            case TypeConstants.RIGHT_BOTTOM_TILE:
                            {
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[RIGHT_BOTTOM_TILE]);
                                layer2Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[RIGHT_BOTTOM_TILE]);
                                break;
                            }
                            case TypeConstants.SHADOW_WALL_TILE:
                            {
                                layer2Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[SHADOW_WALL_TILE]);
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[SHADOW_WALL_TILE]);
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY - 1, 0), tiles[SHADOW_WALL_TILE]);
                                break;
                            }
                            case TypeConstants.WINDOW_TILE:
                            {
                                layer2Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[WINDOW_TILE]);
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tiles[WINDOW_TILE]);
                                layer1Wall.SetTile(new Vector3Int(x + offsetX, y + offsetY - 1, 0), tiles[WINDOW_TILE]);
                                break;
                            }
                            case TypeConstants.ROAD:
                            {
                                layer2Grass.SetTile(new Vector3Int(x + offsetX, y + offsetY - 1, 0), tiles[ROAD]);
                                break;
                            }
                            case TypeConstants.GRASS_ROAD_TILE:
                            {
                                layer2Grass.SetTile(new Vector3Int(x + offsetX, y + offsetY - 1, 0), tiles[GRASS_ROAD_TILE]);
                                break;
                            }
                        }
                        
                    }
                }
            }
        }

        void GenerateLayer1Shadow(byte[,] map)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (map[x, y] == 255)
                    {
                        continue;
                    }
                    layer1Shadow.SetTile(new Vector3Int(x, y, 0), shadowTiles[map[x, y]]);
                }
            }
        }
        
        void GenerateLayerPlant(byte[,] map)
        {
            Random random = new Random(TypeConstants.seed);
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    switch (map[x, y])
                    {
                        case TypeConstants.OBJECT_GRASS_LAYER1:
                        {
                            propsGenerator.GenerateGrassAt(x, y, 1, random );
                            break;
                        }
                        case TypeConstants.OBJECT_GRASS_LAYER2:
                        {
                            propsGenerator.GenerateGrassAt(x, y, 2, random );
                            break;
                        }
                        case TypeConstants.OBJECT_ONE_PLANT_LAYER1:
                        {
                            propsGenerator.GenerateOneTilePlantAt(x, y, 1, random);
                            break;
                        }
                        
                        case TypeConstants.OBJECT_ONE_PLANT_LAYER2:
                        {
                            propsGenerator.GenerateOneTilePlantAt(x, y, 2, random);
                            break;
                        }
                        case TypeConstants.OBJECT_TWO_PLANT_LAYER1:
                        {
                            propsGenerator.GenerateTwoTilePlantAt(x, y, 1, random);
                            break;
                        }
                        
                        case TypeConstants.OBJECT_TWO_PLANT_LAYER2:
                        {
                            propsGenerator.GenerateTwoTilePlantAt(x, y, 2, random);
                            break;
                        }
                        case TypeConstants.OBJECT_TREE_LAYER1:
                        {
                            propsGenerator.GenerateTreeAt(x, y, 1, random);
                            break;
                        }
                        
                        case TypeConstants.OBJECT_TREE_LAYER2:
                        {
                            propsGenerator.GenerateTreeAt(x, y, 2, random);
                            break;
                        }
                        case TypeConstants.OBJECT_1X1_PROP_LAYER1:
                        {
                            propsGenerator.Generate1X1TilePropAt(x, y, 1, random);
                            break;
                        }
                        case TypeConstants.OBJECT_1X1_PROP_LAYER2:
                        {
                            propsGenerator.Generate1X1TilePropAt(x, y, 2, random);
                            break;
                        }
                        case TypeConstants.OBJECT_1X1_PROP_GRASS_LAYER1:
                        {
                            propsGenerator.Generate1X1TilePropAt(x, y, 1, random, true);
                            break;
                        }
                        case TypeConstants.OBJECT_1X1_PROP_GRASS_LAYER2:
                        {
                            propsGenerator.Generate1X1TilePropAt(x, y, 2, random, true);
                            break;
                        }
                        case TypeConstants.OBJECT_2X2_PROP_LAYER1:
                        {
                            propsGenerator.Generate2X2TilePropAt(x, y, 1, random);
                            break;
                        }
                        case TypeConstants.OBJECT_2X2_PROP_LAYER2:
                        {
                            propsGenerator.Generate2X2TilePropAt(x, y, 2, random);
                            break;
                        }
                        case TypeConstants.OBJECT_2X2_PROP_GRASS_LAYER1:
                        {
                            propsGenerator.Generate2X2TilePropAt(x, y, 1, random, true);
                            break;
                        }
                        case TypeConstants.OBJECT_2X2_PROP_GRASS_LAYER2:
                        {
                            propsGenerator.Generate2X2TilePropAt(x, y, 2, random, true);
                            break;
                        }
                        case TypeConstants.OBJECT_6X6_PROP_GRASS_LAYER1:
                        {
                            propsGenerator.Generate6X6TilePropAt(x, y, 1, random);
                            break;
                        }
                        case TypeConstants.OBJECT_6X6_PROP_GRASS_LAYER2:
                        {
                            propsGenerator.Generate6X6TilePropAt(x, y, 2, random);
                            break;
                        }
                        case TypeConstants.OBJECT_CHAIR:
                        {
                            propsGenerator.GenerateChairAt(x, y, 1, random);
                            break;
                        }
                    }
                }
            }
        }
        
        private void Start()
        {
            var maps = ChunkGenerate.RandomMap(6);
            GenerateLayer1Tiles(maps[0]);
            GenerateLayer2Tiles(maps[1]);
            var shadowMap = ChunkGenerate.GenerateShadow(maps[0], maps[1], offsetX, offsetY);
            GenerateLayer1Shadow(shadowMap);
            var plantMap = ChunkGenerate.GenerateProps(maps[0], maps[1], offsetX, offsetY);
            GenerateLayerPlant(plantMap);
        }
    }
}