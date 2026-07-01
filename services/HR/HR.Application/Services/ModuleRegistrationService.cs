using HR.Application.DTOs;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace HR.Application.Services;

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
        var hrApiUrl = _configuration["HR:ApiBaseUrl"] ?? "http://localhost:5041";
        var hrWebUrl = _configuration["HR:UiEntryPoint"] ?? "http://localhost:5002";

        var request = new RegisterModuleRequest
        {
            ModuleId = "hr",
            Key = "hr", // Short key for routing
            DisplayName = "HR",
            Description = "Manage employees, departments, onboarding, and benefits",
            Version = "1.0.0",
            ApiBaseUrl = hrApiUrl,
            UiEntryPoint = $"{hrWebUrl}/hr",
            Icon = Icons.Group,
            Permissions = new List<string> { "hr.read", "hr.write", "hr.admin" },
            Capabilities = new List<string> { "employees", "departments", "onboarding", "benefits" },
            HealthUrl = $"{hrApiUrl}/health",
            TenantMode = "tenant-per-database",
            SupportsMobile = true,
            MobileUISpecUrl = $"{hrApiUrl}/api/hr/mobile/ui-spec",
            MobileContractVersion = "1.0.0",
            NavigationItems = new List<RegisterModuleRequest.NavigationItemDto>
            {
                new() { Label = "Dashboard", Route = "/hr", Icon = Icons.Dashboard },
                new() { Label = "Employees", Route = "/hr/employees", Icon = Icons.People },
                new() { Label = "Departments", Route = "/hr/departments", Icon = Icons.Business },
                new() { Label = "Onboarding", Route = "/hr/onboarding", Icon = Icons.PersonAdd },
                new() { Label = "Benefits", Route = "/hr/benefits", Icon = Icons.CardGiftcard }
            }
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{registryUrl}/api/modules/register", request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine("✓ Successfully registered HR module with Module Registry (including mobile support)");
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
        public const string Group = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M16 11c1.66 0 2.99-1.34 2.99-3S17.66 5 16 5c-1.66 0-3 1.34-3 3s1.34 3 3 3zm-8 0c1.66 0 2.99-1.34 2.99-3S9.66 5 8 5C6.34 5 5 6.34 5 8s1.34 3 3 3zm0 2c-2.33 0-7 1.17-7 3.5V19h14v-2.5c0-2.33-4.67-3.5-7-3.5zm8 0c-.29 0-.62.02-.97.05 1.16.84 1.97 1.97 1.97 3.45V19h6v-2.5c0-2.33-4.67-3.5-7-3.5z\"/>";
        public const string Dashboard = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M3 13h8V3H3v10zm0 8h8v-6H3v6zm10 0h8V11h-8v10zm0-18v6h8V3h-8z\"/>";
        public const string People = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M16 11c1.66 0 2.99-1.34 2.99-3S17.66 5 16 5c-1.66 0-3 1.34-3 3s1.34 3 3 3zm-8 0c1.66 0 2.99-1.34 2.99-3S9.66 5 8 5C6.34 5 5 6.34 5 8s1.34 3 3 3zm0 2c-2.33 0-7 1.17-7 3.5V19h14v-2.5c0-2.33-4.67-3.5-7-3.5zm8 0c-.29 0-.62.02-.97.05 1.16.84 1.97 1.97 1.97 3.45V19h6v-2.5c0-2.33-4.67-3.5-7-3.5z\"/>";
        public const string Business = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M12 7V3H2v18h20V7H12zM6 19H4v-2h2v2zm0-4H4v-2h2v2zm0-4H4V9h2v2zm0-4H4V5h2v2zm4 12H8v-2h2v2zm0-4H8v-2h2v2zm0-4H8V9h2v2zm0-4H8V5h2v2zm10 12h-8v-2h2v-2h-2v-2h2v-2h-2V9h8v10zm-2-8h-2v2h2v-2zm0 4h-2v2h2v-2z\"/>";
        public const string PersonAdd = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M15 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm-9-2V7H4v3H1v2h3v3h2v-3h3v-2H6zm9 4c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z\"/>";
        public const string CardGiftcard = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M20 6h-2.18c.11-.31.18-.65.18-1 0-1.66-1.34-3-3-3-1.05 0-1.96.54-2.5 1.35l-.5.67-.5-.68C10.96 2.54 10.05 2 9 2 7.34 2 6 3.34 6 5c0 .35.07.69.18 1H4c-1.11 0-1.99.89-1.99 2L2 19c0 1.11.89 2 2 2h16c1.11 0 2-.89 2-2V8c0-1.11-.89-2-2-2zm-5-2c.55 0 1 .45 1 1s-.45 1-1 1-1-.45-1-1 .45-1 1-1zM9 4c.55 0 1 .45 1 1s-.45 1-1 1-1-.45-1-1 .45-1 1-1zm11 15H4v-2h16v2zm0-5H4V8h5.08L7 10.83 8.62 12 11 8.76l1-1.36 1 1.36L15.38 12 17 10.83 14.92 8H20v6z\"/>";
    }
}
