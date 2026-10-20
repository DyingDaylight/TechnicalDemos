using UnityEngine;

namespace Gameplay.FloodEscape.Water
{
    public readonly struct WaterFall
    {
        public Vector2Int Opening { get; }
        public Vector2Int Destination { get; }

        public WaterFall(Vector2Int opening, Vector2Int destination)
        {
            Opening = opening;
            Destination = destination;
        }
    }
}