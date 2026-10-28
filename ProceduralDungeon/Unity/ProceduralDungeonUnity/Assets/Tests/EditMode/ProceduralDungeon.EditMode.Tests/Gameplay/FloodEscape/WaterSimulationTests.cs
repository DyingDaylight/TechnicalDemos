using System;
using Core;
using Gameplay.FloodEscape.Water;
using NUnit.Framework;
using UnityEngine;
using DebugTools;

namespace Tests.Gameplay.FloodEscape
{
    public class WaterSimulationTests
    {
        private IReadOnlyDungeonMap map;
        private Vector2Int waterSource;

        [SetUp]
        public void SetUp()
        {
            map = DebugDungeonFactory.CreateFromAscii(
                "#####\n" +
                "#####\n" +
                "#####\n" +
                "##.##\n" +
                "##S.#", 
                out Vector2Int? ws);
            
            if (!ws.HasValue)
                Assert.Fail();
            
            waterSource = ws.Value;
        }
        
        [Test]
        public void Constructor_InitializesWaterSource()
        {
            WaterSimulation simulation = new WaterSimulation(map, waterSource.x, 
                waterSource.y, 0.1f);

            Assert.AreEqual(5, simulation.Width);
            Assert.AreEqual(5, simulation.Height);

            Assert.AreEqual(0f, simulation[waterSource.x, waterSource.y]);

            Assert.AreEqual(1, simulation.Fronts.Count);
            Assert.AreEqual(waterSource, simulation.Fronts[0].EntryPoint);
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
            {
                new WaterSimulation(map, waterSource.x, waterSource.y, -0.1f);
            });
        }

        [Test]
        public void Step_AddsWaterToSource()
        {
            WaterSimulation simulation = new WaterSimulation(map, waterSource.x, waterSource.y, 0.1f);

            simulation.Step();

            Assert.That(simulation[waterSource.x, waterSource.y],
                Is.EqualTo(0.1f).Within(0.0001f));
        }
        
        [Test]
        public void Step_AddsExpectedTotalWater()
        {
            WaterSimulation simulation = new WaterSimulation(map, waterSource.x, waterSource.y, 0.1f);

            for (int i = 0; i < 5; i++)
                simulation.Step();

            Assert.That(GetTotalWater(simulation),
                Is.EqualTo(0.5f).Within(0.0001f));
        }

        [Test]
        public void Step_HorizontalFlowPreservesWaterVolume()
        {
            WaterSimulation simulation = new WaterSimulation(map, waterSource.x, waterSource.y, 0f);

            simulation[waterSource.x, waterSource.y] = 0.8f;

            simulation.Step();

            Assert.That(GetTotalWater(simulation),
                Is.EqualTo(0.8f).Within(0.0001f));
            
            Assert.That(simulation[2, 0], Is.LessThan(0.8f));
            Assert.That(simulation[3, 0], Is.GreaterThan(0f));
        }

