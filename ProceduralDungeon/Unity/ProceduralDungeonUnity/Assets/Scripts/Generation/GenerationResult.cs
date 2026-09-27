using Core;
using JetBrains.Annotations;

namespace Generation
{
    public class GenerationResult
    {
        public DungeonMap Map { get; }
        [CanBeNull] public GenerationHistory History { get; }
        public double GenerationTimeMs { get; }

        public GenerationResult(DungeonMap map, 
            double generationTimeMs, GenerationHistory history = null)
        {
            Map = map;
            GenerationTimeMs = generationTimeMs;
            History = history;
        }
    }
}