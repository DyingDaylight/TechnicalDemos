using System.Collections;
using Core;
using NUnit.Framework;
using Session;
using Tests.Mocks;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Gameplay.FloodEscape
{
    public class FloodEscapeControllerTests
    {
        private GameObject gameObject;
        private GameObject playerObject;
        private PlayerController playerController;

        [SetUp]
        public void SetUp()
        {
            SessionData.Clear();

            gameObject = new GameObject("FloodEscapeController");

            playerObject = new GameObject("Player");
            playerObject.AddComponent<Rigidbody2D>().gravityScale = 0f;
            playerController = playerObject.AddComponent<PlayerController>();
        }

        [UnityTest]
        public IEnumerator Start_WithDungeonMap_PreparesGameplayMap()
        {
            DungeonMap map = CreateTestMap();
            SessionData.SetDungeonMap(map);

            FloodEscapeControllerMock controller = CreateController();
            
            yield return null;

            Assert.AreNotSame(map, controller.Map);
            Assert.IsNull(SessionData.ConsumeDungeonMap());
            Assert.AreEqual(TileType.Floor, controller.Map[2, 0]);
            Assert.AreEqual(TileType.Floor, controller.Map[2, 4]);
            Assert.AreEqual(new Vector3(2.5f, 0.5f, 0f), playerObject.transform.position);
        }

        [UnityTest]
        public IEnumerator Start_WithoutDungeonMap_UsesDebugMapInEditor()
        {
            LogAssert.Expect(LogType.Warning,
                "No dungeon map received. Using a debug map.");
            
            FloodEscapeControllerMock controller = CreateController();
            
            yield return null;

            Assert.IsNotNull(controller.Map);
        }
        
        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(gameObject);
            Object.DestroyImmediate(playerObject);
            SessionData.Clear();
        }
        
        private DungeonMap CreateTestMap()
        {
            DungeonMap map = new DungeonMap(5, 5);

            map[2, 1] = TileType.Floor;
            map[2, 2] = TileType.Floor;
            map[2, 3] = TileType.Floor;

            return map;
        }
        
        private FloodEscapeControllerMock CreateController()
        {
            FloodEscapeControllerMock controller = gameObject.AddComponent<FloodEscapeControllerMock>();

            SerializedObject serializedController = new SerializedObject(controller);
            serializedController.FindProperty("playerController").objectReferenceValue = playerController;
            serializedController.ApplyModifiedProperties();

            return controller;
        }
    }
}