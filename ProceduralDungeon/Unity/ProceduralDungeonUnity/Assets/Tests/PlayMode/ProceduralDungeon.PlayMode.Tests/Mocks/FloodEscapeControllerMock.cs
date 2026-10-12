using System.Collections.Generic;
using Core;
using Gameplay.FloodEscape;
using UnityEngine;

namespace Tests.Mocks
{
    public class FloodEscapeControllerMock : FloodEscapeController
    {
        public readonly List<Vector2Int> BarrierPositions = new();
        
        protected override void DisplayMap(IReadOnlyDungeonMap map)
        {
            
        }
        
        protected override void PlaceBarrier(Vector2Int position)
        {
            BarrierPositions.Add(position);
        }
    }
}