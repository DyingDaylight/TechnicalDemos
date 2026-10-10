using System.Collections;
using Core;
using Gameplay.FloodEscape;
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
        private FloodEscapeControllerMock controller;
        
        private GameObject doorTemplate;
        private GameObject diamondTemplate;
        private Texture2D doorTexture;
        private Sprite doorOpenSprite;
        private Sprite doorClosedSprite;

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
            
            controller = CreateController();
            ConfigureGameplayPrefabs(controller);
            
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
            
            controller = CreateController();
            ConfigureGameplayPrefabs(controller);
            
            yield return null;

            Assert.IsNotNull(controller.Map);
        }
        
        [UnityTest]
        public IEnumerator Start_SpawnsDoorsWithCorrectInitialStates()
        {
            SessionData.SetDungeonMap(CreateTestMap());

            controller = CreateController();
            ConfigureGameplayPrefabs(controller);

            yield return null;

            DoorController entrance = FindSpawnedDoor(new Vector3(2.5f, 0.5f));
            DoorController exit = FindSpawnedDoor(new Vector3(2.5f, 4.5f));

            Assert.AreSame(
                doorOpenSprite,
                entrance.GetComponent<SpriteRenderer>().sprite);

            Assert.AreSame(
                doorClosedSprite,
                exit.GetComponent<SpriteRenderer>().sprite);
        }

        [UnityTest]
        public IEnumerator Start_SpawnsDiamondAtValidPosition()
        {
            SessionData.SetDungeonMap(CreateTestMap());

            controller = CreateController();
            ConfigureGameplayPrefabs(controller);

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
            SessionData.SetDungeonMap(CreateTestMap());

            controller = CreateController();
            ConfigureGameplayPrefabs(controller);

            yield return null;

            DoorController exit = FindSpawnedDoor(new Vector3(2.5f, 4.5f));
            DiamondPickup diamond = FindSpawnedDiamond();

            Assert.AreSame(
                doorClosedSprite,
                exit.GetComponent<SpriteRenderer>().sprite);

            // Trigger the real pickup behaviour.
            playerObject.tag = "Player";
            playerObject.AddComponent<CircleCollider2D>();

            CircleCollider2D diamondCollider =
                diamond.gameObject.AddComponent<CircleCollider2D>();
            diamondCollider.isTrigger = true;

            playerObject.transform.position = diamond.transform.position;

            yield return new WaitForFixedUpdate();

            Assert.AreSame(
                doorOpenSprite,
                exit.GetComponent<SpriteRenderer>().sprite);
        }
        
        [TearDown]
        public void TearDown()
        {
            foreach (DoorController door in Object.FindObjectsByType<DoorController>(FindObjectsSortMode.None))
            {
                if (door.gameObject != doorTemplate)
                    Object.DestroyImmediate(door.gameObject);
            }

            foreach (DiamondPickup diamond in Object.FindObjectsByType<DiamondPickup>(FindObjectsSortMode.None))
            {
                if (diamond.gameObject != diamondTemplate)
                    Object.DestroyImmediate(diamond.gameObject);
            }
            
            Object.DestroyImmediate(gameObject);
            Object.DestroyImmediate(playerObject);
            
            Object.DestroyImmediate(doorTemplate);
            Object.DestroyImmediate(diamondTemplate);
            Object.DestroyImmediate(doorOpenSprite);
            Object.DestroyImmediate(doorClosedSprite);
            Object.DestroyImmediate(doorTexture);
            
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
        
        private FloodEscapeControllerMock CreateController()
        {
            FloodEscapeControllerMock controller = gameObject.AddComponent<FloodEscapeControllerMock>();

            SerializedObject serializedController = new SerializedObject(controller);
            serializedController.FindProperty("playerController").objectReferenceValue = playerController;
            serializedController.ApplyModifiedProperties();

            return controller;
        }
        
        private void ConfigureGameplayPrefabs(FloodEscapeController controller)
        {
            // Door template
            doorTemplate = new GameObject("TestDoor");
            SpriteRenderer renderer = doorTemplate.AddComponent<SpriteRenderer>();
            DoorController doorController = doorTemplate.AddComponent<DoorController>();

            doorTexture = new Texture2D(2, 2);

            doorOpenSprite = Sprite.Create(
                doorTexture,
                new Rect(0, 0, 2, 2),
                new Vector2(0.5f, 0.5f));

            doorClosedSprite = Sprite.Create(
                doorTexture,
                new Rect(0, 0, 2, 2),
                new Vector2(0.5f, 0.5f));

            SerializedObject doorSerialized = new SerializedObject(doorController);
            doorSerialized.FindProperty("spriteRenderer").objectReferenceValue = renderer;
            doorSerialized.FindProperty("openSprite").objectReferenceValue = doorOpenSprite;
            doorSerialized.FindProperty("closedSprite").objectReferenceValue = doorClosedSprite;
            doorSerialized.ApplyModifiedPropertiesWithoutUndo();

            // Diamond template
            diamondTemplate = new GameObject("TestDiamond");
            diamondTemplate.AddComponent<DiamondPickup>();

            // Controller references
            SerializedObject controllerSerialized = new SerializedObject(controller);
            controllerSerialized.FindProperty("doorPrefab").objectReferenceValue = doorController;
            controllerSerialized.FindProperty("diamondPrefab").objectReferenceValue = diamondTemplate;
            controllerSerialized.ApplyModifiedPropertiesWithoutUndo();
        }
        
        private DoorController FindSpawnedDoor(Vector3 position)
        {
            foreach (DoorController door in Object.FindObjectsByType<DoorController>(
                         FindObjectsSortMode.None))
            {
                if (door.gameObject != doorTemplate &&
                    Vector3.Distance(door.transform.position, position) < 0.01f)
                {
                    return door;
                }
            }

            Assert.Fail($"Door not found at {position}");
            return null;
        }

        private DiamondPickup FindSpawnedDiamond()
        {
            foreach (DiamondPickup diamond in Object.FindObjectsByType<DiamondPickup>(
                         FindObjectsSortMode.None))
            {
                if (diamond.gameObject != diamondTemplate)
                    return diamond;
            }

            Assert.Fail("Spawned diamond not found");
            return null;
        }
    }
}