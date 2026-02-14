### `wwwroot/js/README.md`

# ThemeSwitcher.js

This script enables dynamic theme switching via a dropdown.

## How It Works

- Listens for changes to `#theme-toggle`
- Clears existing theme class from `<body>`
- Applies selected theme class (e.g., `theme-dark`, `theme-retro`)

## Extending

To add new themes:
1. Define variables in `theme.css`
2. Add option to the dropdown
3. Add matching `.theme-{name}` class in CSS