using System;
using Core;

namespace Gameplay.FloodEscape
{
    public class WaterSimulation
    {
        private readonly IReadOnlyDungeonMap dungeonMap;
        private readonly float[,] waterLevels;
        
        public int Width => dungeonMap.Width;
        public int Height => dungeonMap.Height;
        
        public WaterSimulation(IReadOnlyDungeonMap map)
        {
            dungeonMap = map;
            waterLevels = new float[map.Width, map.Height];
        }
        
        public float this[int x, int y]
        {
            get => waterLevels[x, y];
            set
            {
                if (!CanContainWater(x, y))
                    return;

                waterLevels[x, y] = Math.Clamp(value, 0f, 1f);
            }
        }
        
        public bool CanContainWater(int x, int y)
        {
            return dungeonMap[x, y] == TileType.Floor;
        }
    }
}