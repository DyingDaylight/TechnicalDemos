using System.Diagnostics;
using Core;

namespace Generation
{
    public class MapGenerator
    {
        private readonly MazeGenerator mazeGenerator;

        public MapGenerator(MazeGenerator mazeGenerator)
        {
            this.mazeGenerator = mazeGenerator;
        }

        public GenerationResult Generate(int width, int height, bool recordHistory)
        {
            GenerationHistory history = null;
            DungeonMap map = new DungeonMap(width, height);

            if (recordHistory)
            {
                history = new GenerationHistory();
                mazeGenerator.StepCompleted += history.Record;
            }
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                mazeGenerator.Generate(map);
            }
            finally
            {
                stopwatch.Stop();
                if (history != null)
                    mazeGenerator.StepCompleted -= history.Record;
            }
            
            double generationTimeMs = stopwatch.Elapsed.TotalMilliseconds;
            GenerationResult generationResult = new GenerationResult(map, generationTimeMs, history);
            return generationResult;
        }
    }
}