using System.Collections;
using System.Linq;
using Core;
using Gameplay.FloodEscape.Gameplay;
using Gameplay.FloodEscape.Water;
using Generation;
using NUnit.Framework;
using Session;
using Tests.Mocks;
using Tests.Shared.Gameplay.FloodEscape;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Gameplay.FloodEscape
{
    public class FloodEscapeControllerTests
    {
        private FloodEscapeTestHelper helper;
        private FloodEscapeControllerMock controller;
        private PlayerController player;
        private DoorController doorPrefab;
        private DiamondPickup diamondPrefab;
        private GameObject waterObject;

        private Texture2D doorTexture;
        private Sprite openSprite;
        private Sprite closedSprite;
        
        
        [SetUp]
        public void SetUp()
        {
            SessionData.Clear();
            
            doorTexture = new Texture2D(2, 2);
            openSprite = Sprite.Create(doorTexture, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
            closedSprite = Sprite.Create(doorTexture, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
            
            helper = new FloodEscapeTestHelper();
            player = helper.CreatePlayer();
        }

        [UnityTest]
        public IEnumerator Start_WithDungeonMap_PreparesGameplayMap()
        {
            DungeonMap map = CreateTestMap();
            DungeonSessionData session = new DungeonSessionData(
                map, 12345, MazeAlgorithm.RecursiveBacktracker);
            SessionData.SetDungeonSessionData(session);
            PrepareController();
            
            yield return null;

            Assert.AreNotSame(map, controller.Map);
            Assert.IsNull(SessionData.ConsumeSessionData());
            Assert.AreEqual(TileType.Floor, controller.Map[2, 0]);
            Assert.AreEqual(TileType.Floor, controller.Map[2, 4]);
            Assert.AreEqual(new Vector3(2.5f, 0.5f, 0f), player.transform.position);
        }

        [UnityTest]
        public IEnumerator Start_WithoutDungeonMap_UsesDebugMapInEditor()
        {
            LogAssert.Expect(LogType.Warning,
                "No dungeon session received. Using a debug map.");
            
            PrepareController();
            
            yield return null;

            Assert.IsNotNull(controller.Map);
        }
        
        [UnityTest]
        public IEnumerator Start_SpawnsDoorsWithCorrectInitialStates()
        {
            DungeonMap map = CreateTestMap();
            DungeonSessionData session = new DungeonSessionData(
                map, 12345, MazeAlgorithm.RecursiveBacktracker);
            SessionData.SetDungeonSessionData(session);
            PrepareController();

            yield return null;

            DoorController entrance = FindSpawnedDoor(new Vector3(2.5f, 0.5f));
            DoorController exit = FindSpawnedDoor(new Vector3(2.5f, 4.5f));

            Assert.AreSame(openSprite, entrance.GetComponent<SpriteRenderer>().sprite);
            Assert.AreSame(closedSprite, exit.GetComponent<SpriteRenderer>().sprite);
        }

        [UnityTest]
        public IEnumerator Start_SpawnsDiamondAtValidPosition()
        {
            DungeonMap map = CreateTestMap();
            DungeonSessionData session = new DungeonSessionData(
                map, 12345, MazeAlgorithm.RecursiveBacktracker);
            SessionData.SetDungeonSessionData(session);
            PrepareController();

            yield return null;

            DiamondPickup diamond = FindSpawnedDiamond();

            Vector3 position = diamond.transform.position;
            int x = Mathf.FloorToInt(position.x);
            int y = Mathf.FloorToInt(position.y);

            Assert.AreEqual(TileType.Floor, controller.Map[x, y]);
            Assert.AreNotEqual(new Vector2Int(2, 0), new Vector2Int(x, y));
            Assert.AreNotEqual(new Vector2Int(2, 4), new Vector2Int(x, y));
        }

        [UnityTest]
        public IEnumerator CollectDiamond_OpensExitDoor()
        {
            DungeonMap map = CreateTestMap();
            DungeonSessionData session = new DungeonSessionData(
                map, 12345, MazeAlgorithm.RecursiveBacktracker);
            SessionData.SetDungeonSessionData(session);
            PrepareController();

            yield return null;

            DoorController exit = FindSpawnedDoor(new Vector3(2.5f, 4.5f));
            DiamondPickup diamond = FindSpawnedDiamond();

            Assert.AreSame(closedSprite, exit.GetComponent<SpriteRenderer>().sprite);

            player.transform.position = diamond.transform.position;

            yield return new WaitForFixedUpdate();

            Assert.AreSame(openSprite, exit.GetComponent<SpriteRenderer>().sprite);
            Assert.IsTrue(exit.GetComponent<Collider2D>().isTrigger);
        }
        
        [UnityTest]
        public IEnumerator Start_PlacesBarriersOutsideEntranceAndExit()
        {
            DungeonMap testMap = CreateTestMap();
            DungeonSessionData session = new DungeonSessionData(
                testMap, 12345, MazeAlgorithm.RecursiveBacktracker);
            SessionData.SetDungeonSessionData(session);

            PrepareController();

            yield return null;

            Assert.That(
                controller.BarrierPositions,
                Is.EquivalentTo(new[]
                {
                    new Vector2Int(2, -1),
                    new Vector2Int(2, 5)
                })
            );
        }
        
        [UnityTest]
        public IEnumerator EnterExit_GeneratesNextLevelAndMovesPlayer()
        {
            DungeonMap map = CreateTestMap();
            SessionData.SetDungeonSessionData(new DungeonSessionData(
                map, 12345, MazeAlgorithm.RecursiveBacktracker));

            PrepareController();
            yield return null;

            IReadOnlyDungeonMap firstMap = controller.Map;

            DoorController exit = FindSpawnedDoor(new Vector3(2.5f, 4.5f));
            DiamondPickup diamond = FindSpawnedDiamond();

            player.transform.position = diamond.transform.position;
            yield return new WaitForFixedUpdate();

            Assert.AreSame(openSprite, exit.GetComponent<SpriteRenderer>().sprite);

            player.transform.position = exit.transform.position;
            yield return new WaitForFixedUpdate();

            Assert.AreNotSame(firstMap, controller.Map);
            Assert.AreEqual(map.Width, controller.Map.Width);
            Assert.AreEqual(map.Height, controller.Map.Height);

            Vector2Int playerTile = Vector2Int.FloorToInt(player.transform.position);

            Assert.AreEqual(0, playerTile.y);
            Assert.AreEqual(TileType.Floor, controller.Map[playerTile.x, playerTile.y]);
        }
        
        [UnityTest]
        public IEnumerator EnterExit_ReplacesLevelObjectsAndResetsExit()
        {
            DungeonMap map = CreateTestMap();
            SessionData.SetDungeonSessionData(new DungeonSessionData(
                map, 12345, MazeAlgorithm.RecursiveBacktracker));

            PrepareController();
            yield return null;

            DoorController oldEntrance = FindSpawnedDoor(new Vector3(2.5f, 0.5f));
            DoorController oldExit = FindSpawnedDoor(new Vector3(2.5f, 4.5f));
            DiamondPickup oldDiamond = FindSpawnedDiamond();

            player.transform.position = oldDiamond.transform.position;
            yield return new WaitForFixedUpdate();

            player.transform.position = oldExit.transform.position;
            yield return new WaitForFixedUpdate();

            Assert.IsFalse(oldEntrance.gameObject.activeSelf);
            Assert.IsFalse(oldExit.gameObject.activeSelf);

            yield return null;

            Assert.IsTrue(oldEntrance == null);
            Assert.IsTrue(oldExit == null);
            Assert.IsTrue(oldDiamond == null);

            DoorController[] doors = Object.FindObjectsByType<DoorController>(
                FindObjectsSortMode.None);

            DoorController[] spawnedDoors = doors
                .Where(door => door != doorPrefab)
                .ToArray();

            Assert.AreEqual(2, spawnedDoors.Length);

            DoorController newExit = spawnedDoors
                .Single(door => door.transform.position.y > 1f);

            Assert.AreSame(closedSprite, newExit.GetComponent<SpriteRenderer>().sprite);

            DiamondPickup newDiamond = FindSpawnedDiamond();

            player.transform.position = newDiamond.transform.position;
            yield return new WaitForFixedUpdate();

            Assert.AreSame(openSprite, newExit.GetComponent<SpriteRenderer>().sprite);
        }
        
        [UnityTest]
        public IEnumerator Start_InitializesWaterSimulation()
        {
            DungeonMap map = CreateTestMap();

            SessionData.SetDungeonSessionData(new DungeonSessionData(
                map, 12345, MazeAlgorithm.RecursiveBacktracker));

            PrepareController();

            yield return null;

            Assert.IsNotNull(waterObject.transform.Find("Water_2_0"));
            Assert.IsNotNull(waterObject.transform.Find("Water_2_1"));
            Assert.IsNotNull(waterObject.transform.Find("Water_2_2"));
            Assert.IsNotNull(waterObject.transform.Find("Water_2_3"));
            Assert.IsNotNull(waterObject.transform.Find("Water_2_4"));

            Assert.IsNull(waterObject.transform.Find("Water_0_0"));
        }
        
        [UnityTest]
        public IEnumerator Update_AdvancesWaterSimulation()
        {
            DungeonMap map = CreateTestMap();

            SessionData.SetDungeonSessionData(new DungeonSessionData(
                map, 12345, MazeAlgorithm.RecursiveBacktracker));

            PrepareController();

            SerializedObject serialized = new SerializedObject(controller);
            serialized.FindProperty("waterStepInterval").floatValue = 0.02f;
            serialized.FindProperty("waterInflowPerStep").floatValue = 0.1f;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            yield return null;

            Transform waterCell = waterObject.transform.Find("Water_2_0");

            Assert.IsNotNull(waterCell);

            yield return new WaitForSeconds(0.1f);

            Assert.IsTrue(waterCell.gameObject.activeSelf);
            Assert.Greater(waterCell.localScale.y, 0f);
        }
        
        [UnityTest]
        public IEnumerator EnterExit_ResetsWaterSimulation()
        {
            DungeonMap map = CreateTestMap();

            SessionData.SetDungeonSessionData(new DungeonSessionData(
                map, 12345, MazeAlgorithm.RecursiveBacktracker));

            PrepareController();

            SerializedObject serialized = new SerializedObject(controller);
            serialized.FindProperty("waterStepInterval").floatValue = 0.02f;
            serialized.FindProperty("waterInflowPerStep").floatValue = 0.1f;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            yield return null;

            Transform oldWater = waterObject.transform.Find("Water_2_0");

            Assert.IsNotNull(oldWater);

            yield return new WaitForSeconds(0.1f);

            Assert.IsTrue(oldWater.gameObject.activeSelf);

            DoorController exit = FindSpawnedDoor(new Vector3(2.5f, 4.5f));
            DiamondPickup diamond = FindSpawnedDiamond();

            player.transform.position = diamond.transform.position;
            yield return new WaitForFixedUpdate();

            player.transform.position = exit.transform.position;
            yield return new WaitForFixedUpdate();

            Assert.AreNotSame(map, controller.Map);

            // The previous level's water objects should be deactivated.
            Assert.IsFalse(oldWater.gameObject.activeSelf);

            // The new level should have its own water objects.
            Assert.Greater(waterObject.transform.childCount, 0);
        }
        
        [TearDown]
        public void TearDown()
        {
            foreach (DoorController door in Object.FindObjectsByType<DoorController>(FindObjectsSortMode.None))
            {
                if (door != doorPrefab)
                    Object.DestroyImmediate(door.gameObject);
            }

            foreach (DiamondPickup diamond in Object.FindObjectsByType<DiamondPickup>(FindObjectsSortMode.None))
            {
                if (diamond != diamondPrefab)
                    Object.DestroyImmediate(diamond.gameObject);
            }
            
            if (controller != null)
                Object.DestroyImmediate(controller.gameObject);
            
            Object.DestroyImmediate(openSprite);
            Object.DestroyImmediate(closedSprite);
            Object.DestroyImmediate(doorTexture);
            Object.DestroyImmediate(waterObject);
            
            helper.Dispose();
            
            SessionData.Clear();
        }
        
        private DungeonMap CreateTestMap()
        {
            DungeonMap map = new DungeonMap(5, 5);

            map[2, 1] = TileType.Floor;
            map[2, 2] = TileType.Floor;
            map[2, 3] = TileType.Floor;

            // Dead-end branch for diamond placement
            map[1, 2] = TileType.Floor;

            return map;
        }
        
        private DoorController FindSpawnedDoor(Vector3 position)
        {
            return Object.FindObjectsByType<DoorController>(FindObjectsSortMode.None)
                .Single(door =>
                    door != doorPrefab &&
                    Vector3.Distance(door.transform.position, position) < 0.01f);
        }

        private DiamondPickup FindSpawnedDiamond()
        {
            return Object.FindObjectsByType<DiamondPickup>(FindObjectsSortMode.None)
                .Single(diamond => diamond != diamondPrefab);
        }
        
        private void PrepareController()
        {
            GameObject controllerObject = new GameObject("FloodEscapeController");
            controller = controllerObject.AddComponent<FloodEscapeControllerMock>();
            
            waterObject = new GameObject("WaterRenderer");
            WaterRenderer waterRenderer = waterObject.AddComponent<WaterRenderer>();
            
            doorPrefab = helper.CreateDoor(openSprite, closedSprite);
            diamondPrefab = helper.CreateDiamond();

            SerializedObject serialized = new SerializedObject(controller);

            serialized.FindProperty("playerController").objectReferenceValue = player;
            serialized.FindProperty("doorPrefab").objectReferenceValue = doorPrefab;
            serialized.FindProperty("diamondPrefab").objectReferenceValue = diamondPrefab.gameObject;
            serialized.FindProperty("waterRenderer").objectReferenceValue = waterRenderer;


            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}