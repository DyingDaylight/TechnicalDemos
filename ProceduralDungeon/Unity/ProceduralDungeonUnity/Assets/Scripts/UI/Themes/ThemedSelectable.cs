using UnityEngine;
using UnityEngine.UI;

namespace UI.Themes
{
    [RequireComponent(typeof(Selectable))]
    public class ThemedSelectable : ThemedComponent
    {
        [SerializeField] private Graphic[] additionalGraphics;
        
        private Selectable selectable;
        private bool wasInteractable;
        
        protected override void ApplyThemeProperties()
        {
            if (selectable == null)
                selectable = GetComponent<Selectable>();

            ColorBlock colors = selectable.colors;

            colors.normalColor = Color.white;
            colors.highlightedColor = Theme.GetColor(ThemeColor.Highlighted);
            colors.pressedColor = Theme.GetColor(ThemeColor.Pressed);
            colors.selectedColor = Theme.GetColor(ThemeColor.Selected);
            colors.disabledColor = Theme.GetColor(ThemeColor.Disabled);

            colors.colorMultiplier = 1f;

            selectable.colors = colors;
            
            wasInteractable = selectable.IsInteractable();
            ApplyAdditionalGraphics();
        }
        
        private void Update()
        {
            if (selectable == null || additionalGraphics == null)
                return;

            bool isInteractable = selectable.IsInteractable();

            if (isInteractable == wasInteractable)
                return;

            wasInteractable = isInteractable;
            ApplyAdditionalGraphics();
        }

        private void ApplyAdditionalGraphics()
        {
            if (additionalGraphics == null)
                return;
            
            ThemeColor color = wasInteractable
                ? ThemeColor.PrimaryText
                : ThemeColor.Disabled;

            Color themeColor = Theme.GetColor(color);

            foreach (Graphic graphic in additionalGraphics)
            {
                if (graphic != null)
                    graphic.color = themeColor;
            }
        }
    }
}