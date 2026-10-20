using System;
using Core;
using Gameplay.FloodEscape.Water;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Gameplay.FloodEscape
{
    public class WaterSimulationTests
    {
        private DungeonMap map;

        [SetUp]
        public void SetUp()
        {
            map = new DungeonMap(5, 5);
            map[2, 0] = TileType.Floor;
            map[2, 1] = TileType.Floor;
            map[3, 0] = TileType.Floor;
        }
        
        [Test]
        public void Constructor_InitializesWaterSource()
        {
            WaterSimulation simulation = new WaterSimulation(map, 2, 0, 0.1f);

            Assert.AreEqual(5, simulation.Width);
            Assert.AreEqual(5, simulation.Height);

            Assert.AreEqual(0f, simulation[2, 0]);

            Assert.AreEqual(1, simulation.Fronts.Count);
            Assert.AreEqual(new Vector2Int(2, 0), simulation.Fronts[0].EntryPoint);
            Assert.IsFalse(simulation.Fronts[0].IncomingFall.HasValue);
        }
        
        [Test]
        public void Constructor_ThrowsForInvalidSource()
        {
            Assert.Throws<ArgumentException>(() =>
                new WaterSimulation(map, 0, 0, 0.1f));

            Assert.Throws<ArgumentException>(() =>
                new WaterSimulation(map, -1, 0, 0.1f));
        }

        [Test]
        public void Constructor_ThrowsForNegativeInflow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new WaterSimulation(map, 2, 0, -0.1f));
        }

        [Test]
        public void Step_AddsWaterToSource()
        {
            WaterSimulation simulation = new WaterSimulation(map, 2, 0, 0.1f);

            simulation.Step();

            Assert.That(simulation[2, 0], Is.EqualTo(0.1f).Within(0.0001f));
        }
        
        [Test]
        public void Step_AddsExpectedTotalWater()
        {
            WaterSimulation simulation = new WaterSimulation(map, 2, 0, 0.1f);

            for (int i = 0; i < 5; i++)
                simulation.Step();

            Assert.That(GetTotalWater(simulation),
                Is.EqualTo(0.5f).Within(0.0001f));
        }

        [Test]
        public void Step_HorizontalFlowPreservesWaterVolume()
        {
            WaterSimulation simulation = new WaterSimulation(map, 2, 0, 0f);

            simulation[2, 0] = 0.8f;

            simulation.Step();

            Assert.That(GetTotalWater(simulation),
                Is.EqualTo(0.8f).Within(0.0001f));
        }

        [Test]
        public void Step_WaterLevelsStayWithinBounds()
        {
            WaterSimulation simulation = new WaterSimulation(map, 2, 0, 0.1f);

            for (int i = 0; i < 100; i++)
                simulation.Step();

            for (int x = 0; x < simulation.Width; x++)
            {
                for (int y = 0; y < simulation.Height; y++)
                {
                    Assert.That(simulation[x, y],
                        Is.InRange(0f, 1f),
                        $"Invalid water level at ({x}, {y})");
                }
            }
        }
        
        [Test]
        public void Step_HorizontalFlowEqualizesAdjacentCells()
        {
            DungeonMap testMap = new DungeonMap(3, 3);
            testMap[0, 0] = TileType.Floor;
            testMap[1, 0] = TileType.Floor;

            WaterSimulation simulation = new WaterSimulation(testMap, 0, 0, 0f);
            simulation[0, 0] = 0.8f;

            simulation.Step();

            Assert.That(simulation[0, 0], Is.EqualTo(0.4f).Within(0.0001f));
            Assert.That(simulation[1, 0], Is.EqualTo(0.4f).Within(0.0001f));
        }
        
        [Test]
        public void Step_GravityMovesWaterDown()
        {
            DungeonMap testMap = new DungeonMap(3, 3);
            testMap[1, 0] = TileType.Floor;
            testMap[1, 1] = TileType.Floor;

            WaterSimulation simulation = new WaterSimulation(testMap, 1, 0, 0f);
            simulation[1, 1] = 0.8f;

            simulation.Step();

            Assert.That(simulation[1, 0], Is.EqualTo(0.8f).Within(0.0001f));
            Assert.That(simulation[1, 1], Is.EqualTo(0f).Within(0.0001f));
        }
        
        [Test]
        public void Step_WaterDoesNotFlowThroughWall()
        {
            DungeonMap testMap = new DungeonMap(5, 3);
            testMap[1, 0] = TileType.Floor;
            testMap[3, 0] = TileType.Floor;

            WaterSimulation simulation = new WaterSimulation(testMap, 1, 0, 0f);
            simulation[1, 0] = 0.8f;

            simulation.Step();

            Assert.That(simulation[1, 0], Is.EqualTo(0.8f).Within(0.0001f));
            Assert.That(simulation[2, 0], Is.EqualTo(0f));
            Assert.That(simulation[3, 0], Is.EqualTo(0f));
        }
        
        private float GetTotalWater(WaterSimulation simulation)
        {
            float total = 0f;

            for (int x = 0; x < simulation.Width; x++)
            {
                for (int y = 0; y < simulation.Height; y++)
                {
                    total += simulation[x, y];
                }
            }

            return total;
        }
    }
}