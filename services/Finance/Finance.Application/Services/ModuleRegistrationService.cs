using Finance.Application.DTOs;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Finance.Application.Services;

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
        var financeApiUrl = _configuration["Finance:ApiBaseUrl"] ?? "http://localhost:5007";
        var financeWebUrl = _configuration["Finance:UiEntryPoint"] ?? "http://localhost:5008";

        var request = new RegisterModuleRequest
        {
            ModuleId = "finance",
            Key = "finance",
            DisplayName = "Finance",
            Description = "Manage invoices, payments, and revenue reporting",
            Version = "1.0.0",
            ApiBaseUrl = financeApiUrl,
            UiEntryPoint = $"{financeWebUrl}/finance",
            Icon = Icons.AccountBalance,
            Permissions = new List<string> { "finance.read", "finance.write", "finance.admin" },
            Capabilities = new List<string> { "invoices", "payments", "reports" },
            HealthUrl = $"{financeApiUrl}/health",
            TenantMode = "tenant-per-database",
            SupportsMobile = true,
            MobileUISpecUrl = $"{financeApiUrl}/api/finance/mobile/ui-spec",
            MobileContractVersion = "1.0.0",
            NavigationItems = new List<RegisterModuleRequest.NavigationItemDto>
            {
                new() { Label = "Dashboard", Route = "/finance", Icon = Icons.Dashboard },
                new() { Label = "Invoices", Route = "/finance/invoices", Icon = Icons.Receipt },
                new() { Label = "Payments", Route = "/finance/payments", Icon = Icons.Payments },
                new() { Label = "Reports", Route = "/finance/reports", Icon = Icons.Assessment }
            }
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{registryUrl}/api/modules/register", request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine("✓ Successfully registered Finance module with Module Registry (including mobile support)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to register with Module Registry: {ex.Message}");
        }
    }

    // Material Icons SVG paths (MudBlazor compatible)
    private static class Icons
    {
        public const string AccountBalance = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M4 10v7h3v-7H4zm6 0v7h3v-7h-3zM2 22h19v-3H2v3zm14-12v7h3v-7h-3zm-4.5-9L2 6v2h19V6l-9.5-5z\"/>";
        public const string Dashboard = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M3 13h8V3H3v10zm0 8h8v-6H3v6zm10 0h8V11h-8v10zm0-18v6h8V3h-8z\"/>";
        public const string Receipt = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M18 17H6v-2h12v2zm0-4H6v-2h12v2zm0-4H6V7h12v2zM3 22l1.5-1.5L6 22l1.5-1.5L9 22l1.5-1.5L12 22l1.5-1.5L15 22l1.5-1.5L18 22l1.5-1.5L21 22V2l-1.5 1.5L18 2l-1.5 1.5L15 2l-1.5 1.5L12 2l-1.5 1.5L9 2 7.5 3.5 6 2 4.5 3.5 3 2v20z\"/>";
        public const string Payments = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M20 4H4c-1.11 0-1.99.89-1.99 2L2 18c0 1.11.89 2 2 2h16c1.11 0 2-.89 2-2V6c0-1.11-.89-2-2-2zm0 14H4v-6h16v6zm0-10H4V6h16v2z\"/>";
        public const string Assessment = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M19 3H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zM9 17H7v-7h2v7zm4 0h-2V7h2v10zm4 0h-2v-4h2v4z\"/>";
    }
}
