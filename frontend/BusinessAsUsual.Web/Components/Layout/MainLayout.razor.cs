using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using BusinessAsUsual.Web.Modules._Shared;
using BusinessAsUsual.Web.Services;

namespace BusinessAsUsual.Web.Components.Layout
{
    /// <summary>
    /// Provides the main layout structure for the application, including navigation, sidebar management, and tenant
    /// selection. Integrates module navigation and header updates within the layout.
    /// </summary>
    /// <remarks>This layout component coordinates navigation between application modules and manages the
    /// sidebar's open or closed state. It responds to location and header changes to update the displayed content and
    /// header information. The layout also supports switching between available tenants. Services such as theme
    /// context, navigation manager, and page header service are injected to facilitate UI theming, navigation, and
    /// header updates. The component implements <see cref="IDisposable"/> to detach event handlers when disposed,
    /// ensuring proper resource cleanup.</remarks>
    public partial class MainLayout : IDisposable
    {
        // ------------------------------------------------------------
        // Injected Services
        // ------------------------------------------------------------

        /// <summary>
        /// Gets or sets the current theme context for the component.
        /// </summary>
        /// <remarks>This property is typically injected to provide access to theme-related information,
        /// such as colors, styles, or user preferences, within the component. The value is supplied by the dependency
        /// injection system and should not be set manually in most scenarios.</remarks>
        [Inject] public ThemeContext ThemeContext { get; set; } = default!;

        /// <summary>
        /// Gets or sets the navigation manager used to programmatically navigate within the application.
        /// </summary>
        /// <remarks>This property is typically injected by the framework in Blazor components to enable
        /// navigation and URL manipulation. Assigning a value manually is not recommended outside of testing
        /// scenarios.</remarks>
        [Inject] public NavigationManager Nav { get; set; } = default!;

        /// <summary>
        /// Gets or sets the service used to manage the page header state and content.
        /// </summary>
        /// <remarks>This property is typically injected by the framework to provide access to page header
        /// functionality within the component. Do not set this property manually unless performing advanced
        /// customization or testing.</remarks>
        [Inject] public PageHeaderService HeaderService { get; set; } = default!;

        /// <summary>
        /// Gets or sets the module discovery service used to discover modules from Module Registry Service.
        /// </summary>
        [Inject] public IModuleDiscoveryService ModuleDiscoveryService { get; set; } = default!;

        // ------------------------------------------------------------
        // State
        // ------------------------------------------------------------

        private bool _sidebarOpen = true;

        private string? _currentModule = null;
        private ModuleDefinition? ActiveModule =>
            Modules.FirstOrDefault(m => m.Name == _currentModule);

        private string CurrentTenant = "Business A";
        private List<string> AvailableTenants = new() { "Business A", "Business B" };

        private DrawerVariant SidebarVariant => DrawerVariant.Responsive;

        // ------------------------------------------------------------
        // Module Navigation
        // ------------------------------------------------------------

        private List<Modules._Shared.ModuleDefinition> Modules = new();

        private List<Modules._Shared.ModuleDefinition> OverflowModules = new()
        {
            new() { Name = "Contrast Audit", Description = "Validate color palette", Route = "/contrast-audit", Icon = Icons.Material.Filled.Abc },
        };

        // ------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------

        /// <summary>
        /// Initializes the component and subscribes to navigation and header change events.
        /// </summary>
        /// <remarks>This method is called during the component's initialization phase. It sets up event
        /// handlers to respond to navigation changes and header updates, ensuring the component remains synchronized
        /// with application state. Override this method to perform additional setup when the component is first
        /// rendered.</remarks>
        protected override async Task OnInitializedAsync()
        {
            Nav.LocationChanged += HandleLocationChanged;
            HeaderService.OnChange += HandleHeaderChanged;

            // Load modules from Module Registry Service FIRST
            await LoadModulesAsync();

            // Then update module from current URI (after modules are loaded)
            UpdateModuleFromUri(Nav.Uri);
        }

