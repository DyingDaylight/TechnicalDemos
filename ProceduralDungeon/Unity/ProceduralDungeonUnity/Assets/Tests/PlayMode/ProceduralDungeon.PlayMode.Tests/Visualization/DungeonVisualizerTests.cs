using NUnit.Framework;
using UnityEngine;
using Visualization;

namespace Tests.Visualization
{
    public class DungeonVisualizerTests
    {
        [Test]
        public void Draw_RendersDungeonMap()
        {
            Assert.Ignore(
                "DungeonVisualizer.Draw uses temporary debug rendering. " +
                "Add rendering tests when theme-based dungeon visualization is implemented.");
        }
    }
}