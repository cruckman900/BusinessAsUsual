using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using BusinessAsUsual.Web.Modules._Shared;

namespace BusinessAsUsual.Web.Components.Layout
{
    public partial class MainLayout : IDisposable
    {
        // ------------------------------------------------------------
        // Injected Services
        // ------------------------------------------------------------
        [Inject] public ThemeContext ThemeContext { get; set; } = default!;
        [Inject] public NavigationManager Nav { get; set; } = default!;
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
        public void Dispose()
        {
            Nav.LocationChanged -= HandleLocationChanged;
            HeaderService.OnChange -= HandleHeaderChanged;
        }
    }
}