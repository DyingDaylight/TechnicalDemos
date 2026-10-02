using System;
using Generation;
using NUnit.Framework;

namespace Tests.Generation
{
    public class MazeGeneratorFactoryTests
    {
        [TestCase(MazeAlgorithm.RecursiveBacktracker, typeof(RecursiveBacktracker))]
        [TestCase(MazeAlgorithm.RandomizedPrim, typeof(RandomizedPrim))]
        [TestCase(MazeAlgorithm.RandomizedKruskal, typeof(RandomizedKruskal))]
        public void Create_WithAlgorithm_ReturnsExpectedGenerator(
            MazeAlgorithm algorithm, Type expectedType)
        {
            MazeGeneratorFactory factory = new MazeGeneratorFactory();

            MazeGenerator generator = factory.Create(algorithm, 123);

            Assert.IsInstanceOf(expectedType, generator);
        }
        
        [Test]
        public void Create_WithInvalidAlgorithm_ThrowsArgumentOutOfRangeException()
        {
            MazeGeneratorFactory factory = new MazeGeneratorFactory();

            MazeAlgorithm invalidAlgorithm = (MazeAlgorithm)999;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                factory.Create(invalidAlgorithm, 123));
        }
    }
}