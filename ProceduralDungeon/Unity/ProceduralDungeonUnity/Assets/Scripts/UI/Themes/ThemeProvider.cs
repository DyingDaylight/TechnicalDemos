using System;
using UnityEngine;

namespace UI.Themes
{
    public class ThemeProvider : MonoBehaviour
    {
        [SerializeField] private DungeonLabTheme theme;

        public DungeonLabTheme Theme => theme;
        
        public event Action ThemeChanged;

        private void OnValidate()
        {
            ThemeChanged?.Invoke();
        }
    }
}