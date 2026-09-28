using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private Button playPauseButton;
        [SerializeField] private GameObject playIcon;
        [SerializeField] private GameObject pauseIcon;
        [SerializeField] private Slider stepSlider;
        [SerializeField] private TMP_Dropdown speedDropdown;

        [Header("Output")] 
        [SerializeField] private TMP_Text generatedInfo;

        [Header("Dependencies")] 
        [SerializeField] private DungeonController dungeonController;
        [SerializeField] private DungeonVisualizer visualizer;
        [SerializeField] private CameraFitter cameraFitter;
        [SerializeField] private DungeonView dungeonView;

        private const float PlaybackInterval = 0.1f;

        private GenerationHistory currentHistory;
        private int currentStep;

        private float playbackSpeed = 1f;
        private Coroutine playbackCoroutine;

        void Start()
        {
            algorithmDropdown.ClearOptions();
            algorithmDropdown.AddOptions(Enum.GetNames(typeof(MazeAlgorithm)).ToList());

            SetPlaceholder(widthInput, defaultWidth);
            SetPlaceholder(heightInput, defaultHeight);
            
            OnRecordHistoryChanged(recordHistoryToggle.isOn);
            
            stepInfo.text = "Step: 0 / 0";
            stepSlider.wholeNumbers = true;
            
            speedDropdown.ClearOptions();
            speedDropdown.AddOptions(new List<string> { "0.5×", "1×", "2×" });
            speedDropdown.value = 1;
            
            UpdatePlaybackControls();
            UpdatePlayPauseIcon();
        }

        public void OnRecordHistoryChanged(bool isOn)
        {
            animateGenerationToggle.interactable = isOn;

            if (!isOn)
                animateGenerationToggle.isOn = false;
        }

        public void OnGenerateClicked()
        {
            StopPlayback();
            
            MazeAlgorithm algorithm = (MazeAlgorithm)algorithmDropdown.value;

            int width = ReadSize(widthInput, defaultWidth);
            int height = ReadSize(heightInput, defaultHeight);
            int seed = ReadSeed();
            bool recordHistory = ReadRecordHistory();

            GenerationResult generationResult =
                dungeonController.GenerateDungeon(algorithm, width, height, seed, recordHistory);

            SetGenerationResult(generationResult);
            ShowGenerationInfo(generationResult, algorithm, width, height, seed);
        }

        public void OnPlaybackSpeedChanged(int index)
        {
            playbackSpeed = index switch
            {
                0 => 0.5f,
                1 => 1f,
                2 => 2f,
                _ => 1f
            };
        }
        
        public void OnPreviousStepClicked()
        {
            SetCurrentStep(currentStep - 1);
        }

        public void OnPlayPauseClicked()
        {
            if (currentHistory == null)
                return;
            
            if (playbackCoroutine != null)
                StopPlayback();
            else
                StartPlayback();
        }

        public void OnNextStepClicked()
        {
            SetCurrentStep(currentStep + 1);
        }
        
        public void OnStepSliderChanged(float value)
        {
            SetCurrentStep((int)value);
        }
        
        private void SetGenerationResult(GenerationResult generationResult)
        {
            SetHistory(generationResult.History);

            cameraFitter.Fit(visualizer.GetCenter(generationResult.Map), visualizer.GetSize(generationResult.Map));
            dungeonView.FitPreview(generationResult.Map.Width, generationResult.Map.Height);

            if (currentHistory != null && animateGenerationToggle.isOn)
            {
                SetCurrentStep(0);
                StartPlayback();
            }
            else
            {
                visualizer.Draw(generationResult.Map);
            }
        }
        
        private void ShowGenerationInfo(GenerationResult generationResult, MazeAlgorithm algorithm, int width, int height, int seed)
        {
            generatedInfo.text = $"Generated with {algorithm} ({width} × {height}). Seed: {seed} " +
                                 $"Time: {generationResult.GenerationTimeMs:F2} ms";
        }
        
        private void SetHistory(GenerationHistory history)
        {
            currentHistory = history;
            
            if (currentHistory != null)
            {
                currentStep = currentHistory.Count - 1;

                stepSlider.minValue = 0;
                stepSlider.maxValue = currentHistory.Count - 1;
                
                SetCurrentStep(currentStep);
            }
            else
            {
                currentStep = 0;
                
                stepSlider.minValue = 0;
                stepSlider.maxValue = 0;
                stepSlider.SetValueWithoutNotify(0);
                
                stepInfo.text = "Step: Final";
                
                UpdatePlaybackControls();
            }
        }
        
        private IEnumerator PlayHistory()
        {
            if (currentStep >= currentHistory.Count - 1)
                SetCurrentStep(0);

            while (currentStep < currentHistory.Count - 1)
            {
                yield return new WaitForSeconds(PlaybackInterval / playbackSpeed);
                SetCurrentStep(currentStep + 1);
            }

            playbackCoroutine = null;
            UpdatePlayPauseIcon();
        }
        
        private void StartPlayback()
        {
            playbackCoroutine = StartCoroutine(PlayHistory());
            UpdatePlayPauseIcon();
        }
        
        private void StopPlayback()
        {
            if (playbackCoroutine == null)
                return;

            StopCoroutine(playbackCoroutine);
            playbackCoroutine = null;
            UpdatePlayPauseIcon();
        }
        
        private void SetCurrentStep(int step)
        {
            if (currentHistory == null)
                return;

            currentStep = Mathf.Clamp(step, 0, currentHistory.Count - 1);

            stepSlider.SetValueWithoutNotify(currentStep);
            
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
            
            playPauseButton.interactable = hasHistory;
            stepSlider.interactable = hasHistory;
            speedDropdown.interactable = hasHistory;
            
            nextButton.interactable = hasHistory && currentStep < currentHistory.Count - 1;
        }
        
        private void UpdatePlayPauseIcon()
        {
            bool isPlaying = playbackCoroutine != null;
            
            playIcon.SetActive(!isPlaying);
            pauseIcon.SetActive(isPlaying);
        }
    }
}
