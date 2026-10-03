using System.Linq;
using System.Reflection;
using Generation;
using NUnit.Framework;
using Tests.Mocks;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Visualization;

namespace Tests.UI
{
    public abstract class DungeonLabUITestBase
    {
        protected GameObject gameObject;
        protected DungeonLabUI dungeonLabUI;

        // Generation
        protected TMP_Dropdown algorithmDropdown;
        protected TMP_InputField widthInput;
        protected TMP_InputField heightInput;
        protected TMP_InputField seedInput;
        protected Toggle recordHistoryToggle;
        protected Toggle animateGenerationToggle;
        protected TMP_Text generatedInfo;

        // Size controls
        protected Button widthDecreaseButton;
        protected Button widthIncreaseButton;
        protected Button heightDecreaseButton;
        protected Button heightIncreaseButton;

        // Playback
        protected Slider stepSlider;
        protected TMP_Text stepInfo;
        protected Button previousStepButton;
        protected Button nextStepButton;
        protected Button playPauseButton;
        protected GameObject playIcon;
        protected GameObject pauseIcon;

        // Dependencies
        private TMP_Dropdown speedDropdown;
        private DungeonController dungeonController;
        private DungeonVisualizer dungeonVisualizer;
        private CameraFitter cameraFitter;
        private DungeonView dungeonView;
        
        [SetUp]
        protected virtual void BaseSetUp()
        {
            gameObject = new GameObject("DungeonLabUI");
            dungeonLabUI = gameObject.AddComponent<DungeonLabUI>();

            SetUpGenerationControls();
            SetUpSizeControls();
            SetUpPlaybackControls();
            SetUpDependencies();

            SetUpTest();
        }
        
        protected virtual void SetUpTest()
        {
        }
        
        [TearDown]
        protected virtual void TearDown()
        {
            Object.DestroyImmediate(gameObject);
        }
        
        private void SetUpGenerationControls()
        {
            algorithmDropdown = CreateComponent<TMP_Dropdown>("AlgorithmDropdown");
            widthInput = CreateComponent<TMP_InputField>("WidthInput");
            heightInput = CreateComponent<TMP_InputField>("HeightInput");
            seedInput = CreateComponent<TMP_InputField>("SeedInput");
            recordHistoryToggle = CreateComponent<Toggle>("RecordHistoryToggle");
            animateGenerationToggle = CreateComponent<Toggle>("AnimateGenerationToggle");
            generatedInfo = CreateComponent<TextMeshProUGUI>("GeneratedInfo");

            algorithmDropdown.AddOptions(
                System.Enum.GetNames(typeof(MazeAlgorithm)).ToList());

            AddPlaceholder(widthInput, "WidthPlaceholder");
            AddPlaceholder(heightInput, "HeightPlaceholder");
            AddPlaceholder(seedInput, "SeedPlaceholder");

            SetPrivateField("algorithmDropdown", algorithmDropdown);
            SetPrivateField("widthInput", widthInput);
            SetPrivateField("heightInput", heightInput);
            SetPrivateField("seedInput", seedInput);
            SetPrivateField("recordHistoryToggle", recordHistoryToggle);
            SetPrivateField("animateGenerationToggle", animateGenerationToggle);
            SetPrivateField("generatedInfo", generatedInfo);
        }
        
        private void SetUpSizeControls()
        {
            widthDecreaseButton = CreateComponent<Button>("WidthDecreaseButton");
            widthIncreaseButton = CreateComponent<Button>("WidthIncreaseButton");
            heightDecreaseButton = CreateComponent<Button>("HeightDecreaseButton");
            heightIncreaseButton = CreateComponent<Button>("HeightIncreaseButton");

            SetPrivateField("widthDecreaseButton", widthDecreaseButton);
            SetPrivateField("widthIncreaseButton", widthIncreaseButton);
            SetPrivateField("heightDecreaseButton", heightDecreaseButton);
            SetPrivateField("heightIncreaseButton", heightIncreaseButton);
        }
        
        private void SetUpPlaybackControls()
        {
            stepSlider = CreateComponent<Slider>("StepSlider");
            stepInfo = CreateComponent<TextMeshProUGUI>("StepInfo");
            previousStepButton = CreateComponent<Button>("PreviousStepButton");
            nextStepButton = CreateComponent<Button>("NextStepButton");
            playPauseButton = CreateComponent<Button>("PlayPauseButton");
            speedDropdown = CreateComponent<TMP_Dropdown>("SpeedDropdown");

            playIcon = CreateGameObject("PlayIcon");
            pauseIcon = CreateGameObject("PauseIcon");

            SetPrivateField("stepSlider", stepSlider);
            SetPrivateField("stepInfo", stepInfo);
            SetPrivateField("previousButton", previousStepButton);
            SetPrivateField("nextButton", nextStepButton);
            SetPrivateField("playPauseButton", playPauseButton);
            SetPrivateField("speedDropdown", speedDropdown);
            SetPrivateField("playIcon", playIcon);
            SetPrivateField("pauseIcon", pauseIcon);
        }

        private void SetUpDependencies()
        {
            dungeonController = gameObject.AddComponent<DungeonController>();
            dungeonVisualizer = gameObject.AddComponent<DungeonVisualizerMock>();
            cameraFitter = gameObject.AddComponent<CameraFitterMock>();
            dungeonView = gameObject.AddComponent<DungeonViewMock>();

            SetPrivateField("dungeonController", dungeonController);
            SetPrivateField("visualizer", dungeonVisualizer);
            SetPrivateField("cameraFitter", cameraFitter);
            SetPrivateField("dungeonView", dungeonView);
        }
        
        protected void GenerateWithHistory()
        {
            widthInput.text = "9";
            heightInput.text = "7";
            seedInput.text = "12345";
            recordHistoryToggle.isOn = true;
            animateGenerationToggle.isOn = false;
            algorithmDropdown.value = 0;

            dungeonLabUI.OnGenerateClicked();
        }
        
        protected void SetPrivateField(string fieldName, object value)
        {
            FieldInfo field = typeof(DungeonLabUI).GetField(
                fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);

            field.SetValue(dungeonLabUI, value);
        }
        
        private T CreateComponent<T>(string name) where T : Component
        {
            GameObject componentObject = new GameObject(name);
            componentObject.transform.SetParent(gameObject.transform);

            return componentObject.AddComponent<T>();
        }
        
        private GameObject CreateGameObject(string name)
        {
            GameObject componentObject = new GameObject(name);
            componentObject.transform.SetParent(gameObject.transform);
            return componentObject;
        }
        
        private void AddPlaceholder(TMP_InputField input, string name)
        {
            TMP_Text placeholder = CreateComponent<TextMeshProUGUI>(name);
            placeholder.transform.SetParent(input.transform);
            input.placeholder = placeholder;
        }
    }
}