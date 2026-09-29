using TMPro;
using UnityEngine;

namespace UI.Themes
{
    [RequireComponent(typeof(TMP_Text))]
    public class ThemedText : ThemedComponent
    {
        [SerializeField] private ThemeColor themeColor;
        
        protected ThemeColor ThemeColor => themeColor;
        
        protected override void ApplyThemeProperties()
        {
            TMP_Text text = GetComponent<TMP_Text>();

            text.color = Theme.GetColor(ThemeColor);
            text.font = Theme.Font;
        }
    }
}