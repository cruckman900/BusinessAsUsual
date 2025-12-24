using MudBlazor;
using System.Collections.Generic;
using static MudBlazor.CategoryTypes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessAsUsual.Web.Themes
{
    /// <summary>
    /// Provides a centralized registry of predefined themes for use throughout the application.
    /// </summary>
    /// <remarks>The registry includes a collection of named themes, such as "light", "dark", and "hazard",
    /// each associated with a corresponding MudTheme instance. This class is intended to simplify theme management and
    /// selection by offering a standard set of themes that can be referenced by name.</remarks>
    public static class ThemeRegistry
    {
        /// <summary>
        /// Provides a collection of predefined MudTheme instances, each representing a named color theme for use in UI
        /// components.
        /// </summary>
        /// <remarks>The dictionary includes a variety of themes such as "light", "dark", "hazard", and
        /// others, each with distinct color palettes for both light and dark modes. Use the theme name as the key to
        /// retrieve the corresponding MudTheme. This collection is intended to simplify theme selection and ensure
        /// consistency across the application.</remarks>
        public static readonly Dictionary<string, MudTheme> Themes = new()
        {
            // additional options we can populate later if needed
            //- Tertiary
            //- Dark
            //- DarkContrastText
            //- GrayLight
            //- GrayLighter
            //- GrayDark
            //- GrayDarker
            //- TableLines
            //- Divider
            //- OverlayLight
            //- OverlayDark


            ["light"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#1976d2",
                    Secondary = "#9c27b0",
                    TextPrimary = "#212121",
                    TextSecondary = "#616161",
                    PrimaryContrastText = "#FFFFFF",
                    SecondaryContrastText = "#FFFFFF",
                    Background = "#f5f5f5",
                    AppbarBackground = "#1976d2",
                    DrawerBackground = "#ffffff",
                    Success = "#4caf50",
                    Warning = "#ff9800",
                    Error = "#f44336",
                    Info = "#0288d1"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#90caf9",
                    Secondary = "#ce93d8",
                    TextPrimary = "#FFFFFF",
                    TextSecondary = "#B0BEC5",
                    PrimaryContrastText = "#000000",
                    SecondaryContrastText = "#FFFFFF",
                    Background = "#121212",
                    AppbarBackground = "#0d47a1",
                    DrawerBackground = "#1a1a1a",
                    Success = "#66bb6a",
                    Warning = "#ffa726",
                    Error = "#ef5350",
                    Info = "#29b6f6"
                }
            },

            ["hazard"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#ff0000",       // blazing red
                    Secondary = "#ffc107",     // hazard yellow
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#fff8e1",    // pale warning backdrop
                    AppbarBackground = "#ff0000",
                    DrawerBackground = "#fff8e1",
                    Success = "#4caf50",
                    Warning = "#ffc107",
                    Error = "#f44336",
                    Info = "#2196f3"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#ff5252",       // softer red glow
                    Secondary = "#ffd54f",     // muted hazard yellow
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#1a1a1a",
                    AppbarBackground = "#b71c1c",
                    DrawerBackground = "#212121",
                    Success = "#81c784",
                    Warning = "#ffb74d",
                    Error = "#e57373",
                    Info = "#64b5f6"
                }
            },

            ["midnight"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#0d47a1",       // deep midnight blue
                    Secondary = "#673ab7",     // purple accent
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#e3f2fd",    // pale sky
                    AppbarBackground = "#0d47a1",
                    DrawerBackground = "#ffffff",
                    Success = "#388e3c",
                    Warning = "#fbc02d",
                    Error = "#d32f2f",
                    Info = "#1976d2"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#90caf9",       // soft blue glow
                    Secondary = "#9575cd",     // muted purple
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#0a0a0f",    // near‑black
                    AppbarBackground = "#0d47a1",
                    DrawerBackground = "#121212",
                    Success = "#66bb6a",
                    Warning = "#ffeb3b",
                    Error = "#ef5350",
                    Info = "#29b6f6"
                }
            },

            ["brownstone"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#8D6E63",       // brownstone brick
                    Secondary = "#A1887F",     // soft taupe
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#FAF3E0",    // parchment backdrop
                    AppbarBackground = "#6D4C41",
                    DrawerBackground = "#FFFFFF",
                    Success = "#4CAF50",
                    Warning = "#FFB300",
                    Error = "#D32F2F",
                    Info = "#1976D2"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#BCAAA4",       // muted stone
                    Secondary = "#D7CCC8",     // light clay
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#2E2E2E",
                    AppbarBackground = "#4E342E",
                    DrawerBackground = "#212121",
                    Success = "#81C784",
                    Warning = "#FFB74D",
                    Error = "#E57373",
                    Info = "#64B5F6"
                }
            },

            ["slate"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#607D8B",       // slate blue-gray
                    Secondary = "#455A64",     // darker slate
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#ECEFF1",    // light gray
                    AppbarBackground = "#607D8B",
                    DrawerBackground = "#FFFFFF",
                    Success = "#388E3C",
                    Warning = "#FBC02D",
                    Error = "#D32F2F",
                    Info = "#0288D1"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#90A4AE",       // soft slate glow
                    Secondary = "#78909C",     // muted gray
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#1C1C1C",
                    AppbarBackground = "#37474F",
                    DrawerBackground = "#121212",
                    Success = "#66BB6A",
                    Warning = "#FFEB3B",
                    Error = "#EF5350",
                    Info = "#29B6F6"
                }
            },

            ["purple"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#9C27B0",       // vivid purple
                    Secondary = "#7B1FA2",     // deep accent
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#F3E5F5",    // lavender backdrop
                    AppbarBackground = "#6A1B9A",
                    DrawerBackground = "#FFFFFF",
                    Success = "#4CAF50",
                    Warning = "#FF9800",
                    Error = "#F44336",
                    Info = "#2196F3"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#CE93D8",       // soft glow purple
                    Secondary = "#BA68C8",     // muted accent
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#1A1A1A",
                    AppbarBackground = "#4A148C",
                    DrawerBackground = "#212121",
                    Success = "#81C784",
                    Warning = "#FFB74D",
                    Error = "#E57373",
                    Info = "#64B5F6"
                }
            },

            ["pink"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#E91E63",       // hot pink
                    Secondary = "#F06292",     // lighter accent
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#FCE4EC",    // blush backdrop
                    AppbarBackground = "#C2185B",
                    DrawerBackground = "#FFFFFF",
                    Success = "#4CAF50",
                    Warning = "#FF9800",
                    Error = "#F44336",
                    Info = "#2196F3"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#F48FB1",       // soft pink glow
                    Secondary = "#EC407A",     // vibrant accent
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#1C1C1C",
                    AppbarBackground = "#880E4F",
                    DrawerBackground = "#212121",
                    Success = "#81C784",
                    Warning = "#FFB74D",
                    Error = "#E57373",
                    Info = "#64B5F6"
                }
            },

            ["emerald"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#2E7D32",       // rich emerald green
                    Secondary = "#66BB6A",     // lighter accent
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#E8F5E9",    // pale green backdrop
                    AppbarBackground = "#1B5E20",
                    DrawerBackground = "#FFFFFF",
                    Success = "#388E3C",
                    Warning = "#FBC02D",
                    Error = "#D32F2F",
                    Info = "#1976D2"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#81C784",       // soft emerald glow
                    Secondary = "#A5D6A7",     // muted accent
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#1A1A1A",
                    AppbarBackground = "#2E7D32",
                    DrawerBackground = "#212121",
                    Success = "#66BB6A",
                    Warning = "#FFEB3B",
                    Error = "#EF5350",
                    Info = "#64B5F6"
                }
            },

            ["ocean"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#0288D1",       // ocean blue
                    Secondary = "#26C6DA",     // aqua accent
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#E1F5FE",    // pale sky backdrop
                    AppbarBackground = "#0277BD",
                    DrawerBackground = "#FFFFFF",
                    Success = "#388E3C",
                    Warning = "#FBC02D",
                    Error = "#D32F2F",
                    Info = "#1976D2"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#4FC3F7",       // soft ocean glow
                    Secondary = "#80DEEA",     // muted aqua
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#0A0A0F",
                    AppbarBackground = "#01579B",
                    DrawerBackground = "#121212",
                    Success = "#66BB6A",
                    Warning = "#FFEB3B",
                    Error = "#EF5350",
                    Info = "#29B6F6"
                }
            },

            ["sunset"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#FF7043",       // sunset orange
                    Secondary = "#FFB74D",     // golden glow
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#FFF3E0",    // pale sky
                    AppbarBackground = "#F4511E",
                    DrawerBackground = "#FFFFFF",
                    Success = "#4CAF50",
                    Warning = "#FBC02D",
                    Error = "#D32F2F",
                    Info = "#1976D2"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#FF8A65",       // softer orange glow
                    Secondary = "#FFD180",     // muted golden
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#1A1A1A",
                    AppbarBackground = "#BF360C",
                    DrawerBackground = "#212121",
                    Success = "#81C784",
                    Warning = "#FFB74D",
                    Error = "#E57373",
                    Info = "#64B5F6"
                }
            },

            ["steel"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#546E7A",       // steel gray-blue
                    Secondary = "#78909C",     // lighter accent
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#ECEFF1",    // light industrial backdrop
                    AppbarBackground = "#455A64",
                    DrawerBackground = "#FFFFFF",
                    Success = "#388E3C",
                    Warning = "#FBC02D",
                    Error = "#D32F2F",
                    Info = "#0288D1"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#90A4AE",       // soft steel glow
                    Secondary = "#B0BEC5",     // muted accent
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#1C1C1C",
                    AppbarBackground = "#263238",
                    DrawerBackground = "#121212",
                    Success = "#66BB6A",
                    Warning = "#FFEB3B",
                    Error = "#EF5350",
                    Info = "#29B6F6"
                }
            },

            ["forest"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#1B5E20",       // deep forest green
                    Secondary = "#4CAF50",     // vibrant leaf
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#E8F5E9",    // pale green backdrop
                    AppbarBackground = "#2E7D32",
                    DrawerBackground = "#FFFFFF",
                    Success = "#388E3C",
                    Warning = "#FBC02D",
                    Error = "#D32F2F",
                    Info = "#1976D2"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#66BB6A",       // soft forest glow
                    Secondary = "#81C784",     // muted leaf
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#1A1A1A",
                    AppbarBackground = "#1B5E20",
                    DrawerBackground = "#212121",
                    Success = "#81C784",
                    Warning = "#FFB74D",
                    Error = "#E57373",
                    Info = "#64B5F6"
                }
            },

            ["ice"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#00BCD4",       // icy cyan
                    Secondary = "#B2EBF2",     // frosty accent
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#E0F7FA",    // pale ice backdrop
                    AppbarBackground = "#00ACC1",
                    DrawerBackground = "#FFFFFF",
                    Success = "#388E3C",
                    Warning = "#FBC02D",
                    Error = "#D32F2F",
                    Info = "#0288D1"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#80DEEA",       // soft ice glow
                    Secondary = "#B2EBF2",     // frosty accent
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#0A0A0F",
                    AppbarBackground = "#006064",
                    DrawerBackground = "#121212",
                    Success = "#66BB6A",
                    Warning = "#FFEB3B",
                    Error = "#EF5350",
                    Info = "#29B6F6"
                }
            },

            ["crimson"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#B71C1C",       // deep crimson red
                    Secondary = "#E53935",     // vibrant accent
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#FFEBEE",    // pale blush backdrop
                    AppbarBackground = "#B71C1C",
                    DrawerBackground = "#FFFFFF",
                    Success = "#4CAF50",
                    Warning = "#FBC02D",
                    Error = "#D32F2F",
                    Info = "#1976D2"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#EF5350",       // glowing crimson
                    Secondary = "#E57373",     // muted accent
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#1A1A1A",
                    AppbarBackground = "#7F0000",
                    DrawerBackground = "#212121",
                    Success = "#81C784",
                    Warning = "#FFB74D",
                    Error = "#E57373",
                    Info = "#64B5F6"
                }
            },

            ["gold"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#FFD700",       // pure gold
                    Secondary = "#FFC107",     // golden accent
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#FFFDE7",    // pale gold backdrop
                    AppbarBackground = "#FFD700",
                    DrawerBackground = "#FFFFFF",
                    Success = "#4CAF50",
                    Warning = "#F57C00",
                    Error = "#D32F2F",
                    Info = "#1976D2"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#FFEB3B",       // glowing gold
                    Secondary = "#FBC02D",     // muted accent
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#1C1C1C",
                    AppbarBackground = "#FFA000",
                    DrawerBackground = "#121212",
                    Success = "#81C784",
                    Warning = "#FFB74D",
                    Error = "#E57373",
                    Info = "#64B5F6"
                }
            },

            ["teal"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#009688",       // vivid teal
                    Secondary = "#4DB6AC",     // lighter accent
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#E0F2F1",    // pale teal backdrop
                    AppbarBackground = "#00796B",
                    DrawerBackground = "#FFFFFF",
                    Success = "#388E3C",
                    Warning = "#FBC02D",
                    Error = "#D32F2F",
                    Info = "#1976D2"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#4DB6AC",       // glowing teal
                    Secondary = "#80CBC4",     // muted accent
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#0A0A0F",
                    AppbarBackground = "#004D40",
                    DrawerBackground = "#121212",
                    Success = "#66BB6A",
                    Warning = "#FFEB3B",
                    Error = "#EF5350",
                    Info = "#29B6F6"
                }
            },

            ["copper"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#B87333",       // classic copper
                    Secondary = "#D2691E",     // rust accent
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#FFF8E1",    // pale metallic backdrop
                    AppbarBackground = "#8B4513",
                    DrawerBackground = "#FFFFFF",
                    Success = "#4CAF50",
                    Warning = "#FF9800",
                    Error = "#D32F2F",
                    Info = "#1976D2"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#CD7F32",       // glowing copper
                    Secondary = "#A0522D",     // muted rust
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#1A1A1A",
                    AppbarBackground = "#5D4037",
                    DrawerBackground = "#212121",
                    Success = "#81C784",
                    Warning = "#FFB74D",
                    Error = "#E57373",
                    Info = "#64B5F6"
                }
            },

            ["indigo"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#3F51B5",       // indigo blue
                    Secondary = "#5C6BC0",     // lighter accent
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#E8EAF6",    // pale indigo backdrop
                    AppbarBackground = "#303F9F",
                    DrawerBackground = "#FFFFFF",
                    Success = "#388E3C",
                    Warning = "#FBC02D",
                    Error = "#D32F2F",
                    Info = "#0288D1"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#7986CB",       // glowing indigo
                    Secondary = "#9FA8DA",     // muted accent
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#1C1C1C",
                    AppbarBackground = "#1A237E",
                    DrawerBackground = "#121212",
                    Success = "#66BB6A",
                    Warning = "#FFEB3B",
                    Error = "#EF5350",
                    Info = "#29B6F6"
                }
            },

            ["ruby"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#9B111E",       // deep ruby red
                    Secondary = "#E53935",     // vibrant accent
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#FFEBEE",    // pale blush backdrop
                    AppbarBackground = "#8B0000",
                    DrawerBackground = "#FFFFFF",
                    Success = "#4CAF50",
                    Warning = "#FBC02D",
                    Error = "#C62828",
                    Info = "#1976D2"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#E57373",       // glowing ruby
                    Secondary = "#EF5350",     // muted accent
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#1A1A1A",
                    AppbarBackground = "#7F0000",
                    DrawerBackground = "#212121",
                    Success = "#81C784",
                    Warning = "#FFB74D",
                    Error = "#E57373",
                    Info = "#64B5F6"
                }
            },

            ["sky"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#03A9F4",       // sky blue
                    Secondary = "#81D4FA",     // lighter accent
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#E1F5FE",    // pale sky backdrop
                    AppbarBackground = "#0288D1",
                    DrawerBackground = "#FFFFFF",
                    Success = "#388E3C",
                    Warning = "#FBC02D",
                    Error = "#D32F2F",
                    Info = "#1976D2"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#4FC3F7",       // glowing sky
                    Secondary = "#81D4FA",     // muted accent
                    TextPrimary = "#FFFFFF",          // crisp white
                    TextSecondary = "#B0BEC5",        // soft gray, still visible
                    PrimaryContrastText = "#000000",  // black text on glowing silver/pearl primaries
                    SecondaryContrastText = "#FFFFFF", // white text on dark secondary
                    Background = "#0A0A0F",
                    AppbarBackground = "#01579B",
                    DrawerBackground = "#121212",
                    Success = "#66BB6A",
                    Warning = "#FFEB3B",
                    Error = "#EF5350",
                    Info = "#29B6F6"
                }
            },

            ["amber"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#FFB300",       // amber gold
                    Secondary = "#FFA000",     // deeper accent
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#FFF8E1",    // pale amber backdrop
                    AppbarBackground = "#FF8F00",
                    DrawerBackground = "#FFFFFF",
                    Success = "#4CAF50",
                    Warning = "#F57C00",
                    Error = "#D32F2F",
                    Info = "#1976D2"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#FFC107",       // glowing amber
                    Secondary = "#FFD54F",     // muted accent
                    TextPrimary = "#FFFFFF",
                    TextSecondary = "#B0BEC5",
                    Background = "#1C1C1C",
                    AppbarBackground = "#FF6F00",
                    DrawerBackground = "#121212",
                    Success = "#81C784",
                    Warning = "#FFB74D",
                    Error = "#E57373",
                    Info = "#64B5F6"
                }
            },

            ["jade"] = new MudTheme()
            {
                PaletteLight = new PaletteLight
                {
                    Primary = "#00A86B",       // jade green
                    Secondary = "#4DB6AC",     // lighter accent
                    TextPrimary = "#212121",          // near‑black, always readable
                    TextSecondary = "#616161",        // medium gray, not too faint
                    PrimaryContrastText = "#FFFFFF",  // white text on colored/primary surfaces
                    SecondaryContrastText = "#212121", // dark text on pale secondary
                    Background = "#E0F2F1",    // pale jade backdrop
                    AppbarBackground = "#00796B",
                    DrawerBackground = "#FFFFFF",
                    Success = "#388E3C",
                    Warning = "#FBC02D",
                    Error = "#D32F2F",
                    Info = "#1976D2"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#4DB6AC",       // glowing jade
                    Secondary = "#80CBC4",     // muted accent
                    TextPrimary = "#FFFFFF",
                    TextSecondary = "#B0BEC5",
                    Background = "#0A0A0F",
                    AppbarBackground = "#004D40",
                    DrawerBackground = "#121212",
                    Success = "#66BB6A",
                    Warning = "#FFEB3B",
                    Error = "#EF5350",
                    Info = "#29B6F6"
                }
            },
        };
    }
}