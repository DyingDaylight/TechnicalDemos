using System;
using System.Collections.Generic;
using Analysis;
using Core;
using UnityEngine;

namespace Gameplay.FloodEscape
{
    public static class DiamondPlacement
    {
        public static Vector2Int FindPosition(IReadOnlyDungeonMap map,
            Vector2Int entrance, Vector2Int exit, float targetDetourRatio)
        {
            // Calculate shortest-path distances from both endpoints.
            int[,] fromEntrance = DungeonAnalyzer.GetDistances(map, entrance);
            int[,] fromExit = DungeonAnalyzer.GetDistances(map, exit);
            
            int directDistance = fromEntrance[exit.x, exit.y];
            
            if (directDistance <= 0)
                throw new ArgumentException("Exit must be reachable from entrance and different from it.",
                    nameof(exit));
            
            // Diamonds can only be placed in dead ends.
            List<Vector2Int> deadEnds = DungeonAnalyzer.FindDeadEnds(map);
            
            bool found = false;
            Vector2Int bestPosition = default;
            float bestDifference = float.MaxValue;
            
            float bestEdgeDistance = float.MaxValue;
            float bestHeightDistance = float.MaxValue;

            float centerY = (map.Height - 1) / 2f;

            foreach (Vector2Int position in deadEnds)
            {
                // Exclude endpoints and unreachable dead ends.
                if (position == entrance || position == exit ||
                    fromEntrance[position.x, position.y] == -1 ||
                    fromExit[position.x, position.y] == -1)
                    continue;
                
                // Measure the extra travel required to collect the diamond
                // compared to going directly from entrance to exit.            
                float detourRatio = (fromEntrance[position.x, position.y] + fromExit[position.x, position.y] - directDistance) / (float)directDistance;
                float difference = Mathf.Abs(detourRatio - targetDetourRatio);

                // Use proximity to a side edge, then vertical center, as tie-breakers.
                float edgeDistance = Mathf.Min(position.x, map.Width - 1 - position.x);
                float heightDistance = Mathf.Abs(position.y - centerY);
                
                bool sameDetour = found && Mathf.Approximately(difference, bestDifference);

                bool betterDetour = !found || (!sameDetour && difference < bestDifference);

                bool betterGeography = edgeDistance < bestEdgeDistance ||
                                       (edgeDistance == bestEdgeDistance && heightDistance < bestHeightDistance);

                // Prioritize the target detour ratio.
                // Break ties by proximity to a side edge, then vertical center.
                if (betterDetour || (sameDetour && betterGeography))
                {
                    bestPosition = position;
                    bestDifference = difference;
                    bestEdgeDistance = edgeDistance;
                    bestHeightDistance = heightDistance;
                    found = true;
                }
            }

            if (!found)
                throw new InvalidOperationException("No suitable dead end found for diamond placement.");
            
            return bestPosition;
        }
    }
}