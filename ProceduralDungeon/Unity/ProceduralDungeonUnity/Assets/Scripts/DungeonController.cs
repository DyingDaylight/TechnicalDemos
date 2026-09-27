using System;
using Generation;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    public GenerationResult GenerateDungeon(MazeAlgorithm mazeAlgorithm, int width, int height, int seed,
        bool recordHistory = false)
    {
        MazeGenerator mazeGenerator = CreateMazeGenerator(mazeAlgorithm, seed);
        MapGenerator mapGenerator = new MapGenerator(mazeGenerator);
        GenerationResult generationResult = mapGenerator.Generate(width, height, recordHistory);
        
        return generationResult;
    }
    
    private MazeGenerator CreateMazeGenerator(MazeAlgorithm algorithm, int seed)
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
                throw new ArgumentOutOfRangeException();
        }
    }
}
