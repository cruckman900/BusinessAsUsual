using MudBlazor;

namespace BusinessAsUsual.Web.Theming;

/// <summary>
/// Centralized MudBlazor theme configuration for BusinessAsUsual.Web.
/// </summary>
/// <remarks>
/// Contributor: Christopher Ruckman  
/// Created: 2025-10-05  
/// Tags: #theme #mudblazor #ui #branding #palette #darkmode #lightmode  
/// </remarks>
public static class ThemeConfig
{
    /// <summary>
    /// Returns the Business As Usual dark theme.
    /// Uses deep grays and bold orange/blue branding for high contrast and contributor clarity.
    /// </summary>
    /// <returns>
    /// A configured <see cref="MudTheme"/> instance with customized <see cref="PaletteDark"/> settings.
    /// </returns>
    public static MudTheme GetDarkTheme()
    {
        return new MudTheme
        {
            PaletteDark = new PaletteDark
            {
                Primary = "#0057B8", // Business Blue
                Secondary = "#FF6B00", // Business Orange
                Background = Colors.Gray.Darken4,
                AppbarBackground = "#0057B8",
                DrawerBackground = Colors.Gray.Darken3,
                Surface = Colors.Gray.Darken2,
                TextPrimary = Colors.Gray.Lighten5,
                TextSecondary = Colors.Gray.Lighten2,
                ActionDefault = "#FF6B00",
                ActionDisabled = Colors.Gray.Darken1,
                Divider = Colors.Gray.Darken1,
                TableLines = Colors.Gray.Darken1,
                LinesDefault = Colors.Gray.Darken2,
                OverlayDark = Colors.Gray.Darken4
            }
        };
    }

    /// <summary>
    /// Returns the Business As Usual light theme.
    /// Uses soft grays and vibrant orange/blue branding for clarity and onboarding warmth.
    /// </summary>
    /// <returns>
    /// A configured <see cref="MudTheme"/> instance with customized <see cref="PaletteLight"/> settings.
    /// </returns>
    public static MudTheme GetLightTheme()
    {
        return new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#0057B8", // Business Blue
                Secondary = "#FF6B00", // Business Orange
                Background = Colors.Gray.Lighten5,
                AppbarBackground = "#0057B8",
                DrawerBackground = Colors.Gray.Lighten4,
                Surface = Colors.Gray.Lighten3,
                TextPrimary = Colors.Gray.Darken4,
                TextSecondary = Colors.Gray.Darken2,
                ActionDefault = "#FF6B00",
                ActionDisabled = Colors.Gray.Lighten1,
                Divider = Colors.Gray.Lighten2,
                TableLines = Colors.Gray.Lighten2,
                LinesDefault = Colors.Gray.Lighten3,
                OverlayLight = Colors.Gray.Lighten5
            }
        };
    }
}