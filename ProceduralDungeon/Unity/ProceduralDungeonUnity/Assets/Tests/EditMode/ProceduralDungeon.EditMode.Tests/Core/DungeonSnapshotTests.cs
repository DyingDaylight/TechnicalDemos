using System.Collections;
using Core;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;

namespace Tests.EditMode.ProceduralDungeon.EditMode.Tests.Core
{
    public class DungeonSnapshotTests
    {
        private DungeonMap map;

        [SetUp]
        public void SetUp()
        {
            map = new DungeonMap(3, 3);

            map[0, 0] = TileType.Wall;
            map[1, 0] = TileType.Floor;
            map[2, 0] = TileType.Wall;
            
            map[0, 1] = TileType.Wall;
            map[1, 1] = TileType.Floor;
            map[2, 1] = TileType.Door;

            map[0, 2] = TileType.Wall;
            map[1, 2] = TileType.Floor;
            map[2, 2] = TileType.Wall;
        }

        [Test]
        public void Constructor_CopiesMap()
        {
            // Act
            DungeonSnapshot snapshot = new DungeonSnapshot(map);
            
            // Assert
            Assert.AreEqual(map.Width, snapshot.Width);
            Assert.AreEqual(map.Height, snapshot.Height);

            for (int x = 0; x < snapshot.Width; x++)
            {
                for (int y = 0; y < snapshot.Height; y++)
                {
                    Assert.AreEqual(map[x, y], snapshot[x, y], $"The tiles at [{x}, {y}] are not equal.");
                }
            }
        }
        
        [Test]
        public void SourceMapChanged_DoesNotChangeSnapshot()
        {
            // Arrange
            DungeonSnapshot snapshot = new DungeonSnapshot(map);
            
            // Act
            map[2, 1] = TileType.Wall;
            
            // Assert
            Assert.AreNotEqual(TileType.Wall, snapshot[2, 1]);
        }
    }
}