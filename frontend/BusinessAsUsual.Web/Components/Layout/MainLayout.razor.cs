using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using BusinessAsUsual.Web.Modules._Shared;

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

        private List<Modules._Shared.ModuleDefinition> Modules = new()
        {
            new() { Name = "HR", Description = "Manage employees, onboarding, and benefits", Route = "/hr", Icon = Icons.Material.Filled.People },
            new() { Name = "Finance", Description = "Invoices, payroll, and financial reporting", Route = "/finance", Icon = Icons.Material.Filled.AttachMoney },
            new() { Name = "CRM", Description="Customer relationships and communication", Route = "/crm", Icon = Icons.Material.Filled.BusinessCenter }
        };

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
        protected override void OnInitialized()
        {
            Nav.LocationChanged += HandleLocationChanged;
            HeaderService.OnChange += HandleHeaderChanged;

            UpdateModuleFromUri(Nav.Uri);
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