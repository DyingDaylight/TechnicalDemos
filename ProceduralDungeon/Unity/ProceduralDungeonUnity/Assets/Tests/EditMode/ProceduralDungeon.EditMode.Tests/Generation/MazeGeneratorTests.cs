using System;
using System.Collections.Generic;
using Core;
using Generation;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Generation
{
    public class MazeGeneratorTests
    {
        [TestCase(MazeAlgorithm.RecursiveBacktracker)]
        [TestCase(MazeAlgorithm.RandomizedPrim)]
        [TestCase(MazeAlgorithm.RandomizedKruskal)]
        public void Generate_KeepsBorderWalls(MazeAlgorithm algorithm)
        {
            // Arrange
            DungeonMap map = new DungeonMap(10, 7);
            MazeGenerator generator = CreateGenerator(algorithm, 0);
            
            // Act
            generator.Generate(map);
            
            // Assert
            for (int x = 0; x < map.Width; x++)
            {
                Assert.AreEqual(TileType.Wall, map[x, 0], $"There is no wall at ({x}, 0).");
                Assert.AreEqual(TileType.Wall, map[x, map.Height - 1], $"There is no wall at ({x}, {map.Height - 1}).");
            }

            for (int y = 0; y < map.Height; y++)
            {
                Assert.AreEqual(TileType.Wall, map[0, y], $"There is no wall at (0, {y}).");
                Assert.AreEqual(TileType.Wall, map[map.Width - 1, y], $"There is no wall at ({map.Width - 1}, {y}).");
            }
        }
        
        [TestCase(MazeAlgorithm.RecursiveBacktracker)]
        [TestCase(MazeAlgorithm.RandomizedPrim)]
        [TestCase(MazeAlgorithm.RandomizedKruskal)]
        public void Generate_WithSameSeed_ProducesSameMap(MazeAlgorithm algorithm)
        {
            // Arrange
            DungeonMap map1 = new DungeonMap(10, 7);
            DungeonMap map2 = new DungeonMap(10, 7);
            int seed = 10764;
            MazeGenerator generator1 = CreateGenerator(algorithm, seed);
            MazeGenerator generator2 = CreateGenerator(algorithm, seed);
            
            // Act
            generator1.Generate(map1);
            generator2.Generate(map2);
            
            // Assert
            Assert.AreEqual(map1.Height, map2.Height);
            Assert.AreEqual(map1.Width, map2.Width);
            for (int x = 0; x < map1.Width; x++)
            {
                for (int y = 0; y < map1.Height; y++)
                {
                    Assert.AreEqual(map1[x, y], map2[x, y], $"The tiles at ({x}, {y}) are not equal with seed {seed}.");
                }
            }
        }
        
        [TestCase(MazeAlgorithm.RecursiveBacktracker)]
        [TestCase(MazeAlgorithm.RandomizedPrim)]
        [TestCase(MazeAlgorithm.RandomizedKruskal)]
        public void Generate_ProducesConnectedMaze(MazeAlgorithm algorithm)
        {
            // Arrange
            DungeonMap map = new DungeonMap(15, 22);
            MazeGenerator generator = CreateGenerator(algorithm, 0);
            
            // Act
            generator.Generate(map);

            // Assert
            int totalFloorCount = CountFloorTiles(map);
            Vector2Int start = FindFloorTile(map);
            int reachableFloorCount = CountReachableFloorTiles(map, start);

            Assert.AreEqual(totalFloorCount, reachableFloorCount,
                $"Expected all {totalFloorCount} floor tiles to be connected, " +
                $"but only {reachableFloorCount} were reachable.");
        }

        [TestCase(MazeAlgorithm.RecursiveBacktracker)]
        [TestCase(MazeAlgorithm.RandomizedPrim)]
        [TestCase(MazeAlgorithm.RandomizedKruskal)]
        public void Generate_ProducesMazeWithoutCycles(MazeAlgorithm algorithm)
        {
            // Arrange
            DungeonMap map = new DungeonMap(27, 19);
            MazeGenerator generator = CreateGenerator(algorithm, 0);

            // Act
            generator.Generate(map);

            // Assert
            int floorCount = CountFloorTiles(map);
            int connectionCount = CountFloorConnections(map);

            Assert.AreEqual(floorCount - 1, connectionCount,
                $"Expected a perfect maze to have {floorCount - 1} connections, " +
                $"but found {connectionCount}.");
        }

        private MazeGenerator CreateGenerator(MazeAlgorithm algorithm, int seed)
        {
            switch (algorithm)
            {
                case MazeAlgorithm.RecursiveBacktracker:
                    return new RecursiveBacktracker(seed);

                case MazeAlgorithm.RandomizedPrim:
                    return new RandomizedPrim(seed);

                case MazeAlgorithm.RandomizedKruskal:
                    return new RandomizedKruskal(seed);

                default:
                    throw new ArgumentOutOfRangeException(nameof(algorithm));
            }
        }
        
        private int CountFloorTiles(DungeonMap map)
        {
            int count = 0;
            
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (map[x, y] == TileType.Floor)
                        count++;
                }
            }
            
            return count;
        }
        
        private Vector2Int FindFloorTile(DungeonMap map)
        {
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (map[x, y] == TileType.Floor)
                        return new Vector2Int(x, y);
                }
            }
            
            throw new InvalidOperationException("Generated maze contains no floor tiles.");
        }
        
        private int CountReachableFloorTiles(DungeonMap map, Vector2Int start)
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            
            Vector2Int[] directions = { Vector2Int.left, Vector2Int.right, 
                Vector2Int.up, Vector2Int.down };
            
            queue.Enqueue(start);
            visited.Add(start);
            
            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                
                foreach (Vector2Int direction in directions)
                {
                    Vector2Int neighbor = current + direction;

                    if (map.IsInside(neighbor.x, neighbor.y)
                        && map[neighbor.x, neighbor.y] == TileType.Floor
                        && !visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
            
            return visited.Count;
        }
        
        private int CountFloorConnections(DungeonMap map)
        {
            int connections = 0;

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (map[x, y] != TileType.Floor)
                        continue;

                    Vector2Int right = new Vector2Int(x + 1, y);
                    Vector2Int up = new Vector2Int(x, y + 1);

                    if (map.IsInside(right.x, right.y)
                        && map[right.x, right.y] == TileType.Floor)
                    {
                        connections++;
                    }

                    if (map.IsInside(up.x, up.y)
                        && map[up.x, up.y] == TileType.Floor)
                    {
                        connections++;
                    }
                }
            }

            return connections;
        }
    }
}