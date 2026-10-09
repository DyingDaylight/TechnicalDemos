using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Analysis
{
    public static class DungeonAnalyzer
    {
        private static readonly Vector2Int[] Directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
        
        public static List<Vector2Int> FindDeadEnds(IReadOnlyDungeonMap map)
        {
            List<Vector2Int> deadEnds = new List<Vector2Int>();
            
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (map[x, y] != TileType.Floor)
                        continue;
                    
                    int floorNeighbors = 0;

                    foreach (Vector2Int direction in Directions)
                    {
                        int nx = x + direction.x;
                        int ny = y + direction.y;

                        if (IsInside(map, nx, ny) && map[nx, ny] == TileType.Floor)
                            floorNeighbors++;
                    }

                    if (floorNeighbors == 1)
                        deadEnds.Add(new Vector2Int(x, y));
                    
                }
            }    
            
            return deadEnds;
        }

        public static int[,] GetDistances(IReadOnlyDungeonMap map, Vector2Int start)
        {
            if (!IsInside(map, start.x, start.y) || map[start.x, start.y] != TileType.Floor)
                throw new ArgumentException(
                    "Start position must be inside the map and on a floor tile.",
                    nameof(start));
            
            int[,] distances = new int[map.Width, map.Height];
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                    distances[x, y] = -1;
            }

            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            
            distances[start.x, start.y] = 0;
            queue.Enqueue(start);
            
            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                
                foreach (Vector2Int direction in Directions)
                {
                    int nx = current.x + direction.x;
                    int ny = current.y + direction.y;

                    if (!IsInside(map, nx, ny) || map[nx, ny] != TileType.Floor || distances[nx, ny] != -1)
                        continue;
                    
                    distances[nx, ny] = distances[current.x, current.y] + 1;
                    queue.Enqueue(new Vector2Int(nx, ny));
                }
            }
            
            return distances;
        }
        
        private static bool IsInside(IReadOnlyDungeonMap map, int x, int y)
        {
            return x >= 0 && x < map.Width &&
                   y >= 0 && y < map.Height;
        }
    }
}