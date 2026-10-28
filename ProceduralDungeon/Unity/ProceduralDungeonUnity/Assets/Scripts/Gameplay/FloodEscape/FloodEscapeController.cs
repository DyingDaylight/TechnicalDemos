using System;
using Core;
using DebugTools;
using Gameplay.FloodEscape.Gameplay;
using Gameplay.FloodEscape.Generation;
using Gameplay.FloodEscape.Rendering;
using Gameplay.FloodEscape.Water;
using Generation;
using Session;
using UI;
using UnityEngine;

namespace Gameplay.FloodEscape
{
    public class FloodEscapeController : MonoBehaviour
    {
        [SerializeField] private DungeonTilemapRenderer tilemapRenderer;
        [SerializeField] private CameraFitter cameraFitter;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private WaterRenderer waterRenderer;
        
        [Header("Diamond")]
        [SerializeField] private float targetDetourRatio = 0.5f;

        [Header("Prefabs")]
        [SerializeField] private DoorController doorPrefab;
        [SerializeField] private GameObject diamondPrefab;
        
        [Header("Water")]
        [SerializeField] private float waterStepInterval = 0.1f;
        [SerializeField] private float waterInflowPerStep = 0.1f;
        
#if UNITY_EDITOR
        [SerializeField] private bool useAsciiDebugMap;
        [SerializeField, TextArea(10, 25)] private string asciiDebugMap;
#endif
        
        private IReadOnlyDungeonMap map;
        
        private DungeonSequenceGenerator sequenceGenerator;
        private DoorController entranceDoor;
        private DoorController exitDoor;
        private DiamondPickup diamond;

        private WaterSimulation simulation;
        private float waterTimer;
        
#if UNITY_EDITOR
        private int waterStepCount;
#endif
        
        public IReadOnlyDungeonMap Map => map;
        
        private void Start()
        {
#if UNITY_EDITOR
            if (useAsciiDebugMap)
            {
                var debugMap = DebugDungeonFactory.CreateFromAscii(asciiDebugMap, out Vector2Int? waterSource);
                InitializeWaterDebugLevel(debugMap, waterSource);
                return;
            }
#endif
            
            DungeonSessionData sessionData = SessionData.ConsumeSessionData();
            
#if UNITY_EDITOR
            if (sessionData == null || sessionData.Map == null)
            {
                Debug.LogWarning("No dungeon session received. Using a debug map.");

                sessionData = new DungeonSessionData(
                    DebugDungeonFactory.Create(),
                    12345,
                    MazeAlgorithm.RecursiveBacktracker
                );
            }
#else
            if (sessionData == null || sessionData.Map == null)
            {
                Debug.LogError("Flood Escape started without dungeon session data.");
                // TODO: Decide how the gameplay scene should handle missing dungeon data in builds.
                return;
            }
#endif
            
            sequenceGenerator = new DungeonSequenceGenerator(sessionData.Map.Width, 
                sessionData.Map.Height, sessionData.Seed, sessionData.Algorithm);
            
            InitializeLevel(sessionData.Map);
        }

        private void Update()
        {
            if (simulation == null || waterStepInterval <= 0f)
                return;

            waterTimer += Time.deltaTime;

            while (waterTimer >= waterStepInterval)
            {
                waterTimer -= waterStepInterval;
                simulation.Step();
                
#if UNITY_EDITOR
                waterStepCount++;
#endif
            }

            waterRenderer.Refresh();
        }
        
        private void OnDestroy()
        {
            CleanupLevel();
        }

        protected virtual void DisplayMap(IReadOnlyDungeonMap map)
        {
            tilemapRenderer.Draw(map);

            cameraFitter.Fit(
                new Vector2(map.Width * 0.5f, map.Height * 0.5f),
                new Vector2(map.Width, map.Height)
            );
        }
        
        protected virtual void PlaceBarrier(Vector2Int position)
        {
            tilemapRenderer.PlaceBarrier(position);
        }
        
        private Vector2 CreateEntrance(DungeonMap mutableMap)
        {
            return CreateOpening(mutableMap, 1, 0);
        }

        private Vector2 CreateExit(DungeonMap mutableMap)
        {
            return CreateOpening(mutableMap, mutableMap.Height - 2, mutableMap.Height - 1);
        }

