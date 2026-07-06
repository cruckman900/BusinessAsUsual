using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace BusinessAsUsual.Web.Services;

/// <summary>
/// Master navigation orchestrator that intercepts ALL routes and manages module/iframe navigation
/// This service ensures the browser URL is the single source of truth
/// </summary>
public class ModuleRouteInterceptor : IDisposable
{
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _js;
    private readonly IModuleDiscoveryService _moduleDiscovery;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleRouteInterceptor"/> class.
    /// </summary>
    /// <param name="navigationManager">The navigation manager.</param>
    /// <param name="js">The JavaScript runtime.</param>
    /// <param name="moduleDiscovery">The module discovery service.</param>
    public ModuleRouteInterceptor(
        NavigationManager navigationManager,
        IJSRuntime js,
        IModuleDiscoveryService moduleDiscovery)
    {
        _navigationManager = navigationManager;
        _js = js;
        _moduleDiscovery = moduleDiscovery;
    }

    /// <summary>
    /// Parses a URL and determines if it's a module route
    /// Returns (moduleKey, internalRoute) or (null, null) if not a module route
    /// </summary>
    public Task<(string? moduleKey, string? internalRoute)> ParseModuleRoute(string url)
    {
        var uri = new Uri(url);
        var path = uri.AbsolutePath;

        Console.WriteLine($"[RouteInterceptor] Parsing route: {path}");

        // Check if this is a module route: /modules/{moduleKey}/{...internalRoute}
        if (path.StartsWith("/modules/", StringComparison.OrdinalIgnoreCase))
        {
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length >= 2 && segments[0].Equals("modules", StringComparison.OrdinalIgnoreCase))
            {
                var moduleKey = segments[1];

                // Get the internal route (everything after /modules/{moduleKey})
                var modulePrefix = $"/modules/{moduleKey}";
                var internalRoute = path.Length > modulePrefix.Length 
                    ? path.Substring(modulePrefix.Length) 
                    : "/";

                // Append query string if present
                if (!string.IsNullOrEmpty(uri.Query))
                {
                    internalRoute += uri.Query;
                }

                Console.WriteLine($"[RouteInterceptor] Detected module route - Key: {moduleKey}, Internal: {internalRoute}");
                return Task.FromResult<(string? moduleKey, string? internalRoute)>((moduleKey, internalRoute));
            }
        }

        return Task.FromResult<(string? moduleKey, string? internalRoute)>((null, null));
    }

    /// <summary>
    /// Builds a full parent shell URL from module key and internal route
    /// Example: BuildModuleUrl("hr", "/employees?id=1") → "/modules/hr/employees?id=1"
    /// </summary>
    public string BuildModuleUrl(string moduleKey, string internalRoute)
    {
        internalRoute = internalRoute.TrimStart('/');
        return $"/modules/{moduleKey}/{internalRoute}";
    }

    /// <summary>
    /// Navigates to a module route, updating the browser URL
    /// The ModuleHost component will detect the route change and update the iframe
    /// </summary>
    public void NavigateToModule(string moduleKey, string internalRoute)
    {
        var url = BuildModuleUrl(moduleKey, internalRoute);
        Console.WriteLine($"[RouteInterceptor] Navigating parent shell to: {url}");
        _navigationManager.NavigateTo(url);
    }

    /// <summary>
    /// Disposes resources used by the route interceptor.
    /// </summary>
    public void Dispose()
    {
        // Cleanup if needed
    }
}
