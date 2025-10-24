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
    public static MudTheme GetDarkTheme() => new MudTheme
    {
        Palette = new PaletteDark
        {
            Primary = "#0057B8", // Business Blue
            Secondary = "#FF6B00", // Business Orange
            Background = Colors.Grey.Darken4,
            AppbarBackground = "#0057B8",
            DrawerBackground = Colors.Grey.Darken3,
            Surface = Colors.Grey.Darken2,
            TextPrimary = Colors.Grey.Lighten5,
            TextSecondary = Colors.Grey.Lighten2,
            ActionDefault = "#FF6B00",
            ActionDisabled = Colors.Grey.Darken1,
            Divider = Colors.Grey.Darken1,
            TableLines = Colors.Grey.Darken1,
            LinesDefault = Colors.Grey.Darken2,
            OverlayDark = Colors.Grey.Darken4
        }
    };

    /// <summary>
    /// Returns the Business As Usual light theme.
    /// Uses soft grays and vibrant orange/blue branding for clarity and onboarding warmth.
    /// </summary>
    public static MudTheme GetLightTheme() => new MudTheme
    {
        Palette = new PaletteLight
        {
            Primary = "#0057B8", // Business Blue
            Secondary = "#FF6B00", // Business Orange
            Background = Colors.Grey.Lighten5,
            AppbarBackground = "#0057B8",
            DrawerBackground = Colors.Grey.Lighten4,
            Surface = Colors.Grey.Lighten3,
            TextPrimary = Colors.Grey.Darken4,
            TextSecondary = Colors.Grey.Darken2,
            ActionDefault = "#FF6B00",
            ActionDisabled = Colors.Grey.Lighten1,
            Divider = Colors.Grey.Lighten2,
            TableLines = Colors.Grey.Lighten2,
            LinesDefault = Colors.Grey.Lighten3,
            OverlayLight = Colors.Grey.Lighten5
        }
    };
}