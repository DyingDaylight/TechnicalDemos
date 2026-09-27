using System;
using System.Linq;
using Core;
using Generation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Visualization;

namespace UI
{
    public class DungeonLabUI : MonoBehaviour
    {
        [Header("Generation Settings")]
        [SerializeField] private TMP_Dropdown algorithmDropdown;
        [SerializeField] private TMP_InputField widthInput;
        [SerializeField] private TMP_InputField heightInput;
        [SerializeField] private TMP_InputField seedInput;
        [SerializeField] private Toggle recordHistoryToggle;
        [SerializeField] private Toggle animateGenerationToggle;
        
        [Header("Size Constraints")]
        [SerializeField] private int minSize = 5;
        [SerializeField] private int maxSize = 51;
        [SerializeField] private int defaultWidth = 21;
        [SerializeField] private int defaultHeight = 21;
     
        [Header("Playback")]
        [SerializeField] private TMP_Text stepInfo;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        
        [Header("Output")]
        [SerializeField] private TMP_Text generatedInfo;
    
        [Header("Dependencies")]
        [SerializeField] private DungeonController dungeonController;
        [SerializeField] private DungeonVisualizer visualizer;
        [SerializeField] private CameraFitter cameraFitter;
        [SerializeField] private DungeonView dungeonView;
    
        private GenerationHistory currentHistory;
        private int currentStep;
        
        void Start()
        {
            algorithmDropdown.ClearOptions();
            algorithmDropdown.AddOptions(Enum.GetNames(typeof(MazeAlgorithm)).ToList());
        
            SetPlaceholder(widthInput, defaultWidth);
            SetPlaceholder(heightInput, defaultHeight);

            OnRecordHistoryChanged(recordHistoryToggle.isOn);
            UpdatePlaybackControls();
            
            stepInfo.text = "Step: 0 / 0";
        }

        public void OnRecordHistoryChanged(bool isOn)
        {
            animateGenerationToggle.interactable = isOn;

            if (!isOn)
                animateGenerationToggle.isOn = false;
        }
        
        public void OnGenerateClicked()
        {
            MazeAlgorithm algorithm = (MazeAlgorithm)algorithmDropdown.value;
        
            int width = ReadSize(widthInput, defaultWidth);
            int height = ReadSize(heightInput, defaultHeight);
        
            int seed = ReadSeed();

            bool recordHistory = ReadRecordHistory();
            
            GenerationResult generationResult = dungeonController.GenerateDungeon(algorithm, width, height, seed, recordHistory);
        
            currentHistory = generationResult.History;
            if (currentHistory != null)
                currentStep = currentHistory.Count - 1;
            
            visualizer.Draw(generationResult.Map);
            cameraFitter.Fit(visualizer.GetCenter(generationResult.Map), visualizer.GetSize(generationResult.Map));
            dungeonView.FitPreview(generationResult.Map.Width, generationResult.Map.Height);
            
            UpdatePlaybackControls();
            
            if (currentHistory != null)
                stepInfo.text = $"Step: {currentStep + 1} / {currentHistory.Count}";
            else
                stepInfo.text = "Step: Final";
            
            generatedInfo.text = $"Generated with {algorithm} ({width} × {height}). Seed: {seed} "+
                                 $"Time: {generationResult.GenerationTimeMs:F2} ms";
        }
        
        public void OnPreviousStepClicked()
        {
            if (currentHistory == null || currentStep <= 0)
                return;

            currentStep--;
            ShowCurrentStep();
            UpdatePlaybackControls();
        }

        public void OnNextStepClicked()
        {
            if (currentHistory == null || currentStep >= currentHistory.Count - 1)
                return;

            currentStep++;
            ShowCurrentStep();
            UpdatePlaybackControls();
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

        private bool ReadRecordHistory()
        {
            return recordHistoryToggle.isOn;
        }
        
        private void ShowCurrentStep()
        {
            if (currentHistory == null)
                return;
            
            IReadOnlyDungeonMap snapshot = currentHistory[currentStep];

            visualizer.Draw(snapshot);
            
            stepInfo.text = $"Step: {currentStep + 1} / {currentHistory.Count}";
        }
        
        private void UpdatePlaybackControls()
        {
            bool hasHistory = currentHistory != null;

            previousButton.interactable = hasHistory && currentStep > 0;

            nextButton.interactable = hasHistory && currentStep < currentHistory.Count - 1;
        }
    }
}
