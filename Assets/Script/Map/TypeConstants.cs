namespace MyMap
{
    public class TypeConstants
    {
        public const int seed = 543341;
        public const int GRASS = 0;
        public const int WALL = 1;
        public const int UP_WALL = 2;
        public const int BOTTOM_WALL = 3; 
        public const int UP_LEFT_WALL = 4;
        public const int UP_RIGHT_WALL = 5;
        public const int BOTTOM_LEFT_WALL = 6;
        public const int BOTTOM_RIGHT_WALL = 7;
        public const int LEFT_WALL = 8;
        public const int RIGHT_WALL = 9;
        public const int OUT_TO_LEFT_TILE = 10;
        public const int OUT_TO_RIGHT_TILE = 11;
        public const int LEFT_BOTTOM_TILE = 12;
        public const int RIGHT_BOTTOM_TILE = 13;
        public const int SHADOW_WALL_TILE = 14;
        public const int WINDOW_TILE = 15;
        public const int ROAD = 16;
        public const int GRASS_ROAD_TILE = 17;
        
        
        public const int STAIR_POINT = 100;
        public const int STAIR_AROUND_POINT = 101;

        public const int NOTHING = 255;
        
        // ------阴影编号------
        public const int SHADOW_HALF = 0;
        public const int SHADOW_QUARTER = 1;
        public const int SHADOW_HALF_INCOMPLETE = 2;
        public const int SHADOW_TRIANGLE = 3;
        
        // ------地块上物品编号------
        public const int OBJECT_STAIR = 0;
        // 该格被其他物品占用，但显示处理时当空处理
        public const int OBJECT_USING = 1;
        public const int OBJECT_GRASS_LAYER1 = 10;
        public const int OBJECT_GRASS_LAYER2 = 11;
        public const int OBJECT_ONE_PLANT_LAYER1 = 12;
        public const int OBJECT_ONE_PLANT_LAYER2 = 13;
        public const int OBJECT_TWO_PLANT_LAYER1 = 14;
        public const int OBJECT_TWO_PLANT_LAYER2 = 15;
        public const int OBJECT_TREE_LAYER1 = 16;
        public const int OBJECT_TREE_LAYER2 = 17;
        public const int OBJECT_1X1_PROP_LAYER1 = 18;
        public const int OBJECT_1X1_PROP_LAYER2 = 19;
        public const int OBJECT_1X1_PROP_GRASS_LAYER1 = 20;
        public const int OBJECT_1X1_PROP_GRASS_LAYER2 = 21;
        public const int OBJECT_2X2_PROP_LAYER1 = 22;
        public const int OBJECT_2X2_PROP_LAYER2 = 23;
        public const int OBJECT_2X2_PROP_GRASS_LAYER1 = 24;
        public const int OBJECT_2X2_PROP_GRASS_LAYER2 = 25;
        public const int OBJECT_6X6_PROP_GRASS_LAYER1 = 26;
        public const int OBJECT_6X6_PROP_GRASS_LAYER2 = 27;
        
        public const int OBJECT_CHAIR = 30;
    }
}