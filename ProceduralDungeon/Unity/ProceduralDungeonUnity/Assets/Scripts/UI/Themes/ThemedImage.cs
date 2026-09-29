using UnityEngine;
using UnityEngine.UI;

namespace UI.Themes
{
    [RequireComponent(typeof(Image))]
    public class ThemedImage : ThemedComponent
    {
        [SerializeField] private ThemeColor themeColor;
        protected ThemeColor ThemeColor => themeColor;
        
        protected override void ApplyThemeProperties()
        {
            Image image = GetComponent<Image>();
            image.color = Theme.GetColor(ThemeColor);
        }
    }
}