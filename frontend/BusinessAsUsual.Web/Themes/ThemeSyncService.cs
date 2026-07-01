using Microsoft.JSInterop;

namespace BusinessAsUsual.Web.Themes;

/// <summary>
/// Service to synchronize theme changes to iframe-embedded modules
/// </summary>
public class ThemeSyncService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ThemeContext _themeContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeSyncService"/> class with the specified JavaScript runtime and theme context. This service listens for theme changes in the application and broadcasts them to any iframe-embedded modules using JavaScript interop.
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="themeContext"></param>
    public ThemeSyncService(IJSRuntime jsRuntime, ThemeContext themeContext)
    {
        _jsRuntime = jsRuntime;
        _themeContext = themeContext;
        _themeContext.OnThemeChanged += BroadcastThemeChange;
    }

    /// <summary>
    /// Initializes the theme synchronization service. This method is called to ensure that any necessary setup for broadcasting theme changes is completed. In this implementation, no additional initialization is required since the JavaScript file responsible for theme synchronization is loaded as a regular script tag.
    /// </summary>
    /// <returns></returns>
    public Task InitializeAsync()
    {
        // theme-sync.js is loaded as a regular script tag, no initialization needed
        return Task.CompletedTask;
    }

    private async void BroadcastThemeChange()
    {
        try
        {
            // Call global window.ThemeSync.broadcastTheme()
            await _jsRuntime.InvokeVoidAsync("ThemeSync.broadcastTheme", 
                _themeContext.ThemeName, 
                _themeContext.IsDarkMode);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ThemeSyncService] Failed to broadcast theme: {ex.Message}");
        }
    }

    /// <summary>
    /// Disposes of the theme synchronization service asynchronously. This method unsubscribes from the theme change event to prevent memory leaks and ensure that the service is properly cleaned up when it is no longer needed.
    /// </summary>
    /// <returns></returns>
    public ValueTask DisposeAsync()
    {
        _themeContext.OnThemeChanged -= BroadcastThemeChange;
        return ValueTask.CompletedTask;
    }
}