        private Vector2 CreateOpening(DungeonMap mutableMap, int innerY, int borderY)
        {
            int centerX = mutableMap.Width / 2;

            for (int offset = 0; offset < mutableMap.Width; offset++)
            {
                int rightX = centerX + offset;
                int leftX = centerX - offset;

                if (rightX < mutableMap.Width && mutableMap[rightX, innerY] == TileType.Floor)
                {
                    mutableMap[rightX, borderY] = TileType.Floor;
                    return new Vector2(rightX + 0.5f, borderY + 0.5f);
                }

                if (leftX >= 0 && mutableMap[leftX, innerY] == TileType.Floor)
                {
                    mutableMap[leftX, borderY] = TileType.Floor;
                    return new Vector2(leftX + 0.5f, borderY + 0.5f);
                }
            }
            
            throw new InvalidOperationException("Could not create dungeon opening.");
        }
        
        private void SpawnDoors(Vector2 entrancePosition, Vector2 exitPosition)
        {
            entranceDoor = Instantiate(doorPrefab, entrancePosition, 
                Quaternion.identity);
            entranceDoor.SetOpen(true);

            exitDoor = Instantiate(doorPrefab, exitPosition, Quaternion.identity);
            exitDoor.SetOpen(false);
            exitDoor.Entered += OnExitEntered;
        }
        
        private void SpawnDiamond(Vector2Int entrancePosition, Vector2Int exitPosition)
        {
            Vector2Int diamondPosition = DiamondPlacement.FindPosition(
                map, entrancePosition, exitPosition, targetDetourRatio);
            
            GameObject diamondObj = Instantiate(diamondPrefab,
                new Vector3(diamondPosition.x + 0.5f, diamondPosition.y + 0.5f, 0f),
                Quaternion.identity);
            
            diamond = diamondObj.GetComponent<DiamondPickup>();
            diamond.Collected += OnDiamondCollected;
        }
        
        private void OnDiamondCollected()
        {
            exitDoor.SetOpen(true);
        }
        
        private void OnExitEntered()
        {
            DungeonMap newMap = sequenceGenerator.GenerateNext();
            InitializeLevel(newMap);
        }

        private void InitializeLevel(IReadOnlyDungeonMap sourceMap)
        {
            CleanupLevel();
            
            DungeonMap gameplayMap = new DungeonMap(sourceMap);

            Vector2 entrancePosition = CreateEntrance(gameplayMap);
            Vector2 exitPosition = CreateExit(gameplayMap);
            
            map = gameplayMap;

            DisplayMap(map);
            playerController.transform.position = entrancePosition;
            
            Vector2Int entrance = Vector2Int.FloorToInt(entrancePosition);
            Vector2Int exit = Vector2Int.FloorToInt(exitPosition);
            
            PlaceBarrier(new Vector2Int(entrance.x, entrance.y - 1));
            PlaceBarrier(new Vector2Int(exit.x, exit.y + 1));
            
            SpawnDoors(entrancePosition, exitPosition);
            SpawnDiamond(entrance, exit);
            
            simulation = new WaterSimulation(gameplayMap, entrance.x, 0, waterInflowPerStep);

            waterRenderer.Initialize(simulation);
            waterRenderer.Refresh();

            waterTimer = 0f;
        }
        
#if UNITY_EDITOR
        private void InitializeWaterDebugLevel(IReadOnlyDungeonMap debugMap, Vector2Int? waterSource)
        {
            if (!waterSource.HasValue)
                return;
            
            waterStepCount = 0;
            
            CleanupLevel();

            map = debugMap;
            DisplayMap(map);

            playerController.gameObject.SetActive(false);

            simulation = new WaterSimulation(map, waterSource.Value.x, waterSource.Value.y, waterInflowPerStep);

            waterRenderer.Initialize(simulation);
            waterRenderer.Refresh();

            waterTimer = 0f;
        }
#endif
        
        private void CleanupLevel()
        {
            if (exitDoor != null)
            {
                exitDoor.Entered -= OnExitEntered;
                exitDoor.gameObject.SetActive(false);
                Destroy(exitDoor.gameObject);
                exitDoor = null;
            }

            if (entranceDoor != null)
            {
                entranceDoor.gameObject.SetActive(false);
                Destroy(entranceDoor.gameObject);
                entranceDoor = null;
            }

            if (diamond != null)
            {
                diamond.Collected -= OnDiamondCollected;
                diamond.gameObject.SetActive(false);
                Destroy(diamond.gameObject);
                diamond = null;
            }

            waterRenderer.Clear();
            simulation = null;
            waterTimer = 0f;
        }
        
#if UNITY_EDITOR
        private void OnGUI()
        {
            if (!useAsciiDebugMap)
                return;

            GUI.Label(
                new Rect(600, 10, 200, 60),
                $"Water step: {waterStepCount}");
        }
#endif
    }
}