🧬 METADATA.md — Preview Power and Icon Precision
This file documents everything that shapes how Business As Usual appears across platforms—from browser tabs to social shares. It’s the invisible layer that makes your site feel polished and intentional.
🖼️ OG Tags & Social Previews
- Purpose: Control how your site appears when shared on Twitter, LinkedIn, Discord, etc.
- Tags: og:image, og:title, og:description, twitter:card, twitter:image
- Placement: Inside <head> via _document.js or next-seo.config.js
- Image: Use /public/og.png with geometric branding, no human figures

🌐 Manifest & Icons
- File: manifest.json in /public
- Contents:
- name, short_name, start_url, display, background_color, theme_color
- Icon array with multiple sizes (e.g., 48x48, 96x96, 192x192, 512x512)
- Purpose: Enables PWA support, mobile install prompts, and consistent branding

🧭 Favicon Wiring
- Tags:
<link rel="icon" href="/favicon.ico" />
<link rel="apple-touch-icon" sizes="180x180" href="/apple-touch-icon.png" />
<link rel="manifest" href="/manifest.json" />

- Design: Icon-only geometric mark, optimized for clarity at small sizes
📓 Bonus Notes
- Preview your OG tags with Twitter Card Validator or Meta Tag Preview
- Log metadata updates in CHANGELOG.md
- Sync with BRANDING.md for visual consistency
