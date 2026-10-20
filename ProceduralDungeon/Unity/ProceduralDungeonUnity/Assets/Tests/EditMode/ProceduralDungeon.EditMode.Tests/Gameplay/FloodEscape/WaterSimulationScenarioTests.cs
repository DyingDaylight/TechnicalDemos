using System.Linq;
using Core;
using Gameplay.FloodEscape.Water;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Gameplay.FloodEscape
{
    public class WaterSimulationScenarioTests
    {
        // Source at the bottom, water distributes horizontally and raises vertically
        
         /*
         * Tests that water spreads horizontally from a bottom source
         * and rises through two vertical shafts simultaneously.
         *
         * Map layout (# = Wall, . = Floor, S = Source):
         *
         *        x  0 1 2 3 4 5 6 7 8
         *
         * y = 4     # # # # # # # # #
         * y = 3     # . # # # . # # #
         * y = 2     # . # # # . # # #
         * y = 1     # . # # # . # # #
         * y = 0     # . . . S . . . #
         *
         * Water should fill the bottom corridor and begin rising
         * through both shafts.
         */
        [Test]
        public void Step_RisesThroughMultipleShaftsSimultaneously()
        {
            DungeonMap testMap = new DungeonMap(9, 5);

            for (int x = 1; x <= 7; x++)
                testMap[x, 0] = TileType.Floor;

            for (int y = 1; y <= 3; y++)
            {
                testMap[1, y] = TileType.Floor;
                testMap[5, y] = TileType.Floor;
            }

            WaterSimulation simulation = new WaterSimulation(testMap, 4, 0, 0.5f);

            for (int i = 0; i < 24; i++)
                simulation.Step();

            Assert.Greater(simulation[1, 1], 0f);
            Assert.Greater(simulation[5, 1], 0f);
        }
        
        /*
         * Tests that water rises through a vertical shaft
         * and spreads horizontally along an upper corridor.
         *
         * Map layout (# = Wall, . = Floor, S = Source):
         *
         *        x  0 1 2 3 4 5 6
         *
         * y = 4     # . . . . . #
         * y = 3     # # # . # # #
         * y = 2     # # # . # # #
         * y = 1     # # # . # # #
         * y = 0     # # # S # # #
         *
         * Water rises from (3, 0) to (3, 4), then spreads
         * in both directions along the upper corridor.
         */
        [Test]
        public void Step_RisesThroughShaftAndSpreadsHorizontally()
        {
            DungeonMap testMap = new DungeonMap(7, 5);

            for (int y = 0; y <= 4; y++)
                testMap[3, y] = TileType.Floor;

            for (int x = 1; x <= 5; x++)
                testMap[x, 4] = TileType.Floor;

            WaterSimulation simulation = new WaterSimulation(testMap, 3, 0, 0.5f);

            for (int i = 0; i < 16; i++)
                simulation.Step();

            Assert.Greater(simulation[1, 4], 0f);
            Assert.Greater(simulation[5, 4], 0f);
        }
        
        /*
         * Tests water rising through two shafts of different heights.
         *
         * Map layout (# = Wall, . = Floor, S = Source):
         *
         *        x  0 1 2 3 4 5 6
         *
         * y = 4     # # # # # . #
         * y = 3     # # # # # . #
         * y = 2     # . # # # . #
         * y = 1     # . # # # . #
         * y = 0     # . . S . . #
         *
         * The shorter shaft should fill completely while water
         * continues rising through the taller shaft.
         */
        [Test]
        public void Step_FillsShaftsOfDifferentHeights()
        {
            DungeonMap testMap = new DungeonMap(7, 5);

            for (int x = 1; x <= 5; x++)
                testMap[x, 0] = TileType.Floor;

            for (int y = 1; y <= 2; y++)
                testMap[1, y] = TileType.Floor;

            for (int y = 1; y <= 4; y++)
                testMap[5, y] = TileType.Floor;

            WaterSimulation simulation = new WaterSimulation(testMap, 3, 0, 0.5f);

            for (int i = 0; i < 18; i++)
                simulation.Step();

            Assert.That(simulation[1, 2], Is.EqualTo(1f).Within(0.0001f));
            Assert.Greater(simulation[5, 3], 0f);
        }
        
        /*
         * Tests water rising through two shafts and meeting
         * in a shared upper corridor.
         *
         * Map layout (# = Wall, . = Floor, S = Source):
         *
         *        x  0 1 2 3 4 5 6 7 8
         *
         * y = 4     # . . . . . . . #
         * y = 3     # . # # # # # . #
         * y = 2     # . # # # # # . #
         * y = 1     # . # # # # # . #
         * y = 0     # . . . S . . . #
         *
         * Water rises through both shafts, spreads toward
         * the center of the upper corridor, and meets there.
         */
        [Test]
        public void Step_MergingFlowsFillSharedUpperCorridor()
        {
            DungeonMap testMap = new DungeonMap(9, 5);

            for (int x = 1; x <= 7; x++)
            {
                testMap[x, 0] = TileType.Floor;
                testMap[x, 4] = TileType.Floor;
            }

            for (int y = 1; y <= 3; y++)
            {
                testMap[1, y] = TileType.Floor;
                testMap[7, y] = TileType.Floor;
            }

            WaterSimulation simulation = new WaterSimulation(testMap, 4, 0, 0.5f);

            for (int i = 0; i < 38; i++)
                simulation.Step();

            Assert.Greater(simulation[3, 4], 0f);
            Assert.Greater(simulation[4, 4], 0f);
            Assert.Greater(simulation[5, 4], 0f);
        }
        
        
        // Source at the top
        
        /*
         * Tests water flowing horizontally along a corridor,
         * reaching a ledge, and creating a new water front below.
         *
         * Map layout (# = Wall, . = Floor, S = Source):
         *
         *        x  0 1 2 3 4 5 6
         *
         * y = 4     # # # # # # #
         * y = 3     # S . . . . #
         * y = 2     # # # # . # #
         * y = 1     # # # # . # #
         * y = 0     # # # . . . #
         *
         * Expected fall:
         *   Opening:     (4, 3)
         *   Destination: (4, 0)
         *
         * Water must travel horizontally from (1, 3) to (4, 3)
         * before discovering the vertical drop.
         */
        [Test]
        public void Step_CreatesNewFrontAfterHorizontalFlowToLedge()
        {
            DungeonMap testMap = new DungeonMap(7, 5);

            for (int x = 1; x <= 5; x++)
                testMap[x, 3] = TileType.Floor;

            testMap[4, 2] = TileType.Floor;
            testMap[4, 1] = TileType.Floor;

            for (int x = 3; x <= 5; x++)
                testMap[x, 0] = TileType.Floor;

            WaterSimulation simulation = new WaterSimulation(testMap, 1, 3, 0.1f);

            for (int i = 0; i < 30; i++)
                simulation.Step();

            WaterFront lowerFront = simulation.Fronts
                .SingleOrDefault(front => front.EntryPoint == new Vector2Int(4, 0));

            Assert.IsNotNull(lowerFront);
            Assert.IsTrue(lowerFront.IncomingFall.HasValue);

            WaterFall fall = lowerFront.IncomingFall.Value;

            Assert.AreEqual(new Vector2Int(4, 3), fall.Opening);
            Assert.AreEqual(new Vector2Int(4, 0), fall.Destination);
        }
        
        /*
         * Tests that water prioritizes the lower reservoir after
         * flowing horizontally to a ledge.
         *
         * Map layout (# = Wall, . = Floor, S = Source):
         *
         *        x  0 1 2 3 4 5 6
         *
         * y = 4     # # # # # # #
         * y = 3     # S . . . . #
         * y = 2     # # # # . # #
         * y = 1     # # # # . # #
         * y = 0     # # # . . . #
         *
         * Water should reach the ledge at (4, 3), fall to (4, 0),
         * and begin filling the lower reservoir.
         */
        [Test]
        public void Step_PrioritizesLowerReservoirAfterFall()
        {
            DungeonMap testMap = new DungeonMap(7, 5);

            for (int x = 1; x <= 5; x++)
                testMap[x, 3] = TileType.Floor;

            testMap[4, 2] = TileType.Floor;
            testMap[4, 1] = TileType.Floor;

            for (int x = 3; x <= 5; x++)
                testMap[x, 0] = TileType.Floor;

            WaterSimulation simulation = new WaterSimulation(testMap, 1, 3, 0.5f);

            for (int i = 0; i < 10; i++)
                simulation.Step();

            Assert.Greater(simulation[4, 0], 0f);
            Assert.Greater(simulation[3, 0] + simulation[5, 0], 0f);
        }
        
        /*
         * Tests water rising into a side opening after filling
         * a lower reservoir.
         *
         * Map layout (# = Wall, . = Floor, S = Source):
         *
         *        x  0 1 2 3 4 5 6 7 8
         *
         * y = 4     # # # # # # # # #
         * y = 3     # S . . . . # # #
         * y = 2     # # # # . # # # #
         * y = 1     # # # # . # . # #
         * y = 0     # # # . . . . # #
         *
         * Water flows along the upper corridor, falls at (4, 3),
         * fills the lower reservoir, then rises into (6, 1).
         */
        [Test]
        public void Step_FillsLowerReservoirAndRisesIntoSideOpening()
        {
            DungeonMap testMap = new DungeonMap(9, 5);

            for (int x = 1; x <= 5; x++)
                testMap[x, 3] = TileType.Floor;

            testMap[4, 2] = TileType.Floor;
            testMap[4, 1] = TileType.Floor;
            testMap[6, 1] = TileType.Floor;

            for (int x = 3; x <= 6; x++)
                testMap[x, 0] = TileType.Floor;

            WaterSimulation simulation = new WaterSimulation(testMap, 1, 3, 0.5f);

            for (int i = 0; i < 30; i++)
                simulation.Step();

            Assert.Greater(simulation[6, 1], 0f);
        }
        
        /*
         * Tests that water falling into a vertical shaft
         * accumulates at the bottom and fills the shaft upward.
         *
         * Map layout (# = Wall, . = Floor, S = Source):
         *
         *        x  0 1 2 3 4
         *
         * y = 4     # S . # #
         * y = 3     # # . # #
         * y = 2     # # . # #
         * y = 1     # # . # #
         * y = 0     # # . # #
         *
         * Water flows from the source to (2, 4), falls down
         * the shaft, and gradually fills it from bottom to top.
         */
        [Test]
        public void Step_FillsVerticalShaftFromBottomUp()
        {
            DungeonMap testMap = new DungeonMap(5, 5);

            testMap[1, 4] = TileType.Floor;

            for (int y = 0; y <= 4; y++)
                testMap[2, y] = TileType.Floor;

            WaterSimulation simulation = new WaterSimulation(testMap, 1, 4, 0.5f);

            for (int i = 0; i < 6; i++)
                simulation.Step();

            Assert.AreEqual(1f, simulation[2, 0], 0.0001f);
            Assert.AreEqual(1f, simulation[2, 1], 0.0001f);
            Assert.Greater(simulation[2, 2], 0f);
        }
        
        /*
         * Tests that water falls into a lower reservoir without
         * prematurely rising through a neighboring shaft.
         *
         * Map layout (# = Wall, . = Floor, S = Source):
         *
         *        x  0 1 2 3 4 5 6 7 8
         *
         * y = 4     # # # # # # # # #
         * y = 3     # S . . . . # # #
         * y = 2     # # # # . # # # #
         * y = 1     # # # # . # . # #
         * y = 0     # # # . . . . # #
         *
         * Water falls through (4, 3) into the lower reservoir.
         * Before the lower reservoir fills, water must not
         * rise through the neighboring shaft at (6, 1).
         */
        
        [Test]
        public void Step_DoesNotRiseThroughSideOpeningBeforeReservoirFills()
        {
            DungeonMap testMap = new DungeonMap(9, 5);

            for (int x = 1; x <= 5; x++)
                testMap[x, 3] = TileType.Floor;

            testMap[4, 2] = TileType.Floor;
            testMap[4, 1] = TileType.Floor;
            testMap[6, 1] = TileType.Floor;

            for (int x = 3; x <= 6; x++)
                testMap[x, 0] = TileType.Floor;

            WaterSimulation simulation = new WaterSimulation(testMap, 1, 3, 0.5f);

            for (int i = 0; i < 8; i++)
                simulation.Step();

            Assert.Greater(simulation[4, 0], 0f);
            Assert.That(simulation[6, 1], Is.EqualTo(0f).Within(0.0001f));
        }
        
        /*
         * Tests that water can rise above its original falling level
         * after filling a lower reservoir.
         *
         * Map layout (# = Wall, . = Floor, S = Source):
         *
         *        x  0 1 2 3 4 5 6 7 8
         *
         * y = 4     # # # # # # . # #
         * y = 3     # S . . . # . # #
         * y = 2     # # # # . # . # #
         * y = 1     # # # # . # . # #
         * y = 0     # # # . . . . # #
         *
         * Water falls through (4, 3), fills the lower reservoir,
         * and rises through the shaft at (6, 0) to (6, 4).
         */
        [Test]
        public void Step_RisesAboveOriginalFallLevel()
        {
            DungeonMap testMap = new DungeonMap(9, 5);

            for (int x = 1; x <= 4; x++)
                testMap[x, 3] = TileType.Floor;

            for (int y = 0; y <= 3; y++)
                testMap[4, y] = TileType.Floor;

            for (int x = 3; x <= 6; x++)
                testMap[x, 0] = TileType.Floor;

            for (int y = 1; y <= 4; y++)
                testMap[6, y] = TileType.Floor;

            WaterSimulation simulation = new WaterSimulation(testMap, 1, 3, 0.5f);

            for (int i = 0; i < 40; i++)
                simulation.Step();

            Assert.Greater(simulation[6, 4], 0f);
        }
        
        /*
         * Tests water falling through a central shaft and rising
         * through three separate shafts connected to the lower reservoir.
         *
         * Map layout (# = Wall, . = Floor, S = Source):
         *
         *        x  0 1 2 3 4 5 6 7 8 9 10
         *
         * y = 4     # # # # S . # # # # #
         * y = 3     # # # # # . # # # # #
         * y = 2     # . # . # . # . # # #
         * y = 1     # . # . # . # . # # #
         * y = 0     # . . . . . . . . # #
         *
         * Water flows from (4, 4) to (5, 4), falls to (5, 0),
         * spreads across the lower reservoir, and rises through
         * the shafts at x = 1, x = 3, and x = 7.
         */
        [Test]
        public void Step_FallsIntoReservoirAndRisesThroughThreeShafts()
        {
            DungeonMap testMap = new DungeonMap(11, 5);

            testMap[4, 4] = TileType.Floor;
            
            for (int y = 0; y <= 4; y++)
                testMap[5, y] = TileType.Floor;

            for (int x = 1; x <= 8; x++)
                testMap[x, 0] = TileType.Floor;

            foreach (int x in new[] { 1, 3, 7 })
            {
                testMap[x, 1] = TileType.Floor;
                testMap[x, 2] = TileType.Floor;
            }

            WaterSimulation simulation = new WaterSimulation(testMap, 4, 4, 0.5f);

            for (int i = 0; i < 40; i++)
                simulation.Step();

            Assert.Greater(simulation[1, 1], 0f);
            Assert.Greater(simulation[3, 1], 0f);
            Assert.Greater(simulation[7, 1], 0f);
        }
        
        /*
         * Tests a cascade of three consecutive waterfalls.
         *
         * Map layout (# = Wall, . = Floor, S = Source):
         *
         *        x  0 1 2 3 4 5 6 7 8
         *
         * y = 6     # S . . # # # # #
         * y = 5     # # # . # # # # #
         * y = 4     # # . . . # # # #
         * y = 3     # # . # # # # # #
         * y = 2     # . . . . # # # #
         * y = 1     # # # # . # # # #
         * y = 0     # # # . . . # # #
         *
         * Expected falls:
         *   (3, 6) -> (3, 4)
         *   (2, 4) -> (2, 2)
         *   (4, 2) -> (4, 0)
         *
         * Water must reach each ledge before creating
         * the next lower water front.
         */
        [Test]
        public void Step_CreatesMultipleFrontsThroughCascade()
        {
            DungeonMap testMap = new DungeonMap(9, 7);

            for (int x = 1; x <= 3; x++)
                testMap[x, 6] = TileType.Floor;

            testMap[3, 5] = TileType.Floor;

            for (int x = 2; x <= 4; x++)
                testMap[x, 4] = TileType.Floor;

            testMap[2, 3] = TileType.Floor;

            for (int x = 1; x <= 4; x++)
                testMap[x, 2] = TileType.Floor;

            testMap[4, 1] = TileType.Floor;

            for (int x = 3; x <= 5; x++)
                testMap[x, 0] = TileType.Floor;

            WaterSimulation simulation = new WaterSimulation(testMap, 1, 6, 0.5f);

            for (int i = 0; i < 40; i++)
                simulation.Step();

            Assert.IsTrue(simulation.Fronts.Any(front =>
                front.IncomingFall.HasValue &&
                front.IncomingFall.Value.Opening == new Vector2Int(3, 6) &&
                front.EntryPoint == new Vector2Int(3, 4)));

            Assert.IsTrue(simulation.Fronts.Any(front =>
                front.IncomingFall.HasValue &&
                front.IncomingFall.Value.Opening == new Vector2Int(2, 4) &&
                front.EntryPoint == new Vector2Int(2, 2)));

            Assert.IsTrue(simulation.Fronts.Any(front =>
                front.IncomingFall.HasValue &&
                front.IncomingFall.Value.Opening == new Vector2Int(4, 2) &&
                front.EntryPoint == new Vector2Int(4, 0)));
        }
        
        /*
         * Tests two waterfalls entering the same lower reservoir.
         *
         * Map layout (# = Wall, . = Floor, S = Source):
         *
         *        x  0 1 2 3 4 5 6 7 8
         *
         * y = 4     # # # # # # # # #
         * y = 3     # . . . S . . . #
         * y = 2     # . # # # # # . #
         * y = 1     # . # # # # # . #
         * y = 0     # . . . . . . . #
         *
         * Water reaches both openings at (1, 3) and (7, 3).
         * Both waterfalls lead into the same lower reservoir.
         */
        [Test]
        public void Step_TwoWaterfallsIntoSameReservoir()
        {
            DungeonMap testMap = new DungeonMap(9, 5);

            for (int x = 1; x <= 7; x++)
            {
                testMap[x, 3] = TileType.Floor;
                testMap[x, 0] = TileType.Floor;
            }

            for (int y = 1; y <= 2; y++)
            {
                testMap[1, y] = TileType.Floor;
                testMap[7, y] = TileType.Floor;
            }

            WaterSimulation simulation = new WaterSimulation(testMap, 4, 3, 0.5f);

            for (int i = 0; i < 40; i++)
                simulation.Step();

            Assert.IsTrue(simulation.Fronts.Any(front =>
                front.IncomingFall.HasValue &&
                front.IncomingFall.Value.Opening == new Vector2Int(1, 3)));

            Assert.IsTrue(simulation.Fronts.Any(front =>
                front.IncomingFall.HasValue &&
                front.IncomingFall.Value.Opening == new Vector2Int(7, 3)));

            Assert.AreEqual(
                simulation.Fronts.Count,
                simulation.Fronts.Select(front => front.EntryPoint).Distinct().Count());
        }
        
        /*
         * Tests water rising through two shafts connected
         * by a horizontal corridor at an intermediate level.
         *
         * Map layout (# = Wall, . = Floor, S = Source):
         *
         *        x  0 1 2 3 4 5 6
         *
         * y = 4     # # # # # . #
         * y = 3     # # # # # . #
         * y = 2     # . . . . . #
         * y = 1     # . # # # # #
         * y = 0     # S # # # # #
         *
         * Water rises through the first shaft, spreads across
         * the intermediate corridor, then rises through
         * the second shaft.
         */
        [Test]
        public void Step_RisesThroughTwoSuccessiveShafts()
        {
            DungeonMap testMap = new DungeonMap(7, 5);

            for (int y = 0; y <= 2; y++)
                testMap[1, y] = TileType.Floor;

            for (int x = 1; x <= 5; x++)
                testMap[x, 2] = TileType.Floor;

            for (int y = 2; y <= 4; y++)
                testMap[5, y] = TileType.Floor;

            WaterSimulation simulation = new WaterSimulation(testMap, 1, 0, 0.5f);

            for (int i = 0; i < 20; i++)
                simulation.Step();

            Assert.Greater(simulation[5, 3], 0f);
            Assert.Greater(simulation[5, 4], 0f);
        }
        
        /*
         * Tests that water fills a lower side corridor
         * before reaching a higher side corridor.
         *
         * Map layout (# = Wall, . = Floor, S = Source):
         *
         *        x  0 1 2 3 4 5 6
         *
         * y = 4     # # # . . . #
         * y = 3     # # # . # # #
         * y = 2     # . . . # # #
         * y = 1     # # # . # # #
         * y = 0     # # # S # # #
         *
         * Water rises through the central shaft and fills
         * the lower side corridor before the upper one.
         */
        [Test]
        public void Step_FillsLowerBranchBeforeUpperBranch()
        {
            DungeonMap testMap = new DungeonMap(7, 5);

            for (int y = 0; y <= 4; y++)
                testMap[3, y] = TileType.Floor;

            for (int x = 1; x <= 2; x++)
                testMap[x, 2] = TileType.Floor;

            for (int x = 4; x <= 5; x++)
                testMap[x, 4] = TileType.Floor;

            WaterSimulation simulation = new WaterSimulation(testMap, 3, 0, 0.5f);

            for (int i = 0; i < 10; i++)
                simulation.Step();

            Assert.That(simulation[1, 2], Is.EqualTo(1f).Within(0.0001f));
            Assert.That(simulation[5, 4], Is.EqualTo(0f).Within(0.0001f));
        }
        
        /*
         * Tests that filling a dead-end side corridor does not
         * prevent water from continuing upward through the main shaft.
         *
         * Map layout (# = Wall, . = Floor, S = Source):
         *
         *        x  0 1 2 3 4 5 6
         *
         * y = 4     # # # . # # #
         * y = 3     # # # . # # #
         * y = 2     # . . . # # #
         * y = 1     # # # . # # #
         * y = 0     # # # S # # #
         *
         * Water fills the dead-end corridor at y = 2,
         * then continues rising to (3, 4).
         */
        [Test]
        public void Step_ContinuesRisingAfterFillingSideDeadEnd()
        {
            DungeonMap testMap = new DungeonMap(7, 5);

            for (int y = 0; y <= 4; y++)
                testMap[3, y] = TileType.Floor;

            for (int x = 1; x <= 2; x++)
                testMap[x, 2] = TileType.Floor;

            WaterSimulation simulation = new WaterSimulation(testMap, 3, 0, 0.5f);

            for (int i = 0; i < 16; i++)
                simulation.Step();

            Assert.That(simulation[1, 2], Is.EqualTo(1f).Within(0.0001f));
            Assert.Greater(simulation[3, 4], 0f);
        }
    }
}