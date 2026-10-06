using System.Collections;
using Core;
using Gameplay.FloodEscape;
using NUnit.Framework;
using Session;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Gameplay.FloodEscape
{
    public class FloodEscapeControllerTests
    {
        private GameObject gameObject;

        [SetUp]
        public void SetUp()
        {
            SessionData.Clear();
            gameObject = new GameObject("FloodEscapeController");
        }

        [UnityTest]
        public IEnumerator Start_WithDungeonMap_ConsumesMapFromSession()
        {
            DungeonMap map = new DungeonMap(5, 5);
            SessionData.SetDungeonMap(map);
            
            FloodEscapeController controller = gameObject.AddComponent<FloodEscapeController>();

            yield return null;

            Assert.AreSame(map, controller.Map);
            Assert.IsNull(SessionData.ConsumeDungeonMap());
        }

        [UnityTest]
        public IEnumerator Start_WithoutDungeonMap_UsesDebugMapInEditor()
        {
            FloodEscapeController controller = gameObject.AddComponent<FloodEscapeController>();

            LogAssert.Expect(LogType.Warning,
                "No dungeon map received. Using a debug map.");
            
            yield return null;

            Assert.IsNotNull(controller.Map);
        }
        
        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameObject);
            SessionData.Clear();
        }
    }
}