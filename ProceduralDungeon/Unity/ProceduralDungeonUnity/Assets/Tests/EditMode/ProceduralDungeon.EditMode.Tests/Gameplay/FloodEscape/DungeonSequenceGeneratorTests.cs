using Core;
using Gameplay.FloodEscape;
using Gameplay.FloodEscape.Generation;
using Generation;
using NUnit.Framework;

namespace Tests.Gameplay.FloodEscape
{
    public class DungeonSequenceGeneratorTests
    {
        private const int Width = 21;
        private const int Height = 15;
        private const int InitialSeed = 12345;
        private const MazeAlgorithm Algorithm = MazeAlgorithm.RecursiveBacktracker;
        

        [Test]
        public void GenerateNext_WithSameInitialParameters_ProducesSameSequence()
        {
            DungeonSequenceGenerator first = new DungeonSequenceGenerator(
                Width, Height, InitialSeed, Algorithm);

            DungeonSequenceGenerator second = new DungeonSequenceGenerator(
                Width, Height, InitialSeed, Algorithm);

            for (int i = 0; i < 5; i++)
            {
                DungeonMap firstMap = first.GenerateNext();
                DungeonMap secondMap = second.GenerateNext();

                AssertMapsEqual(firstMap, secondMap);
            }
        }

        [Test]
        public void GenerateNext_PreservesDimensions()
        {
            DungeonSequenceGenerator sequence = new DungeonSequenceGenerator(
                Width, Height, InitialSeed, Algorithm);

            for (int i = 0; i < 5; i++)
            {
                DungeonMap map = sequence.GenerateNext();

                Assert.AreEqual(Width, map.Width);
                Assert.AreEqual(Height, map.Height);
            }
        }

        private static void AssertMapsEqual(IReadOnlyDungeonMap expected, IReadOnlyDungeonMap actual)
        {
            Assert.AreEqual(expected.Width, actual.Width);
            Assert.AreEqual(expected.Height, actual.Height);

            for (int y = 0; y < expected.Height; y++)
            {
                for (int x = 0; x < expected.Width; x++)
                {
                    Assert.AreEqual(
                        expected[x, y],
                        actual[x, y],
                        $"Maps differ at ({x}, {y})");
                }
            }
        }
        
        [Test]
        public void GenerateNext_ProducesDifferentMapFromInitial()
        {
            MazeGeneratorFactory factory = new MazeGeneratorFactory();
            MazeGenerator mazeGenerator = factory.Create(Algorithm, InitialSeed);
            MapGenerator mapGenerator = new MapGenerator(mazeGenerator);

            DungeonMap initialMap = mapGenerator.Generate(Width, Height, false).Map;

            DungeonSequenceGenerator sequence = new DungeonSequenceGenerator(
                Width, Height, InitialSeed, Algorithm);

            DungeonMap nextMap = sequence.GenerateNext();

            bool mapsDiffer = false;

            for (int y = 0; y < Height && !mapsDiffer; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (initialMap[x, y] != nextMap[x, y])
                    {
                        mapsDiffer = true;
                        break;
                    }
                }
            }

            Assert.IsTrue(mapsDiffer);
        }
    }
}