        [Test]
        public void Step_WaterLevelsStayWithinBounds()
        {
            WaterSimulation simulation = new WaterSimulation(map, waterSource.x, waterSource.y, 0.1f);

            for (var i = 0; i < 100; i++)
                simulation.Step();

            for (var x = 0; x < simulation.Width; x++)
            {
                for (var y = 0; y < simulation.Height; y++)
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
            WaterSimulation simulation = new WaterSimulation(map, waterSource.x, waterSource.y, 0f);

            simulation[waterSource.x, waterSource.y] = 0.8f;
            
            simulation.Step();

            Assert.That(simulation[2, 0], Is.EqualTo(0.4f).Within(0.0001f));
            Assert.That(simulation[3, 0], Is.EqualTo(0.4f).Within(0.0001f));
        }
        
        [Test]
        public void Step_GravityMovesWaterDown()
        {
            IReadOnlyDungeonMap testMap = DebugDungeonFactory.CreateFromAscii(
                "###\n" +
                "#S#\n" +
                "#.#\n",
                out Vector2Int? source);

            WaterSimulation simulation = new WaterSimulation(
                testMap, source.Value.x, source.Value.y, 0f);

            simulation[source.Value.x, source.Value.y] = 0.8f;

            simulation.Step();

            Assert.That(simulation[1, 0], Is.EqualTo(0.8f).Within(0.0001f));
            Assert.That(simulation[1, 1], Is.EqualTo(0f).Within(0.0001f));
        }
        
        [Test]
        public void Step_WaterDoesNotFlowThroughWall()
        {
            IReadOnlyDungeonMap testMap = DebugDungeonFactory.CreateFromAscii(
                "#####\n" +
                "#####\n" +
                "#S#.#\n",
                out Vector2Int? source);

            WaterSimulation simulation = new WaterSimulation(
                testMap, source.Value.x, source.Value.y, 0f);

            simulation[source.Value.x, source.Value.y] = 0.8f;

            simulation.Step();

            Assert.That(simulation[1, 0], Is.EqualTo(0.8f).Within(0.0001f));
            Assert.That(simulation[2, 0], Is.EqualTo(0f));
            Assert.That(simulation[3, 0], Is.EqualTo(0f));
        }
        
        [Test]
        public void Step_RecordsHorizontalFlowDirectionFromRight()
        {
            IReadOnlyDungeonMap testMap = DebugDungeonFactory.CreateFromAscii(
                "###\n" +
                "###\n" +
                "#.S\n",
                out Vector2Int? source);

            WaterSimulation simulation = new WaterSimulation(
                testMap, source.Value.x, source.Value.y, 0f);

            simulation[source.Value.x, source.Value.y] = 1f;

            simulation.Step();

            Assert.That(simulation[1, 0], Is.EqualTo(0.5f).Within(0.0001f));
            Assert.That(simulation[2, 0], Is.EqualTo(0.5f).Within(0.0001f));

            Assert.That(
                simulation.GetFallDirection(1, 0),
                Is.EqualTo(WaterFallDirection.FromRight));
            
            Assert.That(
                simulation.GetFallDirection(2, 0),
                Is.EqualTo(WaterFallDirection.None));
        }
        
        [Test]
        public void Step_RecordsHorizontalFlowDirectionFromLeft()
        {
            IReadOnlyDungeonMap testMap = DebugDungeonFactory.CreateFromAscii(
                "###\n" +
                "###\n" +
                "#S.\n",
                out Vector2Int? source);

            WaterSimulation simulation = new WaterSimulation(
                testMap, source.Value.x, source.Value.y, 0f);

            simulation[source.Value.x, source.Value.y] = 1f;

            simulation.Step();

            Assert.That(simulation[1, 0], Is.EqualTo(0.5f).Within(0.0001f));
            Assert.That(simulation[2, 0], Is.EqualTo(0.5f).Within(0.0001f));

            Assert.That(
                simulation.GetFallDirection(2, 0),
                Is.EqualTo(WaterFallDirection.FromLeft));
            
            Assert.That(
                simulation.GetFallDirection(1, 0),
                Is.EqualTo(WaterFallDirection.None));
        }
        
        [Test]
        public void Step_PreservesFallDirectionWhenWaterFallsFromLeft()
        {
            IReadOnlyDungeonMap testMap = DebugDungeonFactory.CreateFromAscii(
                "###\n" +
                "S.#\n" +
                "#.#",
                out Vector2Int? source);

            WaterSimulation simulation = new WaterSimulation(
                testMap, source.Value.x, source.Value.y, 0f);

            simulation[source.Value.x, source.Value.y] = 0.5f;
            
            simulation.Step();

            Assert.That(
                simulation.GetFallDirection(1, 1),
                Is.EqualTo(WaterFallDirection.FromLeft));
            
            Assert.That(simulation[1, 0], Is.GreaterThan(0f));
        }
        
        [Test]
        public void Step_PreservesFallDirectionWhenWaterFallsFromRight()
        {
            IReadOnlyDungeonMap testMap = DebugDungeonFactory.CreateFromAscii(
                "###\n" +
                ".S#\n" +
                ".##",
                out Vector2Int? source);

            WaterSimulation simulation = new WaterSimulation(
                testMap, source.Value.x, source.Value.y, 0f);

            simulation[source.Value.x, source.Value.y] = 0.5f;
            
            simulation.Step();

            Assert.That(
                simulation.GetFallDirection(0, 1),
                Is.EqualTo(WaterFallDirection.FromRight));
            
            Assert.That(simulation[0, 0], Is.GreaterThan(0f));
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