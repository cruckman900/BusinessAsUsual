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
            Capabilities = new List<string> { "invoices", "payments", "reports", "receivables", "payables", "payroll" },
            HealthUrl = $"{financeApiUrl}/health",
            TenantMode = "tenant-per-database",
            SupportsMobile = true,
            MobileUISpecUrl = $"{financeApiUrl}/api/finance/mobile/ui-spec",
            MobileContractVersion = "1.0.0",
            NavigationItems = new List<RegisterModuleRequest.NavigationItemDto>
            {
                new() { Label = "Dashboard", Route = "/finance", Icon = Icons.Dashboard },
                new() { Label = "Accounts Receivable", Route = "/finance/receivables", Icon = Icons.RequestQuote },
                new() { Label = "Invoices", Route = "/finance/invoices", Icon = Icons.Receipt },
                new() { Label = "Collections", Route = "/finance/receivables/collections", Icon = Icons.Assessment },
                new() { Label = "Accounts Payable", Route = "/finance/payables", Icon = Icons.ReceiptLong },
                new() { Label = "Bills", Route = "/finance/payables/bills", Icon = Icons.Receipt },
                new() { Label = "Vendor Payments", Route = "/finance/payables/vendor-payments", Icon = Icons.Payments },
                new() { Label = "Payments", Route = "/finance/payments", Icon = Icons.Payments },
                new() { Label = "Payroll", Route = "/finance/payroll", Icon = Icons.Wallet },
                new() { Label = "Pay Runs", Route = "/finance/payroll/pay-runs", Icon = Icons.Wallet },
                new() { Label = "Deductions", Route = "/finance/payroll/deductions", Icon = Icons.Assessment },
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
        public const string RequestQuote = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M19 8h-1V3H6v5H5c-1.66 0-3 1.34-3 3v6h4v4h12v-4h4v-6c0-1.66-1.34-3-3-3zm-5.83 7.83c-.29.29-.66.44-1.05.5V17h-.75v-.67c-.71-.15-1.32-.61-1.36-1.42h.9c.04.42.34.75 1.06.75.77 0 .94-.38.94-.62 0-.32-.17-.62-1.03-.82-.95-.23-1.6-.62-1.6-1.4 0-.66.53-1.09 1.19-1.23V11h.75v.68c.72.18 1.08.72 1.11 1.31h-.89c-.03-.44-.27-.75-.83-.75-.53 0-.85.24-.85.58 0 .3.23.49 1.03.7.8.2 1.6.53 1.6 1.52 0 .71-.53 1.1-1.19 1.24zM16 8H8V5h8v3z\"/>";
        public const string ReceiptLong = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M19.5 3.5L18 2l-1.5 1.5L15 2l-1.5 1.5L12 2l-1.5 1.5L9 2 7.5 3.5 6 2 4.5 3.5 3 2v16c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V2l-1.5 1.5zM19 20H5V4h14v16zM6 15h12v2H6zm7-7h5v2h-5zm0-3h5v2h-5zM6 5h5v5H6z\"/>";
        public const string Wallet = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M21 7.28V5c0-1.1-.9-2-2-2H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2v-2.28c.59-.35 1-.98 1-1.72V9c0-.74-.41-1.37-1-1.72zM20 9v6h-7V9h7zM5 19V5h14v2h-6c-1.1 0-2 .9-2 2v6c0 1.1.9 2 2 2h6v2H5z\"/><circle cx=\"16\" cy=\"12\" r=\"1.5\"/>";
    }
}
