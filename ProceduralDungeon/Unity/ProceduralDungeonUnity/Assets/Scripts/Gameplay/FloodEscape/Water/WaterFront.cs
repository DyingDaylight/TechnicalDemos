using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.FloodEscape.Water
{
    public class WaterFront
    {
        public Vector2Int EntryPoint { get; }
        public WaterFall? IncomingFall { get; }
        public List<Vector2Int> Cells { get; set; }
        
        public WaterFront(Vector2Int entryPoint, WaterFall? incomingFall = null)
        {
            EntryPoint = entryPoint;
            IncomingFall = incomingFall;
            Cells = new List<Vector2Int> { entryPoint };
        }
    }
}