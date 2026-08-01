using Inventory.Application.DTOs;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Inventory.Application.Services;

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
        var inventoryApiUrl = _configuration["Inventory:ApiBaseUrl"] ?? "http://localhost:7250";
        var inventoryWebUrl = _configuration["Inventory:UiEntryPoint"] ?? "http://localhost:5008";

        var request = new RegisterModuleRequest
        {
            ModuleId = "inventory",
            Key = "inventory",
            DisplayName = "Inventory",
            Description = "Comprehensive inventory management with products, warehouses, stock tracking, purchase orders, and suppliers",
            Version = "1.0.0",
            ApiBaseUrl = inventoryApiUrl,
            UiEntryPoint = $"{inventoryWebUrl}/inventory",
            Icon = "inventory_2",
            Permissions = new List<string> { "inventory.read", "inventory.write", "inventory.admin" },
            Capabilities = new List<string> { "products", "warehouses", "stock", "purchase-orders", "suppliers", "barcode" },
            HealthUrl = $"{inventoryApiUrl}/health",
            TenantMode = "tenant-per-database",
            SupportsMobile = true,
            MobileUISpecUrl = $"{inventoryApiUrl}/api/inventory/mobile/navigation",
            MobileContractVersion = "1.0.0",
            NavigationItems = new List<RegisterModuleRequest.NavigationItemDto>
            {
                new() { Label = "Dashboard", Route = "/inventory", Icon = "dashboard" },
                new() { Label = "Products", Route = "/inventory/products", Icon = "inventory" },
                new() { Label = "Warehouses", Route = "/inventory/warehouses", Icon = "warehouse" },
                new()
                {
                    Label = "Stock Management",
                    Route = "/inventory/stock",
                    Icon = "inventory_2",
                    Children = new List<RegisterModuleRequest.NavigationItemDto>
                    {
                        new() { Label = "Stock Items", Route = "/inventory/stock/items", Icon = "list_alt" },
                        new() { Label = "Adjustments", Route = "/inventory/stock/adjustments", Icon = "edit" },
                        new() { Label = "Transfers", Route = "/inventory/stock/transfers", Icon = "swap_horiz" },
                        new() { Label = "Cycle Counts", Route = "/inventory/stock/cycle-counts", Icon = "fact_check" }
                    }
                },
                new() { Label = "Purchase Orders", Route = "/inventory/purchase-orders", Icon = "shopping_cart" },
                new() { Label = "Suppliers", Route = "/inventory/suppliers", Icon = "local_shipping" },
                new()
                {
                    Label = "Reports",
                    Route = "/inventory/reports",
                    Icon = "assessment",
                    Children = new List<RegisterModuleRequest.NavigationItemDto>
                    {
                        new() { Label = "Stock Valuation", Route = "/inventory/reports/valuation", Icon = "attach_money" },
                        new() { Label = "Movement History", Route = "/inventory/reports/movements", Icon = "history" },
                        new() { Label = "Low Stock Alert", Route = "/inventory/reports/low-stock", Icon = "warning" }
                    }
                }
            }
        };

        await _httpClient.PostAsJsonAsync($"{registryUrl}/api/modules/register", request);
    }
}
