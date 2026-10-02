using Generation;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    public GenerationResult GenerateDungeon(MazeAlgorithm mazeAlgorithm, int width, int height, int seed,
        bool recordHistory = false)
    {
        MazeGeneratorFactory factory = new MazeGeneratorFactory();
        MazeGenerator mazeGenerator = factory.Create(mazeAlgorithm, seed);
        MapGenerator mapGenerator = new MapGenerator(mazeGenerator);
        GenerationResult generationResult = mapGenerator.Generate(width, height, recordHistory);
        
        return generationResult;
    }
}
