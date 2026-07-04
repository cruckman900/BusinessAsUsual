# Theme Synchronization for Iframe Modules

## Problem
The HR.Web module runs in an iframe, which creates a completely isolated DOM and Blazor context. The parent shell's `ThemeContext` and MudBlazor theme cannot reach into the iframe, so HR pages were stuck with the default MudBlazor theme.

## Solution
Implemented a bidirectional `postMessage`-based theme synchronization system.

## Architecture

### Parent Shell (BusinessAsUsual.Web)

**Files Created/Modified:**

1. **`wwwroot/js/theme-sync.js`** (NEW)
   - JavaScript module that broadcasts theme changes to all iframes
   - Listens for `request-theme` messages from iframes
   - Responds with current theme when requested

2. **`Themes/ThemeSyncService.cs`** (NEW)
   - Blazor service that bridges C# ThemeContext to JavaScript
   - Subscribes to `ThemeContext.OnThemeChanged` events
   - Invokes `ThemeSync.broadcastTheme()` when theme changes

3. **`Themes/ThemeProviderWrapper.razor`** (MODIFIED)
   - Now injects and initializes `ThemeSyncService`
   - Broadcasts initial theme on first render
   - Implements `IAsyncDisposable` for cleanup

4. **`Program.cs`** (MODIFIED)
   - Registers `ThemeSyncService` as scoped service

5. **`Pages/_Host.cshtml`** (MODIFIED)
   - Added `<script src="/js/theme-sync.js"></script>`

### HR Module (HR.Web)

**Files Created/Modified:**

1. **`Themes/IframeThemeContext.cs`** (NEW)
   - Isolated theme context for the iframe
   - References `BusinessAsUsual.Web.Themes.ThemeRegistry` for theme definitions
   - Provides `SetTheme()` and `OnThemeChanged` event

2. **`Components/Layout/IframeThemeReceiver.razor`** (NEW)
   - Blazor component that receives theme messages
   - Initializes JavaScript listener on first render
   - Exposes `[JSInvokable]` method for JavaScript callbacks
   - Updates `IframeThemeContext` when messages arrive

3. **`wwwroot/js/iframe-theme-receiver.js`** (NEW)
   - JavaScript module for iframe side
   - Listens for `message` events from parent window
   - Filters for `type: 'theme-change'` messages
   - Invokes .NET method to update theme
   - Sends `request-theme` on initialization

4. **`Components/Layout/IframeLayout.razor`** (MODIFIED)
   - Now uses `IframeThemeContext` instead of generic `MudThemeProvider`
   - Includes `<IframeThemeReceiver />` component
   - Subscribes to theme changes and triggers re-render

5. **`Program.cs`** (MODIFIED)
   - Registers `IframeThemeContext` as scoped service

6. **`HR.Web.csproj`** (MODIFIED)
   - Added project reference to `BusinessAsUsual.Web` for shared theme registry
   - Excluded bootstrap from static web assets to prevent publishing conflicts
   - Set `StaticWebAssetBasePath` to scope assets under `_content/HR.Web`

## Message Flow

### Initial Load
```
1. HR.Web iframe loads
2. IframeThemeReceiver.razor initializes
3. iframe-theme-receiver.js sends { type: 'request-theme' } to parent
4. Parent theme-sync.js receives message
5. Parent responds with { type: 'theme-change', themeName, isDarkMode }
6. Iframe receives and applies theme
```

### Theme Change
```
1. User changes theme in parent shell
2. ThemeContext.SetTheme() called
3. ThemeContext.OnThemeChanged fires
4. ThemeSyncService.BroadcastThemeChange() invokes JS
5. theme-sync.js broadcasts to all iframes
6. Each iframe receives message
7. IframeThemeReceiver updates IframeThemeContext
8. MudThemeProvider re-renders with new theme
```

## Message Types

### `theme-change`
**Direction:** Parent → Iframe  
**Payload:**
```javascript
{
  type: 'theme-change',
  themeName: 'light' | 'dark' | 'hazard' | ...,
  isDarkMode: boolean
}
```

### `request-theme`
**Direction:** Iframe → Parent  
**Payload:**
```javascript
{
  type: 'request-theme'
}
```

## Benefits

✅ **Real-time sync** - Theme changes propagate instantly to all iframe modules  
✅ **No polling** - Event-driven via `postMessage`  
✅ **Shared theme registry** - HR.Web uses the same theme definitions as parent  
✅ **Automatic initialization** - Iframes request theme on load  
✅ **Multiple iframe support** - Broadcasts to all iframes simultaneously  
✅ **Decoupled** - Modules don't need to know parent implementation details

## Future Enhancements

- [ ] Add theme preview/demo mode
- [ ] Support custom theme overrides per module
- [ ] Persist iframe theme preference in localStorage
- [ ] Add theme transition animations
- [ ] Extend to other module types (non-iframe)

## Testing

To verify:
1. Run both `BusinessAsUsual.Web` and `HR.Web` services
2. Navigate to HR module (which loads in iframe)
3. Open browser console - look for theme sync log messages
4. Change theme in parent shell (ThemeChooser component)
5. Observe HR.Web pages update immediately

Expected console output:
```
[ThemeSync] Loaded in parent shell
[ThemeSync] Listening for theme requests from iframes
[IframeThemeReceiver] Module loaded
[IframeThemeReceiver] Initializing...
[IframeThemeReceiver] Requesting initial theme from parent
[ThemeSync] Iframe requesting current theme
[IframeThemeReceiver] Received theme: light, dark: false
[IframeThemeContext] Theme updated: light, dark: false
```

## Related Files
- `frontend/BusinessAsUsual.Web/Themes/ThemeRegistry.cs` - Theme definitions
- `frontend/BusinessAsUsual.Web/Components/ThemeChooser.razor` - Theme switcher UI
- `docs/HANDOVER_DOCUMENT.md` - Architecture documentation
