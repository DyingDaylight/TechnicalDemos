using Core;
using NUnit.Framework;

namespace Tests.EditMode.ProceduralDungeon.EditMode.Tests.Core
{
    public class DungeonMapTests
    {

        [Test]
        public void Constructor_SetsWidthAndHeight()
        {
            // Arrange
            int width = 10;
            int height = 8;
            
            // Act
            DungeonMap map = new DungeonMap(width, height);
            
            // Assert
            Assert.AreEqual(width, map.Width);
            Assert.AreEqual(height, map.Height);
        }

        [Test]
        public void Indexer_SetsAndGetsTile()
        {
            // Arrange
            int width = 10;
            int height = 8;
            int x = 3;
            int y = 2;
            DungeonMap map = new DungeonMap(width, height);
            
            // Acts
            map[x, y] = TileType.Floor;
            // Assert
            Assert.AreEqual(TileType.Floor, map[x, y]);
            
            // Act
            map[x, y] = TileType.Wall;
            // Assert
            Assert.AreEqual(TileType.Wall, map[x, y]);
        }
        
        [TestCase(3, 2, true)]
        [TestCase(0, 0, true)]
        [TestCase(9, 7, true)]
        [TestCase(-1, 2, false)]
        [TestCase(3, -1, false)]
        [TestCase(10, 2, false)]
        [TestCase(3, 8, false)]
        public void IsInside_ReturnsExpectedResult(int x, int y, bool expected)
        {
            // Arrange
            DungeonMap map = new DungeonMap(10, 8);
            
            // Act and Assert
            Assert.AreEqual(expected, map.IsInside(x, y));
        }
    }
}