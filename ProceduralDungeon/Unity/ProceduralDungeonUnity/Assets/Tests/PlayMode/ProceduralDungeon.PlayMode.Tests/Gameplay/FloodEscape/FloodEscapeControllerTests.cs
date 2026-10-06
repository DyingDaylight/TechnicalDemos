using System.Collections;
using Core;
using Gameplay.FloodEscape;
using NUnit.Framework;
using Session;
using Tests.Mocks;
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
            
            FloodEscapeControllerMock controller = gameObject.AddComponent<FloodEscapeControllerMock>();

            yield return null;

            Assert.AreSame(map, controller.Map);
            Assert.IsNull(SessionData.ConsumeDungeonMap());
        }

        [UnityTest]
        public IEnumerator Start_WithoutDungeonMap_UsesDebugMapInEditor()
        {
            FloodEscapeControllerMock controller = gameObject.AddComponent<FloodEscapeControllerMock>();

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