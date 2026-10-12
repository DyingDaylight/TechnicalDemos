using System.Collections;
using Generation;
using NUnit.Framework;
using Session;
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
            
            DungeonSessionData session = SessionData.ConsumeSessionData();

            Assert.IsNotNull(session);
            Assert.AreSame(dungeonLabUI.GeneratedMap, session.Map);
            Assert.AreEqual(12345, session.Seed);
            Assert.AreEqual(MazeAlgorithm.RecursiveBacktracker, session.Algorithm);
            Assert.AreEqual("FloodEscape", dungeonLabUI.LoadedSceneName);
        }
        
        [Test]
        public void OnPlayButtonClicked_AfterChangingSettings_UsesGeneratedDungeonSettings()
        {
            GenerateWithHistory();

            // change settings without generating a new map
            seedInput.text = "9999";
            algorithmDropdown.value = (int)MazeAlgorithm.RandomizedPrim;

            gameplayDropdown.options.Clear();
            gameplayDropdown.options.Add(new TMP_Dropdown.OptionData("Flood Escape"));
            gameplayDropdown.value = 0;

            dungeonLabUI.OnPlayButtonClicked();

            DungeonSessionData session = SessionData.ConsumeSessionData();

            Assert.IsNotNull(session);
            Assert.AreSame(dungeonLabUI.GeneratedMap, session.Map);
            Assert.AreEqual(12345, session.Seed);
            Assert.AreEqual(MazeAlgorithm.RecursiveBacktracker, session.Algorithm);
        }
        
    }
}