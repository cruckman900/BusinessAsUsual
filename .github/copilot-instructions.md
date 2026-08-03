# Copilot Instructions

## Project Guidelines

- In Razor views (.cshtml), CSS @media queries must be escaped as @@media to prevent Razor from interpreting @ as code. For theme-aware table text, use [data-bs-theme="light"] and [data-bs-theme="dark"] selectors with @@media (prefers-color-scheme) fallbacks. For checkbox spacing in forms, use display: flex with gap: 0.5rem and margin-right: 0.5rem on the input.

- In the BusinessAsUsual Admin project, use CSS variables for theme-aware styling instead of hard-coding colors. The themes (blue, gold, retro, light, obsidian) define --content-text, --card-bg, --card-text, and --border variables. For table text, use var(--content-text) for body cells and var(--card-text) for headers. For muted text, apply opacity: 0.7 to the content text color. This ensures all themes work correctly without needing theme-specific selectors.

## API Guidelines

- For API-first with mock fallback pattern in Admin controllers, use CancellationTokenSource with a 2-second timeout on all HttpClient calls to speed up fallback to mock data. Catch exceptions using pattern matching for HttpRequestException, TaskCanceledException, and OperationCanceledException. This prevents long waits when the API is unavailable and improves page load performance.