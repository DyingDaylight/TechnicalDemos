using Core;
using Generation;
using NUnit.Framework;

namespace Tests.Generation
{
    public class MapGeneratorTests
    {
        private class TestMazeGenerator : MazeGenerator
        {
            public override void Generate(DungeonMap map)
            {
                map[1, 1] = TileType.Door;
                CompleteStep(map);
            }
        }

        [Test]
        public void Generate_CreatesMapWithRequestedDimensions()
        {
            // Arrange
            MazeGenerator mazeGenerator = new TestMazeGenerator();
            MapGenerator mapGenerator = new MapGenerator(mazeGenerator);

            int width = 10;
            int height = 8;

            // Act
            GenerationResult result = mapGenerator.Generate(width, height, false);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Map);
            Assert.AreEqual(width, result.Map.Width);
            Assert.AreEqual(height, result.Map.Height);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void Generate_SetsHistoryAccordingToRecordHistory(bool recordHistory)
        {
            MazeGenerator mazeGenerator = new TestMazeGenerator();
            MapGenerator mapGenerator = new MapGenerator(mazeGenerator);
            
            int width = 10;
            int height = 8;

            // Act
            GenerationResult result = mapGenerator.Generate(width, height, recordHistory);
            
            Assert.IsNotNull(result);
            if (recordHistory)
                Assert.IsNotNull(result.History);
            else
                Assert.IsNull(result.History);
        }
        
        [Test]
        public void Generate_WithHistory_RecordsCompletedStep()
        {
            // Arrange
            MazeGenerator mazeGenerator = new TestMazeGenerator();
            MapGenerator mapGenerator = new MapGenerator(mazeGenerator);

            // Act
            GenerationResult result = mapGenerator.Generate(3, 3, true);

            // Assert
            Assert.IsNotNull(result.History);
            Assert.AreEqual(1, result.History.Count);
            Assert.AreEqual(TileType.Door, result.History[0][1, 1]);
        }
        
        [Test]
        public void Generate_CalledTwice_DoesNotDuplicateHistorySubscription()
        {
            // Arrange
            MazeGenerator mazeGenerator = new TestMazeGenerator();
            MapGenerator mapGenerator = new MapGenerator(mazeGenerator);

            // Act
            GenerationResult firstResult = mapGenerator.Generate(3, 3, true);
            GenerationResult secondResult = mapGenerator.Generate(3, 3, true);

            // Assert
            Assert.IsNotNull(firstResult.History);
            Assert.IsNotNull(secondResult.History);

            Assert.AreEqual(1, firstResult.History.Count);
            Assert.AreEqual(1, secondResult.History.Count);
        }
    }
}