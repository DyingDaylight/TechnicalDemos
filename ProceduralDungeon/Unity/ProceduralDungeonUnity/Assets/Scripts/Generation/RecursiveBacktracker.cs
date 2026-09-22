using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Generation
{
    public class RecursiveBacktracker : MazeGenerator
    {
        
        public RecursiveBacktracker()
        {
        }

        public RecursiveBacktracker(int seed) : base(seed)
        {
        }
        
        public override void Generate(DungeonMap map)
        {
            FillWithWalls(map);
            
            Stack<Vector2Int> stack = new Stack<Vector2Int>();
            
            Vector2Int current = new Vector2Int(1, 1);
            map[current.x, current.y] = TileType.Floor;
            int iterations = 0;
            
            do
            {
                if (++iterations > 100_000)
                    throw new Exception("Maze generation exceeded iteration limit");

                List<Vector2Int> availableNeighbors = GetAvailableNeighbors(current, map, IsWall);
                if (availableNeighbors.Count == 0)
                {
                    if (stack.Count == 0)
                        break;
                    
                    current = stack.Pop();
                    continue;
                }
                
                Vector2Int next = availableNeighbors[_random.Next(availableNeighbors.Count)];
                stack.Push(current);
                MakeWay(current, next, map);
                current = next;
            } while (true);
        }
    }
}