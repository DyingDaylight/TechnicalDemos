using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.Themes
{
    [CreateAssetMenu(
        fileName = "DungeonLabTheme",
        menuName = "Dungeon Lab/Theme")]
    public class DungeonLabTheme : ScriptableObject
    {
        [SerializeField] private List<ThemeColorEntry> colors;
        [SerializeField] private TMP_FontAsset font;

        public TMP_FontAsset Font => font;
        
        public event Action Changed;
        
        public Color GetColor(ThemeColor themeColor)
        {
            ThemeColorEntry entry = colors.Find(entry => entry.Type == themeColor);
            
            if (entry == null)
                throw new InvalidOperationException($"Color {themeColor} is not defined in theme {name}.");
            
            return entry.Color;
        }

        private void OnValidate()
        {
            Changed?.Invoke();
        }
        
        [Serializable]
        public class ThemeColorEntry
        {
            public ThemeColor Type;
            public Color Color;
        }
    }
}