using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Generation
{
    public abstract class MazeGenerator
    {
        protected readonly System.Random _random;

        public MazeGenerator() : this(new System.Random())
        {
        }

        public MazeGenerator(int seed) : this(new System.Random(seed))
        {
        }

        private MazeGenerator(System.Random random)
        {
            _random = random;
        }
        
        public abstract void Generate(DungeonMap map);
        
        protected void FillWithWalls(DungeonMap map)
        {
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    map[x, y] = TileType.Wall;
                }
            }
        }
        
        protected List<Vector2Int> GetAvailableNeighbors(Vector2Int current, DungeonMap map, Func<Vector2Int, DungeonMap, bool> isAvailable)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            
            Vector2Int right = new Vector2Int(current.x + 2, current.y);
            Vector2Int left = new Vector2Int(current.x - 2, current.y);
            Vector2Int up = new Vector2Int(current.x, current.y + 2);
            Vector2Int down = new Vector2Int(current.x, current.y - 2);
            
            if (isAvailable(right, map)) neighbors.Add(right);
            if (isAvailable(left, map)) neighbors.Add(left);
            if (isAvailable(up, map)) neighbors.Add(up);
            if (isAvailable(down, map)) neighbors.Add(down);
            
            return neighbors;
        }
        
        protected void MakeWay(Vector2Int current, Vector2Int next, DungeonMap map)
        {
            Vector2Int middle = (current + next) / 2;
            map[current.x, current.y] = TileType.Floor;
            map[middle.x, middle.y] = TileType.Floor;
            map[next.x, next.y] = TileType.Floor;
        }

        protected bool IsInside(Vector2Int coordinate, DungeonMap map)
        {
            return coordinate.x >= 1
                   && coordinate.x < map.Width - 1
                   && coordinate.y >= 1
                   && coordinate.y < map.Height - 1;
        }

        protected bool IsWall(Vector2Int coordinate, DungeonMap map)
        {
            return IsInside(coordinate, map) && map[coordinate.x, coordinate.y] == TileType.Wall;
        }

        protected bool IsFloor(Vector2Int coordinate, DungeonMap map)
        {
            return IsInside(coordinate, map) && map[coordinate.x, coordinate.y] == TileType.Floor;
        }
    }
}