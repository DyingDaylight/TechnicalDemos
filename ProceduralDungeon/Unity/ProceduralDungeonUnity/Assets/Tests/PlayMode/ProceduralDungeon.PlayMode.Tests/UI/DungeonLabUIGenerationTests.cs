using Generation;
using NUnit.Framework;

namespace Tests.UI
{
    public class DungeonLabUIGenerationTests : DungeonLabUITestBase
    {
        [Test]
        public void OnRecordHistoryChanged_WhenEnabled_EnablesAnimateGeneration()
        {
            animateGenerationToggle.interactable = false;

            dungeonLabUI.OnRecordHistoryChanged(true);

            Assert.IsTrue(animateGenerationToggle.interactable);
        }
        
        [Test]
        public void OnRecordHistoryChanged_WhenDisabled_DisablesAnimateGeneration()
        {
            animateGenerationToggle.isOn = true;
            animateGenerationToggle.interactable = true;

            dungeonLabUI.OnRecordHistoryChanged(false);

            Assert.IsFalse(animateGenerationToggle.isOn);
            Assert.IsFalse(animateGenerationToggle.interactable);
        }
        
        [Test]
        public void OnGenerateClicked_WithExplicitSeed_ShowsSeedInGeneratedInfo()
        {
            widthInput.text = "9";
            heightInput.text = "7";
            seedInput.text = "12345";
            recordHistoryToggle.isOn = false;
            algorithmDropdown.value = 0;

            dungeonLabUI.OnGenerateClicked();

            StringAssert.Contains("12345", generatedInfo.text);
        }
        
        [Test]
        public void OnGenerateClicked_WithSpecifiedSize_ShowsSizeInGeneratedInfo()
        {
            widthInput.text = "9";
            heightInput.text = "7";
            seedInput.text = "12345";
            recordHistoryToggle.isOn = false;
            algorithmDropdown.value = 0;

            dungeonLabUI.OnGenerateClicked();

            StringAssert.Contains("(9 × 7)", generatedInfo.text);
        }

        [Test]
        public void OnGenerateClicked_WithSelectedAlgorithm_ShowsAlgorithmInGeneratedInfo()
        {
            widthInput.text = "9";
            heightInput.text = "7";
            seedInput.text = "12345";
            recordHistoryToggle.isOn = false;
            algorithmDropdown.value = (int)MazeAlgorithm.RandomizedPrim;

            dungeonLabUI.OnGenerateClicked();

            StringAssert.Contains("RandomizedPrim", generatedInfo.text);
        }

        [Test]
        public void OnGenerateClicked_WithoutHistory_DisablesPlaybackControls()
        {
            widthInput.text = "9";
            heightInput.text = "7";
            seedInput.text = "12345";
            recordHistoryToggle.isOn = false;
            algorithmDropdown.value = 0;

            dungeonLabUI.OnGenerateClicked();

            Assert.IsFalse(previousStepButton.interactable);
            Assert.IsFalse(nextStepButton.interactable);
            Assert.IsFalse(playPauseButton.interactable);
            Assert.AreEqual("Step: Final", stepInfo.text);
        }

        [Test]
        public void OnGenerateClicked_WithHistory_EnablesHistory()
        {
            widthInput.text = "9";
            heightInput.text = "7";
            seedInput.text = "12345";
            recordHistoryToggle.isOn = true;
            animateGenerationToggle.isOn = false;
            algorithmDropdown.value = 0;

            dungeonLabUI.OnGenerateClicked();

            Assert.Greater(stepSlider.maxValue, 0);
            Assert.AreEqual(stepSlider.maxValue, stepSlider.value);
            Assert.IsTrue(previousStepButton.interactable);
            Assert.IsFalse(nextStepButton.interactable);
            Assert.IsTrue(playPauseButton.interactable);
        }
        
        [Test]
        public void OnGenerateClicked_WithEmptySeed_ShowsGeneratedSeed()
        {
            widthInput.text = "9";
            heightInput.text = "7";
            seedInput.text = "";
            recordHistoryToggle.isOn = false;
            algorithmDropdown.value = 0;

            dungeonLabUI.OnGenerateClicked();

            string text = generatedInfo.text;
            int seedStart = text.IndexOf("Seed: ") + "Seed: ".Length;
            int seedEnd = text.IndexOf(" Time:", seedStart);
            string seedText = text.Substring(seedStart, seedEnd - seedStart);

            Assert.IsTrue(int.TryParse(seedText, out _));
        }
    }
}