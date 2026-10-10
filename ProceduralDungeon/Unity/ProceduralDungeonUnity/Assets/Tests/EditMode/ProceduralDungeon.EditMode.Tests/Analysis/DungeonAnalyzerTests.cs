using System;
using Analysis;
using Core;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Analysis
{
    public class DungeonAnalyzerTests
    {
        [Test]
        public void FindDeadEnds_StraightCorridor_ReturnsTwoDeadEnds()
        {
            DungeonMap map = new DungeonMap(5, 5);

            map[2, 1] = TileType.Floor;
            map[2, 2] = TileType.Floor;
            map[2, 3] = TileType.Floor;

            var deadEnds = DungeonAnalyzer.FindDeadEnds(map);

            Assert.AreEqual(2, deadEnds.Count);
            CollectionAssert.AreEquivalent(
                new[]
                {
                    new Vector2Int(2, 1),
                    new Vector2Int(2, 3)
                },
                deadEnds
            );
        }

        [Test]
        public void FindDeadEnds_TShapedCorridor_ReturnsThreeDeadEnds()
        {
            DungeonMap map = new DungeonMap(5, 5);
            
            map[1, 3] = TileType.Floor;
            map[2, 3] = TileType.Floor;
            map[3, 3] = TileType.Floor;
            map[2, 2] = TileType.Floor;
            map[2, 1] = TileType.Floor;

            var deadEnds = DungeonAnalyzer.FindDeadEnds(map);

            Assert.AreEqual(3, deadEnds.Count);
            CollectionAssert.AreEquivalent(
                new[]
                {
                    new Vector2Int(2, 1),
                    new Vector2Int(1, 3),
                    new Vector2Int(3, 3)
                },
                deadEnds
            );
        }
        
        [Test]
        public void FindDeadEnds_LoopCorridor_ReturnsNoDeadEnds()
        {
            DungeonMap map = new DungeonMap(5, 5);
            
            for (int x = 1; x <= 3; x++)
            {
                map[x, 1] = TileType.Floor;
                map[x, 3] = TileType.Floor;
            }

            map[1, 2] = TileType.Floor;
            map[3, 2] = TileType.Floor;

            var deadEnds = DungeonAnalyzer.FindDeadEnds(map);

            Assert.AreEqual(0, deadEnds.Count);
        }
        
        [Test]
        public void FindDeadEnds_CorridorAtMapBoundary_ReturnsTwoDeadEnds()
        {
            DungeonMap map = new DungeonMap(5, 5);
            
            map[0, 2] = TileType.Floor;
            map[1, 2] = TileType.Floor;
            map[2, 2] = TileType.Floor;
            
            var deadEnds = DungeonAnalyzer.FindDeadEnds(map);

            Assert.AreEqual(2, deadEnds.Count);
            CollectionAssert.AreEquivalent(
                new[]
                {
                    new Vector2Int(0, 2),
                    new Vector2Int(2, 2)
                },
                deadEnds
            );
        }
        
        [Test]
        public void GetDistances_StraightCorridor_ReturnsCorrectDistances()
        {
            DungeonMap map = new DungeonMap(5, 5);

            map[2, 1] = TileType.Floor;
            map[2, 2] = TileType.Floor;
            map[2, 3] = TileType.Floor;

            int[,] distances = DungeonAnalyzer.GetDistances(map, new Vector2Int(2, 1));

            Assert.AreEqual(0, distances[2, 1]);
            Assert.AreEqual(1, distances[2, 2]);
            Assert.AreEqual(2, distances[2, 3]);
            Assert.AreEqual(-1, distances[1, 1]);
        }
        
        [Test]
        public void GetDistances_TShapedCorridor_ReturnsCorrectDistances()
        {
            DungeonMap map = new DungeonMap(5, 5);

            map[1, 3] = TileType.Floor;
            map[2, 3] = TileType.Floor;
            map[3, 3] = TileType.Floor;
            map[2, 2] = TileType.Floor;
            map[2, 1] = TileType.Floor;

            int[,] distances = DungeonAnalyzer.GetDistances(map, new Vector2Int(2, 1));

            Assert.AreEqual(0, distances[2, 1]);
            Assert.AreEqual(1, distances[2, 2]);
            Assert.AreEqual(2, distances[2, 3]);
            Assert.AreEqual(3, distances[1, 3]);
            Assert.AreEqual(3, distances[3, 3]);
        }
        
        [Test]
        public void GetDistances_DisconnectedAreas_MarksUnreachableCells()
        {
            DungeonMap map = new DungeonMap(5, 5);

            map[1, 1] = TileType.Floor;
            map[1, 2] = TileType.Floor;

            map[3, 2] = TileType.Floor;
            map[3, 3] = TileType.Floor;

            int[,] distances = DungeonAnalyzer.GetDistances(map, new Vector2Int(1, 1));

            Assert.AreEqual(0, distances[1, 1]);
            Assert.AreEqual(1, distances[1, 2]);
            Assert.AreEqual(-1, distances[3, 2]);
            Assert.AreEqual(-1, distances[3, 3]);
        }
        
        [Test]
        public void GetDistances_MultipleRoutes_ReturnsShortestDistance()
        {
            DungeonMap map = new DungeonMap(7, 5);

            for (int x = 1; x <= 5; x++)
            {
                map[x, 1] = TileType.Floor;
                map[x, 3] = TileType.Floor;
            }

            map[1, 2] = TileType.Floor;
            map[5, 2] = TileType.Floor;

            int[,] distances = DungeonAnalyzer.GetDistances(
                map, new Vector2Int(1, 1));

            Assert.AreEqual(0, distances[1, 1]);
            Assert.AreEqual(4, distances[5, 1]);
            Assert.AreEqual(6, distances[5, 3]);
            Assert.AreEqual(4, distances[3, 3]);
        }
        
        [Test]
        public void GetDistances_StartOutsideMap_ThrowsArgumentException()
        {
            DungeonMap map = new DungeonMap(5, 5);

            Assert.Throws<ArgumentException>(() =>
                DungeonAnalyzer.GetDistances(map, new Vector2Int(-1, 2)));
        }

        [Test]
        public void GetDistances_StartOnWall_ThrowsArgumentException()
        {
            DungeonMap map = new DungeonMap(5, 5);
            
            Assert.Throws<ArgumentException>(() =>
                DungeonAnalyzer.GetDistances(map, new Vector2Int(2, 2)));
        }
    }
}