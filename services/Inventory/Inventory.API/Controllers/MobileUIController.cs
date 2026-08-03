using Inventory.Contracts.Navigation;
using Inventory.Contracts.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/inventory/mobile")]
public class MobileUIController : ControllerBase
{
    /// <summary>Get the complete mobile UI specification for the Inventory module.</summary>
    [HttpGet("ui-spec")]
    public ActionResult<MobileUISpecification> GetUISpecification()
    {
        var spec = new MobileUISpecification
        {
            ModuleId = "inventory",
            ModuleName = "Inventory",
            DisplayName = "Inventory",
            Version = "1.0.0",
            Navigation = GetNavigationMap(),
            Screens = new Dictionary<string, object>
            {
                { "product-list", GetProductListSpec() },
                { "warehouse-list", GetWarehouseListSpec() },
                { "stock-item-list", GetStockItemListSpec() },
                { "purchase-order-list", GetPurchaseOrderListSpec() },
                { "supplier-list", GetSupplierListSpec() }
            }
        };

        return Ok(spec);
    }

    /// <summary>Get navigation structure for the mobile app.</summary>
    [HttpGet("navigation")]
    public ActionResult<ModuleNavigationMap> GetNavigation() => Ok(GetNavigationMap());

    [HttpGet("ui-spec/product-list")]
    public ActionResult<ListScreenSpec> GetProductListSpecification() => Ok(GetProductListSpec());

    [HttpGet("ui-spec/warehouse-list")]
    public ActionResult<ListScreenSpec> GetWarehouseListSpecification() => Ok(GetWarehouseListSpec());

    [HttpGet("ui-spec/stock-item-list")]
    public ActionResult<ListScreenSpec> GetStockItemListSpecification() => Ok(GetStockItemListSpec());

    // ----------------------------------------------------------------------
    // Navigation
    // ----------------------------------------------------------------------
    private static ModuleNavigationMap GetNavigationMap() => new()
    {
        ModuleId = "inventory",
        ModuleName = "Inventory",
        Icon = "inventory_2",
        Items = new List<NavigationItem>
        {
            new() { Id = "dashboard", Label = "Dashboard", Icon = "dashboard", Screen = "dashboard", Route = "/inventory" },
            new() { Id = "products", Label = "Products", Icon = "inventory", Screen = "product-list", Route = "/inventory/products" },
            new() { Id = "warehouses", Label = "Warehouses", Icon = "warehouse", Screen = "warehouse-list", Route = "/inventory/warehouses" },
            new()
            {
                Id = "stock",
                Label = "Stock Management",
                Icon = "inventory_2",
                Screen = "stock-dashboard",
                Route = "/inventory/stock",
                Children = new List<NavigationItem>
                {
                    new() { Id = "stock-items", Label = "Stock Items", Icon = "list_alt", Screen = "stock-item-list", Route = "/inventory/stock/items" },
                    new() { Id = "adjustments", Label = "Adjustments", Icon = "edit", Screen = "adjustment-list", Route = "/inventory/stock/adjustments" },
                    new() { Id = "transfers", Label = "Transfers", Icon = "swap_horiz", Screen = "transfer-list", Route = "/inventory/stock/transfers" },
                    new() { Id = "cycle-counts", Label = "Cycle Counts", Icon = "fact_check", Screen = "cycle-count-list", Route = "/inventory/stock/cycle-counts" }
                }
            },
            new() { Id = "purchase-orders", Label = "Purchase Orders", Icon = "shopping_cart", Screen = "purchase-order-list", Route = "/inventory/purchase-orders" },
            new() { Id = "suppliers", Label = "Suppliers", Icon = "local_shipping", Screen = "supplier-list", Route = "/inventory/suppliers" },
            new()
            {
                Id = "reports",
                Label = "Reports",
                Icon = "assessment",
                Screen = "report-dashboard",
                Route = "/inventory/reports",
                Children = new List<NavigationItem>
                {
                    new() { Id = "valuation", Label = "Stock Valuation", Icon = "attach_money", Screen = "valuation-report", Route = "/inventory/reports/valuation" },
                    new() { Id = "movements", Label = "Movement History", Icon = "history", Screen = "movement-report", Route = "/inventory/reports/movements" },
                    new() { Id = "low-stock", Label = "Low Stock Alert", Icon = "warning", Screen = "low-stock-report", Route = "/inventory/reports/low-stock" }
                }
            }
        }
    };

    // ----------------------------------------------------------------------
    // Screen Specifications
    // ----------------------------------------------------------------------
    private static ListScreenSpec GetProductListSpec() => new()
    {
        Type = "list",
        Title = "Products",
        SearchPlaceholder = "Search products...",
        EnableSearch = true,
        EnableFilter = true,
        Columns = new List<ColumnDefinition>
        {
            new() { Name = "sku", Label = "SKU", Type = "text", Sortable = true, Width = 120 },
            new() { Name = "name", Label = "Product Name", Type = "text", Sortable = true, Width = 200 },
            new() { Name = "category", Label = "Category", Type = "text", Sortable = true, Width = 150 },
            new() { Name = "stock", Label = "Stock", Type = "number", Sortable = true, Width = 100 },
            new() { Name = "price", Label = "Price", Type = "currency", Sortable = true, Width = 120 }
        },
        Actions = new List<ActionButton>
        {
            new() { Id = "add", Label = "Add Product", Icon = "add", Action = "navigate", NavigateTo = "product-form" },
            new() { Id = "edit", Label = "Edit", Icon = "edit", Action = "navigate", NavigateTo = "product-form" },
            new() { Id = "delete", Label = "Delete", Icon = "delete", Action = "api-call", ApiEndpoint = "/api/inventory/products/{id}", RequiresConfirmation = true, ConfirmationMessage = "Delete this product?" }
        },
        EmptyStateMessage = "No products found"
    };

