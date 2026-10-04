using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine.TestTools;

namespace Tests.UI
{
    public class DungeonLabUIGameplayTests : DungeonLabUITestBase
    {
        [UnityTest]
        public IEnumerator Start_WithoutGeneratedDungeon_DisablesPlayButton()
        {
            yield return null;

            Assert.IsFalse(playButton.interactable);
        }
        
        [Test]
        public void OnGenerateClicked_WithGeneratedDungeon_EnablesPlayButton()
        {
            playButton.interactable = false;

            GenerateWithHistory();

            Assert.IsTrue(playButton.interactable);
        }
        
        [Test]
        public void OnPlayButtonClicked_WithFloodEscapeSelected_LoadsFloodEscapeScene()
        {
            GenerateWithHistory();

            gameplayDropdown.options.Clear();
            gameplayDropdown.options.Add(new TMP_Dropdown.OptionData("Flood Escape"));
            gameplayDropdown.value = 0;

            dungeonLabUI.OnPlayButtonClicked();

            Assert.AreEqual("FloodEscape", dungeonLabUI.LoadedSceneName);
        }
    }
}