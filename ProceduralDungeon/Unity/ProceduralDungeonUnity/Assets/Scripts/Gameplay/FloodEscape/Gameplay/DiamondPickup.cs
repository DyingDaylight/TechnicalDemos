using System;
using UnityEngine;

namespace Gameplay.FloodEscape.Gameplay
{
    public class DiamondPickup : MonoBehaviour
    {
        private bool isCollected;
    
        public event Action Collected;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isCollected || !other.CompareTag("Player"))
                return;

            isCollected = true;
            Collected?.Invoke();
            Destroy(gameObject);
        }
    }
}
