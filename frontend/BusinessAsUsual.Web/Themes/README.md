🛠 Layout Redesign
The original layout was retired in favor of a fresh, modular shell that better supports recruiter‑ready UX and future expansion. Instead of patching legacy markup, we started from scratch with a clean architecture:
- Three‑layer shell:
- Top Bar → tenant selector, global search, notifications, profile menu.
- Sidebar → dynamic navigation with expressive theming and responsive behavior.
- Main Content Area → recruiter‑ready modules (HR, Orders, Inventory, etc.) with consistent spacing and typography.
- Audit‑driven design: Every component (Tabs, Select, Buttons, Chips, Cards, Inputs, Lists) was tested in the new layout to guarantee contrast and usability.
- Contributor‑friendly pipeline: The layout is modular, responsive, and documented for onboarding clarity. Future collaborators can extend modules without fighting legacy cruft.
- Recruiter‑ready polish: The redesign emphasizes clarity, contrast safety, and expressive branding. It’s not just functional — it’s deliberately styled to showcase professional design decisions.

# Application Layout Diagram

| Layer        | Purpose                                                                 |
|--------------|-------------------------------------------------------------------------|
| **Top Bar**  | Tenant selector, global search, notifications, profile menu             |
| **Sidebar**  | Dynamic navigation, responsive behavior, expressive theming             |
| **Main Area**| Recruiter‑ready modules (HR, Orders, Inventory, etc.) with consistent UX |

Theme Pipeline & Contrast Audit
This project includes a theme registry and a contrast audit page to ensure every MudBlazor component is readable and recruiter‑ready. The pipeline is designed to make theme values easy to maintain while guaranteeing consistent UX across Appbar, Drawer, Tabs, Select, Buttons, Chips, Cards, Typography, Inputs, and Lists.
🎨 Theme Registry
- Each theme defines TextPrimary, TextSecondary, PrimaryContrastText, and SecondaryContrastText.
- Global CSS overrides enforce contrast safety for components that don’t expose a Color prop.
- Special cases (Pearl, Silver, Obsidian) were tested extensively to highlight edge cases in MudBlazor’s palette system.
🔍 Contrast Audit Page
- A dedicated page (/contrast-audit) displays one of each MudBlazor component.
- Used to visually confirm contrast across all themes.
- Serves as a recruiter‑facing showcase of systematic UX testing.
✂️ Design Decisions
- Pearl, Silver, Obsidian were removed from the final fleet.
- Pearl and Silver are too close to white, making text contrast fragile.
- Obsidian is ultra‑dark, causing highlight and contrast text to flip unpredictably.
- Keeping them would have required hacks that undermine the clean, systematic approach.
- Documenting their removal shows deliberate design choices and prioritization of usability.
✅ Recruiter‑Ready Takeaway
This repo demonstrates:
- A pipeline approach to theming, not one‑off fixes.
- Contrast‑safe defaults across all major MudBlazor components.
- Deliberate design decisions: testing, auditing, and trimming themes that don’t meet UX standards.
