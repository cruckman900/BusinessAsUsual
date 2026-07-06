using MudBlazor;

namespace BusinessAsUsual.Web.Services;

/// <summary>
/// Defines a service for discovering and retrieving information about application modules, including their UI entry points and active status. This service interacts with a module registry to fetch module metadata and provides caching to optimize performance. It also includes fallback logic to return hardcoded module definitions if the registry is unavailable.
/// </summary>
public interface IModuleDiscoveryService
{
    /// <summary>
    /// Asynchronously retrieves a list of modules that have a defined UI entry point. This method checks the cache for previously fetched module data and refreshes it if necessary. If the module registry is unavailable, it falls back to a predefined list of hardcoded modules. The returned modules are filtered to include only those with a non-empty UI entry point.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<ModuleDto>> GetModulesWithUiAsync();

    /// <summary>
    /// Asynchronously retrieves a list of active modules. This method checks the cache for previously fetched module data and refreshes it if necessary. If the module registry is unavailable, it falls back to a predefined list of hardcoded modules. The returned modules are filtered to include only those marked as active.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<ModuleDto>> GetActiveModulesAsync();
}

/// <summary>
/// Implements the <see cref="IModuleDiscoveryService"/> interface to provide functionality for discovering and retrieving module information. This service fetches module metadata from a configured module registry endpoint, caches the results for a specified duration, and provides methods to retrieve modules with UI entry points and active modules. If the module registry is unavailable, it falls back to a predefined list of hardcoded modules.
/// </summary>
public class ModuleDiscoveryService : IModuleDiscoveryService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ModuleDiscoveryService> _logger;
    private List<ModuleDto>? _cachedModules;
    private DateTime _lastCacheUpdate = DateTime.MinValue;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleDiscoveryService"/> class with the specified HTTP client, configuration, and logger. The service uses these dependencies to fetch module information from a module registry and log relevant events.
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    public ModuleDiscoveryService(HttpClient httpClient, IConfiguration configuration, ILogger<ModuleDiscoveryService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Asynchronously retrieves a list of modules that have a defined UI entry point. This method checks the cache for previously fetched module data and refreshes it if necessary. If the module registry is unavailable, it falls back to a predefined list of hardcoded modules. The returned modules are filtered to include only those with a non-empty UI entry point.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<ModuleDto>> GetModulesWithUiAsync()
    {
        await RefreshCacheIfNeeded();
        var modulesWithUi = _cachedModules?.Where(m => !string.IsNullOrEmpty(m.UiEntryPoint)) ?? Enumerable.Empty<ModuleDto>();
        _logger.LogInformation("[ModuleDiscovery] GetModulesWithUiAsync returning {Count} modules", modulesWithUi.Count());
        return modulesWithUi;
    }

    /// <summary>
    /// Asynchronously retrieves a list of active modules. This method checks the cache for previously fetched module data and refreshes it if necessary. If the module registry is unavailable, it falls back to a predefined list of hardcoded modules. The returned modules are filtered to include only those marked as active.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<ModuleDto>> GetActiveModulesAsync()
    {
        await RefreshCacheIfNeeded();
        return _cachedModules ?? Enumerable.Empty<ModuleDto>();
    }

    private Task RefreshCacheIfNeeded()
    {
        if (_cachedModules != null && DateTime.UtcNow - _lastCacheUpdate < _cacheExpiration)
        {
            return Task.CompletedTask;
        }

        // TEMPORARY: Force use of fallback data with correct routes
        // TODO: Fix Module Registry to have correct /modules/* routes
        _cachedModules = GetFallbackModules();
        _lastCacheUpdate = DateTime.UtcNow;
        _logger.LogInformation("Using fallback module data (registry temporarily disabled)");
        return Task.CompletedTask;

        /* Original registry lookup - disabled temporarily
        try
        {
            var registryUrl = _configuration["ModuleRegistry:Url"] ?? "http://localhost:5100";
            var response = await _httpClient.GetAsync($"{registryUrl}/api/modules/ui");

            if (response.IsSuccessStatusCode)
            {
                _cachedModules = await response.Content.ReadFromJsonAsync<List<ModuleDto>>() ?? new List<ModuleDto>();
                _lastCacheUpdate = DateTime.UtcNow;
                _logger.LogInformation("Successfully discovered {Count} modules from Module Registry", _cachedModules.Count);
            }
            else
            {
                _logger.LogWarning("Failed to fetch modules from Module Registry. Status: {StatusCode}", response.StatusCode);

                // Fallback to hardcoded modules if MRS is unavailable
                if (_cachedModules == null)
                {
                    _cachedModules = GetFallbackModules();
                    _logger.LogInformation("Using fallback module list");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error discovering modules from Module Registry");

            // Fallback to hardcoded modules if MRS is unavailable
            if (_cachedModules == null)
            {
                _cachedModules = GetFallbackModules();
                _logger.LogInformation("Using fallback module list due to error");
            }
        }
        */
    }

    private List<ModuleDto> GetFallbackModules()
    {
        return new List<ModuleDto>
        {
            new ModuleDto
            {
                ModuleId = "hr",
                Key = "hr",
                DisplayName = "HR",
                Description = "Manage employees, onboarding, and benefits",
                UiEntryPoint = "/hr",
                Icon = Icons.Material.Filled.People,
                IsActive = true,
                NavigationItems = new List<NavigationItemDto>
                {
                    new() { Label = "Home", Route = "/hr", Icon = Icons.Material.Filled.Home },

                    // Employee Management Group
                    new() 
                    { 
                        Label = "Employee Management", 
                        Route = "/hr/employees", 
                        Icon = Icons.Material.Filled.People,
                        ExpandedByDefault = true,
                        Children = new List<NavigationItemDto>
                        {
                            new() { Label = "All Employees", Route = "/hr/employees", Icon = Icons.Material.Filled.List },
                            new() { Label = "Add Employee", Route = "/hr/employees/new", Icon = Icons.Material.Filled.PersonAdd }
                        }
                    },

                    // Departments Group
                    new() 
                    { 
                        Label = "Departments", 
                        Route = "/hr/departments", 
                        Icon = Icons.Material.Filled.Business,
                        ExpandedByDefault = true,
                        Children = new List<NavigationItemDto>
                        {
                            new() { Label = "All Departments", Route = "/hr/departments", Icon = Icons.Material.Filled.List },
                            new() { Label = "Add Department", Route = "/hr/departments/new", Icon = Icons.Material.Filled.Add }
                        }
                    },

                    // Recruiting Group
                    new() 
                    { 
                        Label = "Recruiting", 
                        Route = "/hr/applicants", 
                        Icon = Icons.Material.Filled.PersonSearch,
                        ExpandedByDefault = false,
                        Children = new List<NavigationItemDto>
                        {
                            new() { Label = "Applicants", Route = "/hr/applicants", Icon = Icons.Material.Filled.PersonSearch },
                            new() { Label = "Interviews", Route = "/hr/interviews", Icon = Icons.Material.Filled.Event }
                        }
                    },

                    // Performance Group
                    new() 
                    { 
                        Label = "Performance", 
                        Route = "/hr/reviews", 
                        Icon = Icons.Material.Filled.Assessment,
                        ExpandedByDefault = false,
                        Children = new List<NavigationItemDto>
                        {
                            new() { Label = "Reviews", Route = "/hr/reviews", Icon = Icons.Material.Filled.Assessment },
                            new() { Label = "Goals", Route = "/hr/goals", Icon = Icons.Material.Filled.Flag }
                        }
                    },

                    // Training Group
                    new() 
                    { 
                        Label = "Training", 
                        Route = "/hr/courses", 
                        Icon = Icons.Material.Filled.School,
                        ExpandedByDefault = false,
                        Children = new List<NavigationItemDto>
                        {
                            new() { Label = "Courses", Route = "/hr/courses", Icon = Icons.Material.Filled.School },
                            new() { Label = "Certifications", Route = "/hr/certifications", Icon = Icons.Material.Filled.Verified }
                        }
                    },

                    // Timekeeping Group
                    new() 
                    { 
                        Label = "Timekeeping", 
                        Route = "/hr/timesheets", 
                        Icon = Icons.Material.Filled.AccessTime,
                        ExpandedByDefault = false,
                        Children = new List<NavigationItemDto>
                        {
                            new() { Label = "Timesheets", Route = "/hr/timesheets", Icon = Icons.Material.Filled.AccessTime },
                            new() { Label = "Approvals", Route = "/hr/approvals", Icon = Icons.Material.Filled.Approval }
                        }
                    },

                    // HR Admin Group
                    new() 
                    { 
                        Label = "HR Administration", 
                        Route = "/hr/onboarding", 
                        Icon = Icons.Material.Filled.AdminPanelSettings,
                        ExpandedByDefault = false,
                        Children = new List<NavigationItemDto>
                        {
                            new() { Label = "Onboarding", Route = "/hr/onboarding", Icon = Icons.Material.Filled.PersonAdd },
                            new() { Label = "Benefits", Route = "/hr/benefits", Icon = Icons.Material.Filled.CardGiftcard }
                        }
                    },

                    // Reports
                    new() { Label = "Reports", Route = "/hr/reports", Icon = Icons.Material.Filled.BarChart },

                    // Settings
                    new() { Label = "Settings", Route = "/hr/settings", Icon = Icons.Material.Filled.Settings }
                }
            },
            new ModuleDto
            {
                ModuleId = "crm",
                Key = "crm",
                DisplayName = "CRM",
                Description = "Customer Relationship Management",
                UiEntryPoint = "/crm",
                Icon = Icons.Material.Filled.ContactPhone,
                IsActive = true,
                NavigationItems = new List<NavigationItemDto>
                {
                    new() { Label = "Home", Route = "/crm", Icon = Icons.Material.Filled.Dashboard },

                    // Leads Group
                    new() 
                    { 
                        Label = "Leads", 
                        Route = "/crm/leads", 
                        Icon = Icons.Material.Filled.PersonSearch,
                        ExpandedByDefault = true,
                        Children = new List<NavigationItemDto>
                        {
                            new() { Label = "All Leads", Route = "/crm/leads", Icon = Icons.Material.Filled.List },
                            new() { Label = "Add Lead", Route = "/crm/leads/new", Icon = Icons.Material.Filled.PersonAdd }
                        }
                    },

                    // Opportunities Group
                    new() 
                    { 
                        Label = "Opportunities", 
                        Route = "/crm/opportunities", 
                        Icon = Icons.Material.Filled.TrendingUp,
                        ExpandedByDefault = true,
                        Children = new List<NavigationItemDto>
                        {
                            new() { Label = "All Opportunities", Route = "/crm/opportunities", Icon = Icons.Material.Filled.List },
                            new() { Label = "Add Opportunity", Route = "/crm/opportunities/new", Icon = Icons.Material.Filled.Add }
                        }
                    },

                    // Customers Group
                    new() 
                    { 
                        Label = "Customers", 
                        Route = "/crm/customers", 
                        Icon = Icons.Material.Filled.Business,
                        ExpandedByDefault = true,
                        Children = new List<NavigationItemDto>
                        {
                            new() { Label = "All Customers", Route = "/crm/customers", Icon = Icons.Material.Filled.List },
                            new() { Label = "Add Customer", Route = "/crm/customers/new", Icon = Icons.Material.Filled.Add }
                        }
                    },

                    // Activities
                    new() { Label = "Activities", Route = "/crm/activities", Icon = Icons.Material.Filled.CalendarToday },

                    // Reports
                    new() { Label = "Reports", Route = "/crm/reports", Icon = Icons.Material.Filled.Analytics },

                    // Settings
                    new() { Label = "Settings", Route = "/crm/settings", Icon = Icons.Material.Filled.Settings }
                }
            }
        };
    }
}
