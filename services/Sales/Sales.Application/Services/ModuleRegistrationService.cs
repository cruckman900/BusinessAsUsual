using Sales.Application.DTOs;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Sales.Application.Services;

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
        var salesApiUrl = _configuration["Sales:ApiBaseUrl"] ?? "http://localhost:5009";
        var salesWebUrl = _configuration["Sales:UiEntryPoint"] ?? "http://localhost:5010";

        var request = new RegisterModuleRequest
        {
            ModuleId = "sales",
            Key = "sales",
            DisplayName = "Sales",
            Description = "Manage quotes, orders, and customer transactions",
            Version = "1.0.0",
            ApiBaseUrl = salesApiUrl,
            UiEntryPoint = $"{salesWebUrl}/sales",
            Icon = Icons.PointOfSale,
            Permissions = new List<string> { "sales.read", "sales.write", "sales.admin" },
            Capabilities = new List<string> { "quotes", "orders", "fulfillment", "reports" },
            HealthUrl = $"{salesApiUrl}/health",
            TenantMode = "tenant-per-database",
            SupportsMobile = true,
            MobileUISpecUrl = $"{salesApiUrl}/api/sales/mobile/ui-spec",
            MobileContractVersion = "1.0.0",
            NavigationItems = new List<RegisterModuleRequest.NavigationItemDto>
            {
                new() { Label = "Dashboard", Route = "/sales", Icon = Icons.Dashboard },
                new() 
                { 
                    Label = "Quotes", 
                    Route = "/sales/quotes-hub", 
                    Icon = Icons.RequestQuote,
                    Children = new List<RegisterModuleRequest.NavigationItemDto>
                    {
                        new() { Label = "All Quotes", Route = "/sales/quotes", Icon = Icons.RequestQuote },
                        new() { Label = "Create Quote", Route = "/sales/quotes/create", Icon = Icons.Add, Disabled = true },
                        new() { Label = "Quote Templates", Route = "/sales/quotes/templates", Icon = Icons.Description, Disabled = true },
                        new() { Label = "Expired Quotes", Route = "/sales/quotes/expired", Icon = Icons.EventBusy, Disabled = true }
                    }
                },
                new() 
                { 
                    Label = "Orders", 
                    Route = "/sales/orders-hub", 
                    Icon = Icons.ShoppingCart,
                    Children = new List<RegisterModuleRequest.NavigationItemDto>
                    {
                        new() { Label = "All Orders", Route = "/sales/orders", Icon = Icons.ShoppingCart },
                        new() { Label = "Create Order", Route = "/sales/orders/create", Icon = Icons.Add, Disabled = true },
                        new() { Label = "Fulfillment", Route = "/sales/orders/fulfillment", Icon = Icons.LocalShipping, Disabled = true },
                        new() { Label = "Order Tracking", Route = "/sales/orders/tracking", Icon = Icons.TrackChanges, Disabled = true }
                    }
                },
                new()
                {
                    Label = "Reports",
                    Route = "/sales/reports-hub",
                    Icon = Icons.Assessment,
                    Children = new List<RegisterModuleRequest.NavigationItemDto>
                    {
                        new() { Label = "Sales Analytics", Route = "/sales/reports/analytics", Icon = Icons.Analytics, Disabled = true },
                        new() { Label = "Revenue Report", Route = "/sales/reports/revenue", Icon = Icons.AttachMoney, Disabled = true },
                        new() { Label = "Conversion Metrics", Route = "/sales/reports/conversion", Icon = Icons.TrendingUp, Disabled = true }
                    }
                }
            }
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{registryUrl}/api/modules/register", request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine("✓ Successfully registered Sales module with Module Registry (including mobile support)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to register with Module Registry: {ex.Message}");
        }
    }

    // Material Icons SVG paths (MudBlazor compatible)
    private static class Icons
    {
        public const string PointOfSale = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M17 2H7c-1.1 0-2 .9-2 2v2c0 1.1.9 2 2 2h10c1.1 0 2-.9 2-2V4c0-1.1-.9-2-2-2zm0 4H7V4h10v2zm3 16H4c-1.1 0-2-.9-2-2V9c0-1.1.9-2 2-2h1v4c0 1.1.9 2 2 2h10c1.1 0 2-.9 2-2V7h1c1.1 0 2 .9 2 2v11c0 1.1-.9 2-2 2zm-1-9H5V7h14v6z\"/>";
        public const string Dashboard = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M3 13h8V3H3v10zm0 8h8v-6H3v6zm10 0h8V11h-8v10zm0-18v6h8V3h-8z\"/>";
        public const string RequestQuote = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M19 8h-1V3H6v5H5c-1.66 0-3 1.34-3 3v6h4v4h12v-4h4v-6c0-1.66-1.34-3-3-3zm-5.83 7.83c-.29.29-.66.44-1.05.5V17h-.75v-.67c-.71-.15-1.32-.61-1.36-1.42h.9c.04.42.34.75 1.06.75.77 0 .94-.38.94-.62 0-.32-.17-.62-1.03-.82-.95-.23-1.6-.62-1.6-1.4 0-.66.53-1.09 1.19-1.23V11h.75v.68c.72.18 1.08.72 1.11 1.31h-.89c-.03-.44-.27-.75-.83-.75-.53 0-.85.24-.85.58 0 .3.23.49 1.03.7.8.2 1.6.53 1.6 1.52 0 .71-.53 1.1-1.19 1.24zM16 8H8V5h8v3z\"/>";
        public const string ShoppingCart = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M7 18c-1.1 0-1.99.9-1.99 2S5.9 22 7 22s2-.9 2-2-.9-2-2-2zM1 2v2h2l3.6 7.59-1.35 2.45c-.16.28-.25.61-.25.96 0 1.1.9 2 2 2h12v-2H7.42c-.14 0-.25-.11-.25-.25l.03-.12.9-1.63h7.45c.75 0 1.41-.41 1.75-1.03l3.58-6.49c.08-.14.12-.31.12-.48 0-.55-.45-1-1-1H5.21l-.94-2H1zm16 16c-1.1 0-1.99.9-1.99 2s.89 2 1.99 2 2-.9 2-2-.9-2-2-2z\"/>";
        public const string Assessment = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M19 3H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zM9 17H7v-7h2v7zm4 0h-2V7h2v10zm4 0h-2v-4h2v4z\"/>";
        public const string Add = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M19 13h-6v6h-2v-6H5v-2h6V5h2v6h6v2z\"/>";
        public const string Description = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M14 2H6c-1.1 0-1.99.9-1.99 2L4 20c0 1.1.89 2 1.99 2H18c1.1 0 2-.9 2-2V8l-6-6zm2 16H8v-2h8v2zm0-4H8v-2h8v2zm-3-5V3.5L18.5 9H13z\"/>";
        public const string EventBusy = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M9.31 17l2.44-2.44L14.19 17l1.06-1.06-2.44-2.44 2.44-2.44L14.19 10l-2.44 2.44L9.31 10l-1.06 1.06 2.44 2.44-2.44 2.44L9.31 17zM19 3h-1V1h-2v2H8V1H6v2H5c-1.11 0-1.99.9-1.99 2L3 19c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H5V8h14v11z\"/>";
        public const string LocalShipping = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M20 8h-3V4H3c-1.1 0-2 .9-2 2v11h2c0 1.66 1.34 3 3 3s3-1.34 3-3h6c0 1.66 1.34 3 3 3s3-1.34 3-3h2v-5l-3-4zM6 18.5c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5zm13.5-9l1.96 2.5H17V9.5h2.5zm-1.5 9c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5z\"/>";
        public const string TrackChanges = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M19.07 4.93l-1.41 1.41C19.1 7.79 20 9.79 20 12c0 4.42-3.58 8-8 8s-8-3.58-8-8c0-4.08 3.05-7.44 7-7.93v2.02C8.16 6.57 6 9.03 6 12c0 3.31 2.69 6 6 6s6-2.69 6-6c0-1.66-.67-3.16-1.76-4.24l-1.41 1.41C15.55 9.89 16 10.9 16 12c0 2.21-1.79 4-4 4s-4-1.79-4-4c0-1.86 1.28-3.41 3-3.86v2.14c-.6.35-1 .98-1 1.72 0 1.1.9 2 2 2s2-.9 2-2c0-.74-.4-1.38-1-1.72V2h-1C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10c0-2.76-1.12-5.26-2.93-7.07z\"/>";
        public const string Analytics = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M19 3H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm-5 14h-1.5v-6H14v6zm-3.5 0h-1.5v-8h1.5v8zM8 17H6.5v-4H8v4zm9.5 0H16v-9h1.5v9z\"/>";
        public const string AttachMoney = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M11.8 10.9c-2.27-.59-3-1.2-3-2.15 0-1.09 1.01-1.85 2.7-1.85 1.78 0 2.44.85 2.5 2.1h2.21c-.07-1.72-1.12-3.3-3.21-3.81V3h-3v2.16c-1.94.42-3.5 1.68-3.5 3.61 0 2.31 1.91 3.46 4.7 4.13 2.5.6 3 1.48 3 2.41 0 .69-.49 1.79-2.7 1.79-2.06 0-2.87-.92-2.98-2.1h-2.2c.12 2.19 1.76 3.42 3.68 3.83V21h3v-2.15c1.95-.37 3.5-1.5 3.5-3.55 0-2.84-2.43-3.81-4.7-4.4z\"/>";
        public const string TrendingUp = "<path d=\"M0 0h24v24H0z\" fill=\"none\"/><path d=\"M16 6l2.29 2.29-4.88 4.88-4-4L2 16.59 3.41 18l6-6 4 4 6.3-6.29L22 12V6z\"/>";
    }
}
