using Core;
using DebugTools;
using Session;
using UnityEngine;

namespace Gameplay.FloodEscape
{
    public class FloodEscapeController : MonoBehaviour
    {
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
#endif
            }
            
            Debug.Log("Map is read");
            Debug.Log(DungeonDebug.ToString(map));
        }
    }
}