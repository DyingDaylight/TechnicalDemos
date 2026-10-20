using System;
using Core;
using Gameplay.FloodEscape;
using Gameplay.FloodEscape.Generation;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Gameplay.FloodEscape
{
    public class DiamondPlacementTests
    {
        [TestCase(0.5f, 4, 2)]
        [TestCase(1.0f, 1, 4)]
        [TestCase(0.9f, 1, 4)]
        public void FindPosition_SelectsDeadEndClosestToTargetDetour(
            float targetDetourRatio, int expectedX, int expectedY)
        {
            DungeonMap map = new DungeonMap(7, 7);

            // Main path from entrance to exit.
            for (int y = 1; y <= 5; y++)
                map[3, y] = TileType.Floor;

            // Short branch: adds 2 steps to the route.
            map[4, 2] = TileType.Floor;

            // Long branch: adds 4 steps to the route.
            map[2, 4] = TileType.Floor;
            map[1, 4] = TileType.Floor;
            
            Vector2Int entrance = new Vector2Int(3, 1);
            Vector2Int exit = new Vector2Int(3, 5);

            Vector2Int position = DiamondPlacement.FindPosition(
                map, entrance, exit, targetDetourRatio);

            Assert.AreEqual(new Vector2Int(expectedX, expectedY), position);
        }
        
        [Test]
        public void FindPosition_ExcludesEntranceAndExit()
        {
            DungeonMap map = new DungeonMap(5, 5);

            for (int y = 1; y <= 3; y++)
                map[2, y] = TileType.Floor;

            map[3, 2] = TileType.Floor;

            Vector2Int position = DiamondPlacement.FindPosition(
                map,
                new Vector2Int(2, 1),
                new Vector2Int(2, 3),
                0.0f);

            Assert.AreEqual(new Vector2Int(3, 2), position);
        }
        
        [Test]
        public void FindPosition_IgnoresUnreachableDeadEnds()
        {
            DungeonMap map = new DungeonMap(7, 7);

            for (int y = 1; y <= 5; y++)
                map[3, y] = TileType.Floor;

            map[4, 3] = TileType.Floor;

            // Disconnected corridor.
            map[1, 1] = TileType.Floor;
            map[1, 2] = TileType.Floor;

            Vector2Int position = DiamondPlacement.FindPosition(
                map,
                new Vector2Int(3, 1),
                new Vector2Int(3, 5),
                0.5f);

            Assert.AreEqual(new Vector2Int(4, 3), position);
        }
        
        [Test]
        public void FindPosition_NoAvailableDeadEnds_ThrowsInvalidOperationException()
        {
            DungeonMap map = new DungeonMap(5, 5);

            for (int y = 1; y <= 3; y++)
                map[2, y] = TileType.Floor;
            
            Assert.Throws<InvalidOperationException>(() =>
                DiamondPlacement.FindPosition(
                    map,
                    new Vector2Int(2, 1),
                    new Vector2Int(2, 3),
                    0.5f));
        }
        
        [Test]
        public void FindPosition_PrefersDeadEndCloserToVerticalCenter()
        {
            DungeonMap map = new DungeonMap(9, 9);

            for (int y = 1; y <= 7; y++)
                map[4, y] = TileType.Floor;

            map[5, 2] = TileType.Floor;
            map[5, 4] = TileType.Floor;

            Vector2Int position = DiamondPlacement.FindPosition(
                map,
                new Vector2Int(4, 1),
                new Vector2Int(4, 7),
                1f / 3f);

            Assert.AreEqual(new Vector2Int(5, 4), position);
        }
        
        [Test]
        public void FindPosition_PrefersDeadEndCloserToSideEdge()
        {
            // Both dead ends have the same detour and vertical preference.
            // The less favorable candidate is visited first by FindDeadEnds,
            // so the test verifies that edge proximity overrides iteration order.

            DungeonMap map = new DungeonMap(11, 11);

            // Main path.
            for (int y = 1; y <= 9; y++)
                map[7, y] = TileType.Floor;

            // Inner dead end at (9, 4).
            map[8, 4] = TileType.Floor;
            map[9, 4] = TileType.Floor;

            // Edge dead end at (5, 6).
            map[6, 6] = TileType.Floor;
            map[5, 6] = TileType.Floor;

            Vector2Int position = DiamondPlacement.FindPosition(
                map,
                new Vector2Int(7, 1),
                new Vector2Int(7, 9),
                0.5f);

            Assert.AreEqual(new Vector2Int(9, 4), position);
        }
        
        [Test]
        public void FindPosition_UnreachableExit_ThrowsArgumentException()
        {
            DungeonMap map = new DungeonMap(5, 5);

            // Entrance and exit belong to disconnected floor regions.
            map[1, 1] = TileType.Floor;
            map[1, 2] = TileType.Floor;

            map[3, 2] = TileType.Floor;
            map[3, 3] = TileType.Floor;

            Vector2Int entrance = new Vector2Int(1, 1);
            Vector2Int exit = new Vector2Int(3, 3);

            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                DiamondPlacement.FindPosition(map, entrance, exit, 0.5f));

            Assert.AreEqual("exit", exception.ParamName);
        }
    }
}