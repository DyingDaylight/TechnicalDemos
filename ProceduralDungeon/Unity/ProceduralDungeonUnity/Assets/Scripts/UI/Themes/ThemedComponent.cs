using UnityEngine;

namespace UI.Themes
{
    [ExecuteAlways]
    public abstract class ThemedComponent : MonoBehaviour
    {
        [SerializeField] private ThemeColor themeColor;
        
        protected DungeonLabTheme Theme => theme;
        protected ThemeColor ThemeColor => themeColor;
        
        private ThemeProvider themeProvider;
        private DungeonLabTheme theme;

        protected virtual void OnEnable()
        {
            RefreshThemeProvider();
        }

        protected virtual void OnDisable()
        {
            UnsubscribeFromTheme();
            UnsubscribeFromThemeProvider();
            
            theme = null;
            themeProvider = null;
        }

        protected virtual void OnValidate()
        {
            RefreshThemeProvider();
        }

        private void RefreshThemeProvider()
        {
            ThemeProvider newThemeProvider = GetComponentInParent<ThemeProvider>();

            if (themeProvider != newThemeProvider)
            {
                UnsubscribeFromThemeProvider();
                
                themeProvider = newThemeProvider;
                
                if (themeProvider != null)
                    themeProvider.ThemeChanged += OnThemeChanged;
            }
            
            RefreshTheme();
        }
        
        private void RefreshTheme()
        {
            DungeonLabTheme newTheme = themeProvider != null
                ? themeProvider.Theme
                : null;

            if (theme != newTheme)
            {
                UnsubscribeFromTheme();

                theme = newTheme;

                if (theme != null)
                    theme.Changed += ApplyTheme;
            }

            ApplyTheme();
        }

        private void OnThemeChanged()
        {
            RefreshTheme();
        }

        private void UnsubscribeFromThemeProvider()
        {
            if (themeProvider != null)
                themeProvider.ThemeChanged -= OnThemeChanged;
        }
        
        private void UnsubscribeFromTheme()
        {
            if (theme != null)
                theme.Changed -= ApplyTheme;
        }
        
        private void ApplyTheme()
        {
            if (theme == null)
                return;

            ApplyThemeProperties();
        }
        
        protected abstract void ApplyThemeProperties();
    }
}