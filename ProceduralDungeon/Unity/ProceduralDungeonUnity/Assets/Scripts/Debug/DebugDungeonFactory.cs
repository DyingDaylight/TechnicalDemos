using Core;
using Generation;

namespace DebugTools
{
    public static class DebugDungeonFactory
    {
        private const int DebugSeed = 15;
        private const int DebugWidth = 19;
        private const int DebugHeight = 21;
        private const bool DebugRecordHistory = false;
        
        public static IReadOnlyDungeonMap Create()
        {
            MazeGeneratorFactory factory = new MazeGeneratorFactory();
            MazeGenerator mazeGenerator = factory.Create(MazeAlgorithm.RecursiveBacktracker, DebugSeed);
            MapGenerator mapGenerator = new MapGenerator(mazeGenerator);
            GenerationResult generationResult = mapGenerator.Generate(DebugWidth, DebugHeight, DebugRecordHistory);
            return generationResult.Map;
        }
    }
}