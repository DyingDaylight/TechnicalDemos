using Core;
using NUnit.Framework;

namespace Tests.Core
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
        
        [TestCase(0, 0, true)]
        [TestCase(2, 2, true)]
        [TestCase(1, 1, true)]
        [TestCase(-1, 2, false)]
        [TestCase(3, 2, false)]
        [TestCase(1, -1, false)]
        [TestCase(2, 3, false)]
        public void IsInside_ReturnsExpectedResult(int x, int y, bool expected)
        {
            // Arrange
            IReadOnlyDungeonMap snapshot = new DungeonSnapshot(map);
            
            // Act and Assert
            Assert.AreEqual(expected, snapshot.IsInside(x, y));
        }
    }
}