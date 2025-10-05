🎬 SPLASH.md — Animated Intro with Live-Performance Energy
This file documents the splash screen logic that kicks off Business As Usual like a stage entrance. It’s not just a loading screen—it’s a moment.

⚡ Trigger Logic
- Activation: Splash screen appears on first load or route change
- Dismissal: Auto-dismiss after animation completes or user scrolls
- Optional Delay: Configurable timeout for dramatic effect

🎞️ Animation Flow
- Elements:
- Geometric logo (arrow, stepped bars, semi-circle)
- Fade-in text with all-caps branding
- Scroll-triggered transition to main UI
- Tech: Framer Motion, CSS keyframes, or GSAP depending on frontend stack

🔊 Sound Cue
- Purpose: Add a subtle “Now Playing” vibe
- Trigger: On splash load or logo reveal
- File: /public/splash.mp3 or /sounds/intro.wav
- Volume: Low by default, user-controlled

🎛️ Customization
- Dark Mode Sync: Splash adapts to theme
- Performance: Lazy-load assets, optimize for mobile
- Accessibility: Skip option for screen readers or reduced motion

📓 Bonus Notes
- Log splash tweaks in CHANGELOG.md
- Sync with BRANDING.md for visual consistency
- Reference LICENSE.md for asset protection
