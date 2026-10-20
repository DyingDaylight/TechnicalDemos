using Core;
using Generation;
using UnityEngine;

namespace Gameplay.FloodEscape.Generation
{
    public class DungeonSequenceGenerator
    {
        private const bool RecordHistory = false;

        private readonly int width;
        private readonly int height;
        private readonly MazeAlgorithm mazeAlgorithm;

        private int seed;

        public DungeonSequenceGenerator(int width, int height, int seed, MazeAlgorithm mazeAlgorithm)
        {
            this.seed = seed;
            this.width = width;
            this.height = height;
            this.mazeAlgorithm = mazeAlgorithm;
        }
        
        public DungeonMap GenerateNext()
        {
            int nextSeed = NextSeed(seed);
            MazeGeneratorFactory factory = new MazeGeneratorFactory();
            MazeGenerator mazeGenerator = factory.Create(mazeAlgorithm, nextSeed);
            MapGenerator mapGenerator = new MapGenerator(mazeGenerator);
            GenerationResult generationResult = mapGenerator.Generate(width, height, RecordHistory);
            
            seed = nextSeed;
            Debug.Log($"Map generated with seed {seed}");
            return generationResult.Map;
        }

        private static int NextSeed(int seed)
        {
            return unchecked(seed * 1664525 + 1013904223);
        }
    }
}