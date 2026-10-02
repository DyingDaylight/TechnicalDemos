using Core;
using Generation;
using NUnit.Framework;

namespace Tests.Generation
{
    public class GenerationHistoryTests
    {

        [Test]
        public void Constructor_CreatesEmptyHistory()
        {
            // Arrange & Act
            GenerationHistory history = new GenerationHistory();

            // Assert
            Assert.AreEqual(0, history.Count);
        }
        
        [Test]
        public void Record_AddsSnapshot()
        {
            // Arrange
            DungeonMap map = new DungeonMap(3, 3);
            GenerationHistory history = new GenerationHistory();

            // Act
            map[1, 1] = TileType.Door;
            history.Record(map);

            // Assert
            Assert.AreEqual(1, history.Count);
            Assert.AreEqual(TileType.Door, history[0][1, 1]);
        }
        
        [Test]
        public void Record_MultipleStates_PreservesEachSnapshot()
        {
            // Arrange
            DungeonMap map = new DungeonMap(3, 3);
            GenerationHistory history = new GenerationHistory();

            map[1, 1] = TileType.Door;

            // Act
            history.Record(map);

            map[1, 1] = TileType.Wall;
            history.Record(map);

            // Assert
            Assert.AreEqual(2, history.Count);
            Assert.AreEqual(TileType.Door, history[0][1, 1]);
            Assert.AreEqual(TileType.Wall, history[1][1, 1]);
        }
    }
}