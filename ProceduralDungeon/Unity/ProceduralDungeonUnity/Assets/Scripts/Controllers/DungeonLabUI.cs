using System;
using System.Linq;
using Core;
using DefaultNamespace;
using Generation;
using TMPro;
using UnityEngine;

public class DungeonLabUI : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown algorithmDropdown;

    [SerializeField] private int minSize = 5;
    [SerializeField] private int maxSize = 51;
    [SerializeField] private int defaultWidth = 21;
    [SerializeField] private int defaultHeight = 21;

    [SerializeField] private TMP_InputField widthInput;
    [SerializeField] private TMP_InputField heightInput;
    
    [SerializeField] private TMP_InputField seedInput;
    
    [SerializeField] private TMP_Text generatedInfo;
    
    [SerializeField] private DungeonController dungeonController;
    
    [SerializeField] private DungeonVisualizer visualizer;
    
    void Start()
    {
        algorithmDropdown.ClearOptions();
        algorithmDropdown.AddOptions(Enum.GetNames(typeof(MazeAlgorithm)).ToList());
        
        SetPlaceholder(widthInput, defaultWidth);
        SetPlaceholder(heightInput, defaultHeight);
    }

    public void OnGenerateClicked()
    {
        MazeAlgorithm algorithm = (MazeAlgorithm)algorithmDropdown.value;
        
        int width = ReadSize(widthInput, defaultWidth);
        int height = ReadSize(heightInput, defaultHeight);
        
        int seed = ReadSeed();
        
        DungeonMap map = dungeonController.GenerateDungeon(algorithm, width, height, seed);
        
        visualizer.Draw(map);
        
        generatedInfo.text = $"Generated with {algorithm} ({width} × {height}). Seed: {seed}";
    }
    
    private void SetPlaceholder(TMP_InputField input, int value)
    {
        TMP_Text placeholder = input.placeholder as TMP_Text;
        placeholder.text = value.ToString();
    }
    
    private int ReadSize(TMP_InputField input, int defaultValue)
    {
        if (!int.TryParse(input.text, out int value))
            return defaultValue;

        return Mathf.Clamp(value, minSize, maxSize);
    }
    
    private int ReadSeed()
    {
        if (int.TryParse(seedInput.text, out int seed))
            return seed;

        return UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }
}
