using Core;
using Generation;

namespace Session
{
    public class DungeonSessionData
    {
        public IReadOnlyDungeonMap Map { get; }
        public int Seed { get; }
        public MazeAlgorithm Algorithm { get; }

        public DungeonSessionData(IReadOnlyDungeonMap map, int seed, MazeAlgorithm algorithm)
        {
            Map = map;
            Seed = seed;
            Algorithm = algorithm;
        }
    }
}