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
        
        protected IReadOnlyDungeonMap map;
        
        public IReadOnlyDungeonMap Map => map;
        
        private void Start()
        {
            map = SessionData.ConsumeDungeonMap();
            
            if (map == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("No dungeon map received. Using a debug map.");
                map = DebugDungeonFactory.Create();
#else
                Debug.LogError("Flood Escape started without a dungeon map.");
                // TODO: Decide how the gameplay scene should handle missing dungeon data in builds.
#endif
            }

            DisplayMap(map);
        }
        
        protected virtual void DisplayMap(IReadOnlyDungeonMap map)
        {
            tilemapRenderer.Draw(map);

            cameraFitter.Fit(
                new Vector2(map.Width * 0.5f, map.Height * 0.5f),
                new Vector2(map.Width, map.Height)
            );
        }
    }
}