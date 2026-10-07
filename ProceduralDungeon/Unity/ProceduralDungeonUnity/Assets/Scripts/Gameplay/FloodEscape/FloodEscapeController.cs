using System;
using Core;
using DebugTools;
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
        
        
        protected IReadOnlyDungeonMap map;
        
        public IReadOnlyDungeonMap Map => map;
        
        private void Start()
        {
            IReadOnlyDungeonMap sourceMap = SessionData.ConsumeDungeonMap();
            if (sourceMap == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("No dungeon map received. Using a debug map.");
                sourceMap = DebugDungeonFactory.Create();
#else
                Debug.LogError("Flood Escape started without a dungeon map.");
                // TODO: Decide how the gameplay scene should handle missing dungeon data in builds.
#endif
            }
            
            DungeonMap gameplayMap = new DungeonMap(sourceMap);

            Vector2 entrancePosition = CreateEntrance(gameplayMap);
            Vector2 exitPosition = CreateExit(gameplayMap);

            map = gameplayMap;

            DisplayMap(map);
            playerController.transform.position = entrancePosition;
        }
        
        protected virtual void DisplayMap(IReadOnlyDungeonMap map)
        {
            tilemapRenderer.Draw(map);

            cameraFitter.Fit(
                new Vector2(map.Width * 0.5f, map.Height * 0.5f),
                new Vector2(map.Width, map.Height)
            );
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
    }
}