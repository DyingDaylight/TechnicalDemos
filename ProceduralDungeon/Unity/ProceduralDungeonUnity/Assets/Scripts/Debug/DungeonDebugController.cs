using Core;
using DebugTools;
using Generation;
using UnityEngine;
using Visualization;

public class DungeonDebugController : MonoBehaviour
{
    [SerializeField] MazeAlgorithm algorithm;
    [SerializeField] private int dungeonWidth = 20;
    [SerializeField] private int dungeonHeight = 20;
    [SerializeField] private bool useSeed = true;
    [SerializeField] private int seed = 100;
    
    [SerializeField] private DungeonController dungeonController;
    [SerializeField] private DungeonVisualizer visualizer;
    
    void Start()
    {
        int _seed = useSeed ? seed : Random.Range(int.MinValue, int.MaxValue);
        
        DungeonMap map = dungeonController.GenerateDungeon(
            algorithm, dungeonWidth, dungeonHeight, _seed);

        Debug.Log(DungeonDebug.ToString(map));
        visualizer.Draw(map);
    }

}