        private async Task LoadModulesAsync()
        {
            try
            {
                var discoveredModules = await ModuleDiscoveryService.GetModulesWithUiAsync();

                Console.WriteLine($"[MainLayout] Discovered {discoveredModules.Count()} modules");

                Modules = discoveredModules.Select(m => new Modules._Shared.ModuleDefinition
                {
                    Key = m.Key,
                    Name = m.DisplayName,
                    Description = m.Description,
                    // Use /modules/{key} route to embed the module UI via iframe
                    Route = $"/modules/{m.Key}",
                    Icon = m.Icon ?? Icons.Material.Filled.Apps,
                    NavigationItems = m.NavigationItems.Select(nav => ConvertToModuleNavigationItem(nav)).ToList()
                }).ToList();

                foreach (var module in Modules)
                {
                    Console.WriteLine($"[MainLayout] Module: {module.Name} -> {module.Route} ({module.NavigationItems.Count} nav items)");
                }

                // Re-update module detection now that modules are loaded
                UpdateModuleFromUri(Nav.Uri);

                StateHasChanged();
            }
            catch (Exception ex)
            {
                // Log error but don't crash the app
                Console.WriteLine($"Error loading modules: {ex.Message}");
            }
        }

        private void HandleHeaderChanged()
        {
            InvokeAsync(StateHasChanged);
        }

        // ------------------------------------------------------------
        // Navigation + Sidebar
        // ------------------------------------------------------------

        private void HandleLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            UpdateModuleFromUri(e.Location);
            StateHasChanged();
        }

        private void UpdateModuleFromUri(string uri)
        {
            var path = new Uri(uri).AbsolutePath.ToLower();

            // Check for embedded module routes first
            if (path.StartsWith("/modules/"))
            {
                var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    var moduleKey = parts[1]; // e.g., "hr" from "/modules/hr/employees"

                    // If modules are loaded, find by route
                    if (Modules.Any())
                    {
                        var module = Modules.FirstOrDefault(m => m.Route.Contains(moduleKey, StringComparison.OrdinalIgnoreCase));
                        _currentModule = module?.Name;
                    }
                    else
                    {
                        // Fallback: Use key directly (modules not loaded yet)
                        // This will be updated when LoadModulesAsync completes
                        _currentModule = moduleKey.ToUpper();
                    }
                    return;
                }
            }

            // Legacy hardcoded routes
            if (path.StartsWith("/hr"))
                _currentModule = "HR";
            else if (path.StartsWith("/finance"))
                _currentModule = "Finance";
            else if (path.StartsWith("/crm"))
                _currentModule = "CRM";
            else if (path.StartsWith("/timekeeping"))
                _currentModule = "Timekeeping";
            else if (path.StartsWith("/contractaudit"))
                _currentModule = "Contract Audit";
            else if (path.StartsWith("/admin"))
                _currentModule = "Admin";
            else
                _currentModule = null;
        }

        private void HandleTenantChanged(string tenant)
        {
            CurrentTenant = tenant;
        }

        private void SelectModule(string module)
        {
            _currentModule = module;
            _sidebarOpen = true;
        }

        private void ToggleSidebar()
        {
            _sidebarOpen = !_sidebarOpen;
        }

        private void HandleNavigate(string route)
        {
            Nav.NavigateTo(route);
            _sidebarOpen = false;
            StateHasChanged();
        }

        // ------------------------------------------------------------
        // Helper Methods
        // ------------------------------------------------------------

        private ModuleNavigationItem ConvertToModuleNavigationItem(NavigationItemDto dto)
        {
            var item = new ModuleNavigationItem
            {
                Label = dto.Label,
                Route = dto.Route,
                Icon = dto.Icon,
                ExpandedByDefault = dto.ExpandedByDefault
            };

            if (dto.Children?.Any() == true)
            {
                item.Children = dto.Children.Select(ConvertToModuleNavigationItem).ToList();
            }

            return item;
        }

        // ------------------------------------------------------------
        // Cleanup
        // ------------------------------------------------------------

        /// <summary>
        /// Releases resources used by the instance and unsubscribes from event handlers to prevent memory leaks.
        /// </summary>
        /// <remarks>Call this method when the instance is no longer needed to ensure that event
        /// subscriptions are properly removed. After calling <see cref="Dispose"/>, the instance should not be
        /// used.</remarks>
        public void Dispose()
        {
            Nav.LocationChanged -= HandleLocationChanged;
            HeaderService.OnChange -= HandleHeaderChanged;
        }
    }
}