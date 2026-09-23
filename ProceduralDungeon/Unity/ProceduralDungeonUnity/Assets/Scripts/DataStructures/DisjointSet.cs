using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures
{
    public class DisjointSet
    {
        private readonly Dictionary<Vector2Int, Vector2Int> parents = new();
        private readonly Dictionary<Vector2Int, int> sizes = new();
        
        public void MakeSet(Vector2Int item)
        {
            parents[item] = item;
            sizes[item] = 1;
        }

        public Vector2Int Find(Vector2Int item)
        {
            if (!parents.ContainsKey(item))
                throw new ArgumentException("Item is not in the disjoint set.");
            
            if (item == parents[item])
                return item;
            
            parents[item] = Find(parents[item]);
            
            return parents[item];
        }

        public void Union(Vector2Int a, Vector2Int b)
        {
            Vector2Int rootA = Find(a);
            Vector2Int rootB = Find(b);
            
            if (rootA == rootB)
                return;

            if (sizes[rootA] < sizes[rootB])
            {
                parents[rootA] = rootB;
                sizes[rootB] += sizes[rootA];
            }
            else
            {
                parents[rootB] = rootA;
                sizes[rootA] += sizes[rootB];
            }
        }
    }
}