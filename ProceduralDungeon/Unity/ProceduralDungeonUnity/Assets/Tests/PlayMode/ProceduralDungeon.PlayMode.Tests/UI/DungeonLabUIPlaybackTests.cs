using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests.UI
{
    public class DungeonLabUIPlaybackTests : DungeonLabUITestBase
    {
        [Test]
        public void OnPreviousStepClicked_MovesToPreviousStep()
        {
            GenerateWithHistory();

            float lastStep = stepSlider.value;

            dungeonLabUI.OnPreviousStepClicked();
            
            Assert.AreEqual(lastStep - 1, stepSlider.value);
            StringAssert.Contains(
                $"Step: {lastStep} / {stepSlider.maxValue + 1}",
                stepInfo.text);

            Assert.IsTrue(previousStepButton.interactable);
            Assert.IsTrue(nextStepButton.interactable);
        }
        
        [Test]
        public void OnPreviousStepClicked_AtFirstStep_StaysAtFirstStep()
        {
            GenerateWithHistory();

            dungeonLabUI.OnStepSliderChanged(0);

            dungeonLabUI.OnPreviousStepClicked();

            Assert.AreEqual(0, stepSlider.value);
            StringAssert.StartsWith("Step: 1 /", stepInfo.text);

            Assert.IsFalse(previousStepButton.interactable);
            Assert.IsTrue(nextStepButton.interactable);
        }

        [Test]
        public void OnNextStepClicked_MovesToNextStep()
        {
            GenerateWithHistory();

            dungeonLabUI.OnStepSliderChanged(0);

            dungeonLabUI.OnNextStepClicked();

            Assert.AreEqual(1, stepSlider.value);
            StringAssert.StartsWith("Step: 2 /", stepInfo.text);

            Assert.IsTrue(previousStepButton.interactable);
            Assert.IsTrue(nextStepButton.interactable);
        }

        [Test]
        public void OnNextStepClicked_AtLastStep_StaysAtLastStep()
        {
            GenerateWithHistory();

            float lastStep = stepSlider.maxValue;

            dungeonLabUI.OnNextStepClicked();

            Assert.AreEqual(lastStep, stepSlider.value);
            StringAssert.Contains($"Step: {lastStep + 1} / {lastStep + 1}", stepInfo.text);

            Assert.IsTrue(previousStepButton.interactable);
            Assert.IsFalse(nextStepButton.interactable);
        }

        [Test]
        public void OnStepSliderChanged_MovesToSelectedStep()
        {
            GenerateWithHistory();

            dungeonLabUI.OnStepSliderChanged(2);

            Assert.AreEqual(2, stepSlider.value);
            StringAssert.StartsWith("Step: 3 /", stepInfo.text);

            Assert.IsTrue(previousStepButton.interactable);
            Assert.IsTrue(nextStepButton.interactable);
        }
        
        [UnityTest]
        public IEnumerator OnPlayPauseClicked_WhenStopped_StartsPlayback()
        {
            GenerateWithHistory();

            dungeonLabUI.OnStepSliderChanged(0);

            dungeonLabUI.OnPlayPauseClicked();

            Assert.IsFalse(playIcon.activeSelf);
            Assert.IsTrue(pauseIcon.activeSelf);

            yield return null;
        }

        [UnityTest]
        public IEnumerator OnPlayPauseClicked_WhenPlaying_StopsPlayback()
        {
            GenerateWithHistory();

            dungeonLabUI.OnStepSliderChanged(0);

            dungeonLabUI.OnPlayPauseClicked();
            dungeonLabUI.OnPlayPauseClicked();

            Assert.IsTrue(playIcon.activeSelf);
            Assert.IsFalse(pauseIcon.activeSelf);

            yield return null;
        }
    }
}