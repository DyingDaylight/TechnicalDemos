using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Gameplay.FloodEscape.Water
{
    public class WaterSimulation
    {
        private readonly IReadOnlyDungeonMap dungeonMap;
        private readonly List<WaterFront> fronts = new();
        private readonly float[,] waterLevels;
        private readonly float[,] waterChanges;
        private readonly float[,] outgoingWater;
        private readonly float inflowPerStep;

        private float pendingWater;

        public IReadOnlyList<WaterFront> Fronts => fronts;
        public int Width => dungeonMap.Width;
        public int Height => dungeonMap.Height;
        
        public WaterSimulation(IReadOnlyDungeonMap map, int sourceX, int sourceY, float inflowPerStep)
        {
            dungeonMap = map;
            waterLevels = new float[map.Width, map.Height];
            waterChanges = new float[map.Width, map.Height];
            outgoingWater = new float[map.Width, map.Height];
            
            if (sourceX < 0 || sourceX >= map.Width ||
                sourceY < 0 || sourceY >= map.Height ||
                !CanContainWater(sourceX, sourceY))
                throw new ArgumentException("Invalid water source position.");
            
            if (inflowPerStep < 0f)
                throw new ArgumentOutOfRangeException(nameof(inflowPerStep));
            
            this.inflowPerStep = inflowPerStep;
            
            WaterFront waterFront = new WaterFront(new Vector2Int(sourceX, sourceY));
            fronts.Add(waterFront);
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
        
        public void Step()
        {
            CalculateHorizontalFlow();
            ApplyHorizontalFlow();
            
            ApplyGravity();
            
            pendingWater += inflowPerStep;
            DistributeWater();
        }
        
        private void CalculateHorizontalFlow()
        {
            Array.Clear(outgoingWater, 0, outgoingWater.Length);
            Array.Clear(waterChanges, 0, waterChanges.Length);
            
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width - 1; x++)
                {
                    if (CanContainWater(x, y) && CanContainWater(x + 1, y))
                    {
                        float leftLevel = waterLevels[x, y];
                        float rightLevel = waterLevels[x + 1, y];

                        float difference = leftLevel - rightLevel;
                        float flow = difference * 0.5f;
                        
                        if (flow > 0f)
                            outgoingWater[x, y] += flow;
                        else
                            outgoingWater[x + 1, y] -= flow;
                    }
                }
            }
            
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width - 1; x++)
                {
                    if (CanContainWater(x, y) && CanContainWater(x + 1, y))
                    {
                        float leftLevel = waterLevels[x, y];
                        float rightLevel = waterLevels[x + 1, y];
                        
                        float difference = leftLevel - rightLevel;
                        float flow = difference * 0.5f;

                        if (flow == 0f)
                            continue;

                        int sourceX = flow > 0f ? x : x + 1;

                        float scale = Math.Min(1f,
                            waterLevels[sourceX, y] / outgoingWater[sourceX, y]);

                        flow *= scale;
                        
                        waterChanges[x, y] -= flow;
                        waterChanges[x + 1, y] += flow;
                    }
                }
            }
        }
        
        private void ApplyHorizontalFlow()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    waterLevels[x, y] += waterChanges[x, y];
                }
            }
        }
        
        private void ApplyGravity()
        {
            for (int y = 1; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (!CanContainWater(x, y) || !CanContainWater(x, y - 1))
                        continue;

                    float availableWater = waterLevels[x, y];
                    float freeSpace = 1f - waterLevels[x, y - 1];

                    float flow = Math.Min(availableWater, freeSpace);

                    waterLevels[x, y] -= flow;
                    waterLevels[x, y - 1] += flow;
                }
            }
        }
        
        private void DistributeWater()
        {
            const float epsilon = 0.0001f;

            while (pendingWater > epsilon)
            {
                CompleteNearlyFullCells();
                AdvanceFilledFronts();
                
                List<WaterFront> priorityFronts = GetLowestActiveFronts();
                
                if (priorityFronts.Count == 0)
                    break;

                float distributed = DistributeBetweenFronts(priorityFronts);
                
                pendingWater -= distributed;
                
                if (distributed <= 0f)
                    break;
            }
        }

        private float DistributeBetweenFronts(List<WaterFront> priorityFronts)
        {
            Dictionary<WaterFront, int> targetCounts = new();
            int totalTargets = 0;
                
            foreach (WaterFront front in priorityFronts)
            {
                int count = 0;

                foreach (Vector2Int target in GetFrontTargets(front))
                {
                    if (waterLevels[target.x, target.y] < 1f)
                        count++;
                }

                targetCounts[front] = count;
                totalTargets += count;
            }

            if (totalTargets == 0)
                return 0f;
                
            float availableWater = pendingWater;
            float distributed = 0f;

            foreach (WaterFront front in priorityFronts)
            {
                float budget = availableWater * targetCounts[front] / totalTargets;
                float remaining = ProcessFront(front, budget);
                distributed += budget - remaining;
            }
            
            return distributed;
        }

        private List<WaterFront> GetLowestActiveFronts()
        {
            int lowestLevel = int.MaxValue;
            List<WaterFront> priorityFronts = new();

            foreach (WaterFront front in fronts)
            {
                int level = FindLowestUnfilledLevel(front);
                    
                if (level == int.MaxValue)
                    continue;

                if (level < lowestLevel)
                {
                    lowestLevel = level;
                    priorityFronts.Clear();
                }
                    
                if (level == lowestLevel)
                    priorityFronts.Add(front);
            }
            
            return priorityFronts;
        }
        
        private void AdvanceFilledFronts()
        {
            foreach (WaterFront front in fronts)
            {
                while (front.Cells.Count > 0)
                {
                    List<Vector2Int> horizontalFront = GetFrontTargets(front);

                    if (horizontalFront.Count == 0)
                        break;

                    bool isFull = true;

                    foreach (Vector2Int cell in horizontalFront)
                    {
                        if (waterLevels[cell.x, cell.y] < 1f)
                        {
                            isFull = false;
                            break;
                        }
                    }

                    if (!isFull)
                        break;

                    List<Vector2Int> upperFront = FindUpperCells(horizontalFront);

                    if (front.IncomingFall.HasValue)
                    {
                        Vector2Int opening = front.IncomingFall.Value.Opening;
                        upperFront.Remove(opening);
                    }

                    front.Cells = upperFront;
                }
            }
        }

        private float ProcessFront(WaterFront currentFront, float amount)
        {
            while (currentFront.Cells.Count > 0 && amount > 0f)
            {
                List<Vector2Int> horizontalFront = GetFrontTargets(currentFront);
                
                amount = DistributeToTargets(horizontalFront, amount,  out List<WaterFall> falls);

                foreach (WaterFall fall in falls)
                {
                    AddFront(fall);
                }
                    
                bool isFull = true;

                foreach (Vector2Int cell in horizontalFront)
                {
                    if (waterLevels[cell.x, cell.y] < 1f)
                    {
                        isFull = false;
                        break;
                    }
                }

                if (!isFull)
                    break;

                List<Vector2Int> upperFront = FindUpperCells(horizontalFront);
                    
                if (currentFront.IncomingFall.HasValue)
                {
                    Vector2Int opening = currentFront.IncomingFall.Value.Opening;
                    upperFront.Remove(opening);
                }
                    
                currentFront.Cells = upperFront;
            }

            return amount;
        }
        
        private float DistributeToTargets(List<Vector2Int> targets, float amount, out List<WaterFall> falls)
        {
            falls = new List<WaterFall>();
            
            while (amount > 0)
            {
                int availableCount = 0;

                foreach (Vector2Int target in targets)
                {
                    if (waterLevels[target.x, target.y] < 1f)
                        availableCount++;
                }
                
                if (availableCount == 0)
                    break;

                float share = amount / availableCount;
                float distributed = 0f;
                
                foreach (Vector2Int target in targets)
                {
                    float current = waterLevels[target.x, target.y];
                    float freeSpace = 1f - current;
                    
                    if (freeSpace <= 0f)
                        continue;
                    
                    float increase = Math.Min(share, freeSpace);
                    float newLevel = current + increase;

                    float actualIncrease = newLevel - current;
                    
                    waterLevels[target.x, target.y] = newLevel;
                    distributed += actualIncrease;

                    if (actualIncrease > 0f)
                    {
                        Vector2Int? destination = FindFallDestination(target.x, target.y);

                        if (destination.HasValue)
                        {
                            Vector2Int opening = target;
                            if (!falls.Exists(fall => fall.Opening == opening))
                                falls.Add(new WaterFall(opening, destination.Value));
                        }
                    }
                }
                
                if (distributed <= 0f)
                    break;
                
                amount = Math.Max(0f, amount - distributed);
            }
            
            return amount;
        }

        
        private void AddFront(WaterFall fall)
        {
            foreach (WaterFront front in fronts)
            {
                if (front.EntryPoint == fall.Destination)
                    return;
            }

            WaterFront waterFront = new WaterFront(fall.Destination, fall);
            fronts.Add(waterFront);
        }
        
        private int FindLowestUnfilledLevel(WaterFront front)
        {
            int minY = int.MaxValue;

            foreach (Vector2Int cell in front.Cells)
            {
                if (waterLevels[cell.x, cell.y] < 1f)
                    minY = Mathf.Min(minY, cell.y);
            }

            return minY;
        }
        
        private List<Vector2Int> GetFrontTargets(WaterFront front)
        {
            List<Vector2Int> targets = new();

            foreach (Vector2Int cell in front.Cells)
            {
                foreach (Vector2Int target in GetHorizontalTargets(cell.x, cell.y))
                {
                    if (!targets.Contains(target))
                        targets.Add(target);
                }
            }

            return targets;
        }
        
        private List<Vector2Int> FindUpperCells(List<Vector2Int> cells)
        {
            List<Vector2Int> result = new List<Vector2Int>();
            
            foreach (Vector2Int cell in cells)
            {
                int nextY = cell.y + 1;
                if (dungeonMap.IsInside(cell.x, nextY) && CanContainWater(cell.x, nextY)
                                                       && waterLevels[cell.x, nextY] < 1f)
                    result.Add(new Vector2Int(cell.x, nextY));
            }
            
            return result;
        }
        
        
        private List<Vector2Int> GetHorizontalTargets(int x, int y)
        {
            List<Vector2Int> cells = new List<Vector2Int>();
            
            if (!dungeonMap.IsInside(x, y) || !CanContainWater(x, y))
                return cells;
            
            if (waterLevels[x, y] <= 0f)
            {
                cells.Add(new Vector2Int(x, y));
                return cells;
            }
            
            cells.Add(new Vector2Int(x, y));

            foreach (int direction in new[] {-1, 1})
            {
                for (int nextX = x + direction;
                     dungeonMap.IsInside(nextX, y) && CanContainWater(nextX, y);
                     nextX += direction)
                {
                    bool isDry = waterLevels[nextX, y] <= 0f;
 
                    cells.Add(new Vector2Int(nextX, y));
                    
                    if (isDry)
                        break;
                }
            }
            
            return cells;
        }
        
        private Vector2Int? FindFallDestination(int x, int y)
        {
            Vector2Int? fallDestination = null;
            
            for (int nextY = y - 1; nextY >= 0; nextY--)
            {
                if (!dungeonMap.IsInside(x, nextY) || !CanContainWater(x, nextY))
                    break;
                
                if (waterLevels[x, nextY] >= 1f)
                    break;
                
                fallDestination = new Vector2Int(x, nextY);
            }
            
            return fallDestination;
        }
        
        private void CompleteNearlyFullCells()
        {
            const float threshold = 0.99f;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (!CanContainWater(x, y))
                        continue;

                    float level = waterLevels[x, y];

                    if (level < threshold || level >= 1f)
                        continue;

                    float missing = 1f - level;

                    if (pendingWater < missing)
                        continue;

                    waterLevels[x, y] = 1f;
                    pendingWater -= missing;
                }
            }
        }
    }
}