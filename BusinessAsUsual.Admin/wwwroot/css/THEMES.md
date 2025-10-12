# Theme Variables

This file defines CSS variables for layout zones and color schemes.

## Themes Available

- `default`: Branded dark header, light sidebar
- `dark`: Full dark mode with neon flair
- `light`: Clean white layout with soft contrast
- `retro`: LinearDescent legacy theme with glowing accents
- `contributor`: Gold-accented layout with Smart Commit flair


## Usage

Themes are applied by adding a class to `<body>`:
```html
<body class="theme-dark">

Variables are scoped using var(--variable-name) in layout CSS files.