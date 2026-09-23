using System.Collections.Generic;
using Core;
using DataStructures;
using UnityEngine;

namespace Generation
{
    public class RandomizedKruskal : MazeGenerator
    {
        private struct MazeEdge
        {
            public Vector2Int First;
            public Vector2Int Second;

            public MazeEdge(Vector2Int first, Vector2Int second)
            {
                First = first;
                Second = second;
            }
        }
        
        public RandomizedKruskal()
        {
        }

        public RandomizedKruskal(int seed) : base(seed)
        {
        }
        
        public override void Generate(DungeonMap map)
        {
            FillWithWalls(map);

            List<MazeEdge> edges = new List<MazeEdge>();
            DisjointSet sets = new DisjointSet();
            
            Initialize(map, edges, sets);
            ShuffleEdges(edges);

            foreach (MazeEdge edge in edges)
            {
                if (sets.Find(edge.First) != sets.Find(edge.Second))
                {
                    sets.Union(edge.First, edge.Second);
                    MakeWay(edge.First, edge.Second, map);
                }
            }
            
        }
        
        private void Initialize(DungeonMap map, List<MazeEdge> edges, DisjointSet sets)
        {
            for (int x = 1; x < map.Width - 1; x += 2)
            {
                for (int y = 1; y < map.Height - 1; y += 2)
                {
                    Vector2Int first = new Vector2Int(x, y);
                    
                    sets.MakeSet(first);
                    
                    Vector2Int right = new Vector2Int(x + 2, y);
                    Vector2Int up = new Vector2Int(x, y + 2);
                    
                    if (IsInside(right, map))
                        edges.Add(new MazeEdge(first, right));
                    if (IsInside(up, map))
                        edges.Add(new MazeEdge(first, up));
                }
            }
        } 
        
        private void ShuffleEdges(List<MazeEdge> list)
        {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = _random.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}