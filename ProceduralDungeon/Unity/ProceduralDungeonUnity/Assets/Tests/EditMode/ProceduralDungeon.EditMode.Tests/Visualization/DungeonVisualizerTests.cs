using Core;
using NUnit.Framework;
using UnityEngine;
using Visualization;

namespace Tests.Visualization
{
    public class DungeonVisualizerTests
    {
        private GameObject gameObject;
        private DungeonVisualizer visualizer;

        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject();
            visualizer = gameObject.AddComponent<DungeonVisualizer>();
        }
        
        [Test]
        public void GetSize_ReturnsMapDimensions()
        {
            int width = 10;
            int height = 6;
            DungeonMap map = new DungeonMap(width, height);

            Vector2 size = visualizer.GetSize(map);

            Assert.AreEqual(width, size.x);
            Assert.AreEqual(height, size.y);
        }
        
        [Test]
        public void GetCenter_ReturnsMapCenter()
        {
            int width = 10;
            int height = 6;
            DungeonMap map = new DungeonMap(width, height);

            Vector2 center = visualizer.GetCenter(map);

            Assert.AreEqual(4.5f, center.x);
            Assert.AreEqual(2.5f, center.y);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameObject);
        }
    }
}