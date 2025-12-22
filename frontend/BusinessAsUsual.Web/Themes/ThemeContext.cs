using BusinessAsUsual.Web.Themes;
using MudBlazor;

/// <summary>
/// Provides context for the application's current theme, including the selected theme name, dark mode setting, and the
/// active theme instance.
/// </summary>
/// <remarks>ThemeContext manages theme-related state for UI components and allows switching between themes at
/// runtime. It exposes an event to notify subscribers when the theme changes, enabling dynamic updates to the
/// application's appearance.</remarks>
public class ThemeContext
{
    /// <summary>
    /// Gets or sets the name of the current visual theme applied to the application.
    /// </summary>
    public string ThemeName { get; set; } = "light";

    /// <summary>
    /// Gets or sets a value indicating whether dark mode is enabled for the application interface.
    /// </summary>
    public bool IsDarkMode { get; set; } = false;

    /// <summary>
    /// Gets the currently selected theme for the application. If the specified theme is not found, the default "light"
    /// theme is returned.
    /// </summary>
    /// <remarks>This property provides access to the active theme based on the current theme name. If the
    /// theme name does not correspond to a registered theme, the property falls back to the default "light" theme to
    /// ensure consistent appearance.</remarks>
    public MudTheme CurrentTheme =>
        ThemeRegistry.Themes.TryGetValue(ThemeName, out var theme)
            ? theme
            : ThemeRegistry.Themes["light"];

    /// <summary>
    /// Represents a callback that is invoked when the application's theme changes.
    /// </summary>
    /// <remarks>Assign a delegate to this field to execute custom logic in response to theme changes. The
    /// callback is triggered whenever the theme is updated, allowing the application to react accordingly. This field
    /// is optional; if not set, no action is taken when the theme changes.</remarks>
    public Action? OnThemeChanged;

    /// <summary>
    /// Sets the current application theme by specifying its name and whether dark mode is enabled.
    /// </summary>
    /// <remarks>Invokes the theme change event after updating the theme settings. Changing the theme may
    /// affect the appearance of all UI elements.</remarks>
    /// <param name="name">The name of the theme to apply. Cannot be null or empty.</param>
    /// <param name="darkMode">A value indicating whether dark mode should be enabled. Set to <see langword="true"/> to enable dark mode;
    /// otherwise, <see langword="false"/>.</param>
    public void SetTheme(string name, bool darkMode)
    {
        ThemeName = name;
        IsDarkMode = darkMode;
        OnThemeChanged?.Invoke();
    }
}