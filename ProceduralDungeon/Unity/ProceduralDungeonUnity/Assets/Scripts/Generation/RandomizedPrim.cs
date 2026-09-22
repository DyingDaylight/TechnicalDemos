
using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Generation
{
    public class RandomizedPrim : MazeGenerator
    {
        public RandomizedPrim()
        {
        }

        public RandomizedPrim(int seed) : base(seed)
        {
        }
        
        public override void Generate(DungeonMap map)
        {
            FillWithWalls(map);
            
            List<Vector2Int> frontier = new List<Vector2Int>();
            Func<Vector2Int, DungeonMap, bool> isUnseen =
                (coordinate, dungeonMap) =>
                    IsWall(coordinate, dungeonMap)
                    && !frontier.Contains(coordinate);
            
            Vector2Int current = new Vector2Int(1, 1);
            map[current.x, current.y] = TileType.Floor;
            
            List<Vector2Int> availableNeighbors = GetAvailableNeighbors(current, map, isUnseen);
            frontier.AddRange(availableNeighbors);

            while (frontier.Count > 0)
            {
                Vector2Int currentTile = frontier[_random.Next(frontier.Count)];
                frontier.Remove(currentTile);
                
                List<Vector2Int> neighbors = GetAvailableNeighbors(currentTile, map, IsFloor);
                Vector2Int nextTile = neighbors[_random.Next(neighbors.Count)];
                MakeWay(currentTile, nextTile, map);    
                
                List<Vector2Int> unseenNeighbors = GetAvailableNeighbors(currentTile, map, isUnseen);
                frontier.AddRange(unseenNeighbors);
            }
        }
    }
}