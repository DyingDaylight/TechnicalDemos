using UnityEngine;
using UnityEngine.UI;

namespace UI.Themes
{
    [RequireComponent(typeof(Image))]
    public class ThemedImage : ThemedComponent
    {
        protected override void ApplyThemeProperties()
        {
            Image image = GetComponent<Image>();
            image.color = Theme.GetColor(ThemeColor);
        }
    }
}