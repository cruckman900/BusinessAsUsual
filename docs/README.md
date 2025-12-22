🎸 Welcome to /docs — Your Backstage Pass to Business As Usual
This folder is your annotated tour guide through the architecture, branding, and onboarding strategy behind Business As Usual. Every markdown file here is tuned for clarity, speed, and expressive documentation—like a Metallica solo in text form.
- Purpose: Centralize onboarding, architecture notes, and legal strategy in one discoverable place.
- Tone: Loud, clear, and empowering. Docs should feel like a creative win, not a chore.
- Audience: Future collaborators, legal reviewers, and technical troubleshooters.

📁 File Index — What’s in the Setlist?
- README.md — This file. Your high-level tour guide to the folder’s purpose, tone, and layout.
- CHANGELOG.md — Timestamped changelog meets backstage diary. Every milestone logged with flair.
- LICENSE.md — Proprietary license tuned for expressive platforms. Covers frontend, backend, and branding.
- BRANDING.md — Visual identity breakdown: logo anatomy, favicon variants, OG image strategy, and dark mode riffs.
- ONBOARDING.md — Step-by-step setup for collaborators. From cloning to Docker to AWS deployment.
- ARCHITECTURE.md — Modular breakdown of the ASP.NET Core backend. Clean layers, Dockerized flow, and future-proofing.
- METADATA.md — Favicon, manifest, OG tags, Twitter cards. Everything that makes your site pop in previews.
- SPLASH.md — Animated splash intro logic. Scroll triggers, sound cues, and live-performance vibes.
- BADGES.md — Project badge anatomy. SVG variants, animation notes, and discoverability strategy.

🕶️ CHANGELOG.md — Timestamped Wins and Loud Milestones
This file is your backstage diary. Every technical fix, branding tweak, or onboarding breakthrough gets logged here like a setlist of victories. It’s not just a changelog—it’s a celebration.
- Format: Timestamped entries with bold headers, short riffs, and clear impact.
- Tone: Energetic, expressive, and onboarding-friendly. Every entry should help future collaborators understand what changed, why it matters, and how to replicate or extend it.
- Examples:
## [2025-10-03] 🎯 Renamed project, fixed Visual Studio path issues
- Updated solution and project names for clarity
- Fixed broken launch profile paths after rename
- Reorganized markdown docs into `/docs` for future-proof onboarding

- Why it matters: This file turns troubleshooting into storytelling. It’s your proof of progress, your onboarding compass, and your creative logbook.

## 🛠 Layout Redesign
We retired the legacy layout and rebuilt a fresh, modular shell with a top bar, dynamic sidebar, and recruiter‑ready content area. See [/Themes/README.md](./Themes/README.md) for details on the redesign process.

+-------------------------------------------------------------+
|                         Top Bar                             |
|  Tenant Selector | Search | Notifications | Profile Menu    |
+----------------------+--------------------------------------+
|      Sidebar         |           Main Content Area          |
|  Navigation Links    |  Recruiter-ready modules:            |
|  Responsive Theming  |  - HR                                |
|                      |  - Projects                          |
|                      |  - Resume                            |
|                      |  - Future modules                    |
+-------------------------------------------------------------+

## 🎨 Theme Pipeline & Contrast Audit

This project includes a theme registry with multiple recruiter‑ready palettes and a contrast audit page.

- **Theme Registry**: Each theme defines consistent text and contrast values. Special cases (Pearl, Silver, Obsidian) were tested and removed for usability.
- **Contrast Audit Page**: Navigate to `/contrast-audit` to see a live showcase of MudBlazor components under different themes.
- **Detailed Notes**: See [/Themes/README.md](./Themes/README.md) for the full pipeline explanation and design decisions.