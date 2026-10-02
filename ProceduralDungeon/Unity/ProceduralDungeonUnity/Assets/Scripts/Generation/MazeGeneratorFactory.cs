using System;

namespace Generation
{
    public class MazeGeneratorFactory
    {
        public MazeGenerator Create(MazeAlgorithm algorithm, int seed)
        {
            switch (algorithm)
            {
                case MazeAlgorithm.RecursiveBacktracker:
                    return new RecursiveBacktracker(seed);

                case MazeAlgorithm.RandomizedPrim:
                    return new RandomizedPrim(seed);

                case MazeAlgorithm.RandomizedKruskal:
                    return new RandomizedKruskal(seed);

                default:
                    throw new ArgumentOutOfRangeException(nameof(algorithm));
            }
        }
    }
}