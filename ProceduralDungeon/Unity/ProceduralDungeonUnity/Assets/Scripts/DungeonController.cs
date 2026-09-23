using System;
using Core;
using DebugTools;
using DefaultNamespace;
using Generation;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    [SerializeField] MazeAlgorithm algorithm;
    [SerializeField] private int dungeonWidth = 20;
    [SerializeField] private int dungeonHeight = 20;
    [SerializeField] private bool useSeed = true;
    [SerializeField] private int seed = 100;
    
    [SerializeField] private DungeonVisualizer visualizer;
    
    void Start()
    {
        MazeGenerator mazeGenerator = CreateMazeGenerator();
        MapGenerator mapGenerator = new MapGenerator(mazeGenerator);
        DungeonMap map = mapGenerator.Generate(dungeonWidth, dungeonHeight);

        Debug.Log(DungeonDebug.ToString(map));
        visualizer.Draw(map);
    }
    
    private MazeGenerator CreateMazeGenerator()
    {
        switch (algorithm)
        {
            case MazeAlgorithm.RecursiveBacktracker:
                return useSeed
                    ? new RecursiveBacktracker(seed)
                    : new RecursiveBacktracker();

            case MazeAlgorithm.RandomizedPrim:
                return useSeed
                    ? new RandomizedPrim(seed)
                    : new RandomizedPrim();
            
            case MazeAlgorithm.RandomizedKruskal:
                return useSeed
                    ? new RandomizedKruskal(seed)
                    : new RandomizedKruskal();

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
