using UnityEngine;
using UnityEngine.UI;

namespace UI.Themes
{
    [RequireComponent(typeof(Selectable))]
    public class ThemedSelectable : ThemedComponent
    {
        protected override void ApplyThemeProperties()
        {
            Selectable selectable = GetComponent<Selectable>();

            ColorBlock colors = selectable.colors;

            colors.normalColor = Color.white;
            colors.highlightedColor = Theme.GetColor(ThemeColor.Highlighted);
            colors.pressedColor = Theme.GetColor(ThemeColor.Pressed);
            colors.selectedColor = Theme.GetColor(ThemeColor.Selected);
            colors.disabledColor = Theme.GetColor(ThemeColor.Disabled);

            colors.colorMultiplier = 1f;

            selectable.colors = colors;
        }
    }
}