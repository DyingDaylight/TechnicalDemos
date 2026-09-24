using System;
using Core;
using DebugTools;
using DefaultNamespace;
using Generation;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    public DungeonMap GenerateDungeon(MazeAlgorithm mazeAlgorithm, int width, int height, int seed)
    {
        MazeGenerator mazeGenerator = CreateMazeGenerator(mazeAlgorithm, seed);
        MapGenerator mapGenerator = new MapGenerator(mazeGenerator);
        DungeonMap map = mapGenerator.Generate(width, height);
        
        return map;
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