    private static ListScreenSpec GetWarehouseListSpec() => new()
    {
        Type = "list",
        Title = "Warehouses",
        SearchPlaceholder = "Search warehouses...",
        EnableSearch = true,
        Columns = new List<ColumnDefinition>
        {
            new() { Name = "code", Label = "Code", Type = "text", Sortable = true, Width = 100 },
            new() { Name = "name", Label = "Name", Type = "text", Sortable = true, Width = 200 },
            new() { Name = "location", Label = "Location", Type = "text", Sortable = true, Width = 200 },
            new() { Name = "capacity", Label = "Capacity", Type = "number", Sortable = true, Width = 120 }
        },
        Actions = new List<ActionButton>
        {
            new() { Id = "add", Label = "Add Warehouse", Icon = "add", Action = "navigate", NavigateTo = "warehouse-form" },
            new() { Id = "edit", Label = "Edit", Icon = "edit", Action = "navigate", NavigateTo = "warehouse-form" }
        },
        EmptyStateMessage = "No warehouses found"
    };

    private static ListScreenSpec GetStockItemListSpec() => new()
    {
        Type = "list",
        Title = "Stock Items",
        SearchPlaceholder = "Search stock items...",
        EnableSearch = true,
        EnableFilter = true,
        Columns = new List<ColumnDefinition>
        {
            new() { Name = "product", Label = "Product", Type = "text", Sortable = true, Width = 200 },
            new() { Name = "warehouse", Label = "Warehouse", Type = "text", Sortable = true, Width = 150 },
            new() { Name = "quantity", Label = "Quantity", Type = "number", Sortable = true, Width = 100 },
            new() { Name = "available", Label = "Available", Type = "number", Sortable = true, Width = 100 },
            new() { Name = "reorderPoint", Label = "Reorder Point", Type = "number", Sortable = true, Width = 120 }
        },
        Actions = new List<ActionButton>
        {
            new() { Id = "adjust", Label = "Adjust", Icon = "edit", Action = "navigate", NavigateTo = "adjustment-form" }
        },
        Stats = new List<StatCard>
        {
            new() { Id = "total-items", Label = "Total Items", Value = "0", Icon = "inventory", Color = "primary" },
            new() { Id = "low-stock", Label = "Low Stock", Value = "0", Icon = "warning", Color = "warning" }
        },
        EmptyStateMessage = "No stock items found"
    };

    private static ListScreenSpec GetPurchaseOrderListSpec() => new()
    {
        Type = "list",
        Title = "Purchase Orders",
        SearchPlaceholder = "Search purchase orders...",
        EnableSearch = true,
        EnableFilter = true,
        Columns = new List<ColumnDefinition>
        {
            new() { Name = "orderNumber", Label = "PO Number", Type = "text", Sortable = true, Width = 120 },
            new() { Name = "supplier", Label = "Supplier", Type = "text", Sortable = true, Width = 180 },
            new() { Name = "orderDate", Label = "Order Date", Type = "date", Sortable = true, Width = 120 },
            new() { Name = "status", Label = "Status", Type = "badge", Sortable = true, Width = 120 },
            new() { Name = "total", Label = "Total", Type = "currency", Sortable = true, Width = 120 }
        },
        Actions = new List<ActionButton>
        {
            new() { Id = "add", Label = "Create PO", Icon = "add", Action = "navigate", NavigateTo = "purchase-order-form" },
            new() { Id = "view", Label = "View", Icon = "visibility", Action = "navigate", NavigateTo = "purchase-order-detail" }
        },
        Filters = new List<FilterOption>
        {
            new()
            {
                Id = "status",
                Label = "Status",
                Type = "select",
                Values = new List<FilterValue>
                {
                    new() { Id = "all", Label = "All", Value = "" },
                    new() { Id = "draft", Label = "Draft", Value = "draft" },
                    new() { Id = "submitted", Label = "Submitted", Value = "submitted" },
                    new() { Id = "received", Label = "Received", Value = "received" }
                }
            }
        },
        EmptyStateMessage = "No purchase orders found"
    };

    private static ListScreenSpec GetSupplierListSpec() => new()
    {
        Type = "list",
        Title = "Suppliers",
        SearchPlaceholder = "Search suppliers...",
        EnableSearch = true,
        Columns = new List<ColumnDefinition>
        {
            new() { Name = "name", Label = "Supplier Name", Type = "text", Sortable = true, Width = 200 },
            new() { Name = "contact", Label = "Contact", Type = "text", Sortable = true, Width = 150 },
            new() { Name = "email", Label = "Email", Type = "text", Sortable = false, Width = 200 },
            new() { Name = "phone", Label = "Phone", Type = "text", Sortable = false, Width = 150 }
        },
        Actions = new List<ActionButton>
        {
            new() { Id = "add", Label = "Add Supplier", Icon = "add", Action = "navigate", NavigateTo = "supplier-form" },
            new() { Id = "edit", Label = "Edit", Icon = "edit", Action = "navigate", NavigateTo = "supplier-form" }
        },
        EmptyStateMessage = "No suppliers found"
    };
}
