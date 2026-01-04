🎨 Theme System & Contrast Audit
A unified design pipeline for consistent, accessible, expressive UI.
The Business As Usual theme system is built to ensure clarity, accessibility, and visual consistency across every MudBlazor component. This document explains how themes are structured, how contrast is enforced, and why certain palettes were removed during testing.
This is the authoritative reference for anyone extending, modifying, or auditing the UI design system.

🌈 Theme Registry
The theme registry defines the full fleet of supported palettes. Each theme includes:

- TextPrimary
- TextSecondary
- PrimaryContrastText
- SecondaryContrastText
- Additional palette values required for MudBlazor 8.x
  Themes are intentionally minimal and predictable. Instead of scattering color overrides across components, the registry centralizes all palette decisions in one place.
  Global CSS Overrides
  Some MudBlazor components do not expose a Color prop or do not fully respect palette values.
  To ensure consistent contrast across the entire UI, global CSS overrides are applied to:
- AppBar
- Drawer
- Tabs
- Select
- Buttons
- Chips
- Cards
- Inputs
- Lists
- Typography elements
  These overrides guarantee that text and surfaces remain readable across all themes, even in edge cases.

🔍 Contrast Audit Page
A dedicated audit page (/contrast-audit) renders one of each major MudBlazor component under the currently selected theme.
This page exists to:

- Validate contrast ratios visually
- Catch edge cases in component rendering
- Ensure consistency across light/dark variants
- Provide a quick sanity check when adding or modifying themes
  It is a practical tool for contributors and a safeguard against regressions.

✂️ Design Decisions
During testing, several experimental themes were removed from the final registry:
Pearl

- Too close to white
- Text contrast became fragile across multiple components
- Required hacks to maintain readability
  Silver
- Similar issues to Pearl
- Highlight and hover states lacked reliable contrast
  Obsidian
- Extremely dark
- Caused unpredictable contrast flips in MudBlazor’s palette system
- Required overrides that undermined the clean, systematic approach
  Removing these themes preserves the integrity of the design system and avoids long-term maintenance traps.

🧭 Why This Matters
The theme system is built as a pipeline, not a collection of one-off fixes.
It ensures:

- Consistent contrast across all components
- Predictable behavior when switching themes
- A scalable foundation for future palettes
- A clean separation between design tokens and UI components
- A contributor-friendly workflow for extending the design system
  This approach keeps the UI expressive without sacrificing accessibility or maintainability.

📁 Related Documentation
For layout, architecture, and UI shell details, see:

- /docs/ARCHITECTURE.md — Application layout, shell structure, and UI flow
- /docs/BRANDING.md — Logo, favicon, and identity guidelines
- /docs/METADATA.md — OG tags, manifest, and preview assets

🧭 Theme Registry Diagram
A high-level map of how theme values flow through the system.
+---------------------+
| Theme Registry |
| (Palette Tokens) |
+----------+----------+
|
v
+---------------------+
| MudBlazor Palette |
| (Light/Dark Modes) |
+----------+----------+
|
v
+---------------------+
| Global CSS Overrides|
| (Contrast Enforcement)
+----------+----------+
|
v
+---------------------+
| UI Components |
| (AppBar, Drawer, |
| Tabs, Buttons, etc)|
+---------------------+
|
v
+---------------------+
| Contrast Audit Page |
| (/contrast-audit) |
+---------------------+

This diagram shows the pipeline approach:
Tokens → Palette → Overrides → Components → Audit

🔄 Theme Lifecycle Flowchart
How a theme moves from idea → implementation → validation.
┌────────────────────┐
│ 1. Define Palette │
│ (colors, tokens) │
└─────────┬──────────┘
v
┌────────────────────┐
│ 2. Register Theme │
│ in ThemeRegistry │
└─────────┬──────────┘
v
┌────────────────────┐
│ 3. Apply Global │
│ CSS Overrides │
└─────────┬──────────┘
v
┌────────────────────┐
│ 4. Visual Testing │
│ on Audit Page │
└─────────┬──────────┘
v
┌────────────────────┐
│ 5. Accessibility │
│ Review │
└─────────┬──────────┘
v
┌────────────────────┐
│ 6. Approve or │
│ Remove Theme │
└────────────────────┘

This lifecycle ensures every theme is vetted before joining the fleet.

🧩 How to Add a New Theme
A contributor-friendly guide.

1. Create a Palette
   Add a new entry in your theme registry file:
   new MudTheme()
   {
   Palette = new PaletteLight()
   {
   Primary = "#...",
   Secondary = "#...",
   TextPrimary = "#...",
   TextSecondary = "#...",
   PrimaryContrastText = "#...",
   SecondaryContrastText = "#..."
   }
   }

2. Add Light/Dark Variants (if applicable)
   MudBlazor supports both; define both if your theme needs them.
3. Register the Theme
   Add it to your theme registry dictionary:
   Themes.Add("MyNewTheme", myTheme);

4. Test in the Contrast Audit Page
   Navigate to:
   /contrast-audit

Check:

- Buttons
- Tabs
- Inputs
- Chips
- Cards
- AppBar
- Drawer
- Typography

5. Validate Accessibility
   Confirm:

- TextPrimary vs background
- TextSecondary vs background
- Hover states
- Disabled states
- Focus rings

6. Approve or Remove
   If the theme requires hacks or breaks contrast rules, remove it.
   If it passes, add it to the official fleet.

🧱 CSS Override Reference Table
The single source of truth for global overrides.
+-----------+----------------------------------------------------+-----------------------------------------|
| Component | Why Override? | What’s Enforced? |
+-----------+----------------------------------------------------+-----------------------------------------|
| AppBar | MudBlazor sometimes ignores text contrast | TextPrimary, background, icon color |
| Drawer | Surface contrast varies by theme | Background, text, hover states |
| Tabs | Selected/unselected contrast inconsistent | Active color, inactive color, underline |
| Buttons | Some variants ignore palette contrast | Text color, background, hover |
| Chips | Light themes cause low contrast | Text color, border, background |
| Cards | Surface blending issues | Background, shadow, text |
| Inputs | Label and helper text contrast varies | Label color, border, focus ring |
| Lists | Hover and selected states inconsistent | Text color, background, hover |
+-----------+----------------------------------------------------+-----------------------------------------|
These overrides ensure predictable behavior across all themes.

✂️ Removed Themes (Design Rationale)
Pearl
Too close to white → fragile contrast.
Silver
Hover and highlight states lacked reliable contrast.
Obsidian
Ultra-dark → unpredictable contrast flips.
Removing them keeps the system clean, maintainable, and accessible.

🧭 Why This Matters
The theme system is built as a pipeline, not a patchwork.
It ensures:

- Consistent contrast
- Predictable component behavior
- Scalable palette design
- Contributor-friendly workflows
- A clean, expressive UI foundation

## 🎨 Top Bar Styling

- Custom styles for MudNavLink active state
- Animated underline for active modules
- Hover transitions for icons
- Responsive collapse for mobile
- Overflow menu for crowded modules
