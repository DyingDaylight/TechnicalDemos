using Generation;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class DungeonControllerTests
    {
        private GameObject gameObject;
        private DungeonController controller;

        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject();
            controller = gameObject.AddComponent<DungeonController>();
        }
        
        [TestCase(MazeAlgorithm.RecursiveBacktracker)]
        [TestCase(MazeAlgorithm.RandomizedPrim)]
        [TestCase(MazeAlgorithm.RandomizedKruskal)]
        public void GenerateDungeon_WithAlgorithm_ReturnsGeneratedMap(MazeAlgorithm algorithm)
        {
            GenerationResult generationResult = controller.GenerateDungeon(algorithm,
                10, 7, 123, false);
            
            Assert.IsNotNull(generationResult);
            Assert.IsNotNull(generationResult.Map);
            Assert.IsNull(generationResult.History);
            Assert.AreEqual(10, generationResult.Map.Width);
            Assert.AreEqual(7, generationResult.Map.Height);
        }
        
        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameObject);
        }
    }
}