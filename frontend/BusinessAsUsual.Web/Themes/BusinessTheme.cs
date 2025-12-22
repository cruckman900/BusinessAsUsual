using MudBlazor;

namespace BusinessAsUsual.Web.Themes
{
    /// <summary>
    /// Provides a predefined theme for business-oriented applications using MudBlazor components.
    /// </summary>
    /// <remarks>The theme includes customized color palettes for both light and dark modes, suitable for
    /// business application styling. Use this theme as a starting point for consistent branding and appearance across
    /// your application.</remarks>
    public static class BusinessTheme
    {
        /// <summary>
        /// Gets the default theme configuration used by MudBlazor components.
        /// </summary>
        /// <remarks>The default theme provides predefined color palettes and typography settings for both
        /// light and dark modes. Use this theme as a starting point for consistent styling across your application, or
        /// as a base for creating custom themes.</remarks>
        public static MudTheme Default = new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = "#FF8800",
                Secondary = "#0066CC",
                Background = "#F9F9F9",
                AppbarBackground = "#FF8800",
                DrawerBackground = "#FFFFFF"
            },
            PaletteDark = new PaletteDark()
            {
                Primary = "#FF8800",
                Secondary = "#66B2FF",
                Background = "#121212",
                AppbarBackground = "#1E1E1E",
                DrawerBackground = "#1E1E1E"
            },
            // Typography is now mostly fixed in 8.x
            // You can still reference slots, but not override font family/size here.
            Typography = new Typography()
        };
    }
}