using CRM.Application.DTOs;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace CRM.Application.Services;

public interface IModuleRegistrationService
{
    Task RegisterWithModuleRegistryAsync();
}

public class ModuleRegistrationService : IModuleRegistrationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ModuleRegistrationService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task RegisterWithModuleRegistryAsync()
    {
        var registryUrl = _configuration["ModuleRegistry:Url"] ?? "http://localhost:5100";
        var crmApiUrl = _configuration["CRM:ApiBaseUrl"] ?? "http://localhost:5004";
        var crmWebUrl = _configuration["CRM:UiEntryPoint"] ?? "http://localhost:5003";

        var request = new RegisterModuleRequest
        {
            ModuleId = "crm",
            Key = "crm", // Short key for routing
            DisplayName = "CRM",
            Description = "Manage leads, opportunities, and customers",
            Version = "1.0.0",
            ApiBaseUrl = crmApiUrl,
            UiEntryPoint = $"{crmWebUrl}/crm",
            Icon = Icons.ContactPhone,
            Permissions = new List<string> { "crm.read", "crm.write", "crm.admin" },
            Capabilities = new List<string> { "leads", "opportunities", "customers" },
            HealthUrl = $"{crmApiUrl}/health",
            TenantMode = "tenant-per-database",
            SupportsMobile = true,
            MobileUISpecUrl = $"{crmApiUrl}/api/crm/mobile/ui-spec",
            MobileContractVersion = "1.0.0",
            NavigationItems = new List<RegisterModuleRequest.NavigationItemDto>
            {
                new() { Label = "Dashboard", Route = "/crm", Icon = Icons.Dashboard },
                new() { Label = "Leads", Route = "/crm/leads", Icon = Icons.PersonSearch },
                new() { Label = "Opportunities", Route = "/crm/opportunities", Icon = Icons.TrendingUp },
                new() { Label = "Customers", Route = "/crm/customers", Icon = Icons.Business }
            }
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{registryUrl}/api/modules/register", request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine("✓ Successfully registered CRM module with Module Registry (including mobile support)");
        }
        catch (Exception ex)
        {
            // Log the error but don't fail startup
            Console.WriteLine($"Failed to register with Module Registry: {ex.Message}");
        }
    }

    // Material Icons SVG paths (MudBlazor compatible)
    private static class Icons
    {
        public const string ContactPhone = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M22 3H2C.9 3 0 3.9 0 5v14c0 1.1.9 2 2 2h20c1.1 0 1.99-.9 1.99-2L24 5c0-1.1-.9-2-2-2zM8 6c1.66 0 3 1.34 3 3s-1.34 3-3 3-3-1.34-3-3 1.34-3 3-3zm6 12H2v-1c0-2 4-3.1 6-3.1s6 1.1 6 3.1v1zm3.85-4h1.64L21 16l-1.99 1.99c-1.31-.98-2.28-2.38-2.73-3.99-.18-.64-.28-1.31-.28-2s.1-1.36.28-2c.45-1.62 1.42-3.01 2.73-3.99L21 8l-1.51 2h-1.64c-.22.63-.35 1.3-.35 2s.13 1.37.35 2z\"/>";
        public const string Dashboard = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M3 13h8V3H3v10zm0 8h8v-6H3v6zm10 0h8V11h-8v10zm0-18v6h8V3h-8z\"/>";
        public const string PersonSearch = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M10 8c0-2.21-1.79-4-4-4S2 5.79 2 8s1.79 4 4 4 4-1.79 4-4zm2.46 3.36c-.68.68-1.53 1.15-2.46 1.4v.24H4v-.24C1.61 12.15 0 10.28 0 8c0-2.76 2.24-5 5-5s5 2.24 5 5c0 .93-.25 1.78-.7 2.53l.16.13z\"/>";
        public const string TrendingUp = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M16 6l2.29 2.29-4.88 4.88-4-4L2 16.59 3.41 18l6-6 4 4 6.3-6.29L22 12V6z\"/>";
        public const string Business = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M12 7V3H2v18h20V7H12zM6 19H4v-2h2v2zm0-4H4v-2h2v2zm0-4H4V9h2v2zm0-4H4V5h2v2zm4 12H8v-2h2v2zm0-4H8v-2h2v2zm0-4H8V9h2v2zm0-4H8V5h2v2zm10 12h-8v-2h2v-2h-2v-2h2v-2h-2V9h8v10zm-2-8h-2v2h2v-2zm0 4h-2v2h2v-2z\"/>";
    }
}
