using System.Collections;
using Core;
using Gameplay.FloodEscape.Water;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Gameplay.FloodEscape
{
    public class WaterRendererTests
    {
        private GameObject rendererObject;
        private WaterRenderer waterRenderer;
        private DungeonMap map;
        
        [SetUp]
        public void SetUp()
        {
            rendererObject = new GameObject("WaterRenderer");
            waterRenderer = rendererObject.AddComponent<WaterRenderer>();

            map = new DungeonMap(3, 3);
            map[1, 0] = TileType.Floor;
            map[1, 1] = TileType.Floor;
            map[2, 1] = TileType.Floor;
        }
        
        [UnityTest]
        public IEnumerator Initialize_CreatesWaterObjectsOnlyForValidCells()
        {
            WaterSimulation simulation = CreateSimulation();

            waterRenderer.Initialize(simulation);

            Assert.AreEqual(3, rendererObject.transform.childCount);

            Assert.IsNotNull(FindWaterCell(1, 0));
            Assert.IsNotNull(FindWaterCell(1, 1));
            Assert.IsNotNull(FindWaterCell(2, 1));

            Assert.IsNull(FindWaterCell(0, 0));
            Assert.IsNull(FindWaterCell(0, 1));

            yield return null;
        }
        
        [UnityTest]
        public IEnumerator Refresh_UpdatesWaterObjects()
        {
            WaterSimulation simulation = CreateSimulation();
            waterRenderer.Initialize(simulation);

            Transform waterCell = FindWaterCell(1, 0);

            Assert.IsNotNull(waterCell);
            Assert.IsFalse(waterCell.gameObject.activeSelf);

            simulation.Step();
            waterRenderer.Refresh();

            float level = simulation[1, 0];

            Assert.Greater(level, 0f);
            Assert.IsTrue(waterCell.gameObject.activeSelf);
            Assert.AreEqual(level, waterCell.localScale.y, 0.0001f);

            Transform emptyCell = FindWaterCell(2, 1);
            Assert.IsNotNull(emptyCell);
            Assert.IsFalse(emptyCell.gameObject.activeSelf);

            yield return null;
        }
        
        [UnityTest]
        public IEnumerator Clear_RemovesWaterObjectsAndResetsState()
        {
            waterRenderer.Initialize(CreateSimulation());

            Assert.AreEqual(3, rendererObject.transform.childCount);

            waterRenderer.Clear();

            foreach (Transform child in rendererObject.transform)
                Assert.IsFalse(child.gameObject.activeSelf);

            Assert.DoesNotThrow(() => waterRenderer.Refresh());

            yield return null;

            Assert.AreEqual(0, rendererObject.transform.childCount);
        }
        
        [UnityTest]
        public IEnumerator Initialize_WhenCalledAgain_ReplacesPreviousWaterObjects()
        {
            waterRenderer.Initialize(CreateSimulation());

            Transform oldWaterCell = FindWaterCell(1, 0);
            Assert.IsNotNull(oldWaterCell);

            waterRenderer.Initialize(CreateSimulation());

            Assert.IsFalse(oldWaterCell.gameObject.activeSelf);

            yield return null;

            Assert.IsTrue(oldWaterCell == null);
            Assert.AreEqual(3, rendererObject.transform.childCount);

            Assert.IsNotNull(FindWaterCell(1, 0));
            Assert.IsNotNull(FindWaterCell(1, 1));
            Assert.IsNotNull(FindWaterCell(2, 1));
        }
        
        [TearDown]
        public void TearDown()
        {
            if (rendererObject != null)
                Object.DestroyImmediate(rendererObject);
        }

        private WaterSimulation CreateSimulation()
        {
            return new WaterSimulation(map, 1, 0, 0.1f);
        }

        private Transform FindWaterCell(int x, int y)
        {
            return rendererObject.transform.Find($"Water_{x}_{y}");
        }
    }
}