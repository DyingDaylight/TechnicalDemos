using System;
using Core;
using DebugTools;
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

        [Header("Diamond Settings")]
        [SerializeField] private float targetDetourRatio = 0.5f;

        [Header("Prefabs")]
        [SerializeField] private DoorController doorPrefab;
        [SerializeField] private GameObject diamondPrefab;

        protected IReadOnlyDungeonMap map;

        private DungeonSequenceGenerator sequenceGenerator;
        private DoorController entranceDoor;
        private DoorController exitDoor;
        private DiamondPickup diamond;
        private bool hasDiamond;
        
        public IReadOnlyDungeonMap Map => map;
        
        private void Start()
        {
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
        
        private Vector2 CreateEntrance(DungeonMap map)
        {
            return CreateOpening(map, 1, 0);
        }

        private Vector2 CreateExit(DungeonMap map)
        {
            return CreateOpening(map, map.Height - 2, map.Height - 1);
        }

        private Vector2 CreateOpening(DungeonMap map, int innerY, int borderY)
        {
            int centerX = map.Width / 2;

            for (int offset = 0; offset < map.Width; offset++)
            {
                int rightX = centerX + offset;
                int leftX = centerX - offset;

                if (map.IsInside(rightX, innerY) &&
                    map[rightX, innerY] == TileType.Floor)
                {
                    map[rightX, borderY] = TileType.Floor;
                    return new Vector2(rightX + 0.5f, borderY + 0.5f);
                }

                if (map.IsInside(leftX, innerY) &&
                    map[leftX, innerY] == TileType.Floor)
                {
                    map[leftX, borderY] = TileType.Floor;
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
        
        private void SpawnDiamond(IReadOnlyDungeonMap map, Vector2Int entrancePosition,
            Vector2Int exitPosition)
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
            hasDiamond = true;
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
            SpawnDiamond(map, entrance, exit);
        }
        
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

            hasDiamond = false;
        }
    }
}