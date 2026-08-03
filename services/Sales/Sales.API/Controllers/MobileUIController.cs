using Sales.Contracts.Navigation;
using Sales.Contracts.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace Sales.API.Controllers;

[ApiController]
[Route("api/sales/mobile")]
public class MobileUIController : ControllerBase
{
    /// <summary>Get the complete mobile UI specification for the Sales module.</summary>
    [HttpGet("ui-spec")]
    public ActionResult<MobileUISpecification> GetUISpecification()
    {
        var spec = new MobileUISpecification
        {
            ModuleId = "sales",
            ModuleName = "Sales",
            DisplayName = "Sales",
            Version = "1.0.0",
            Navigation = GetNavigationMap(),
            Screens = new Dictionary<string, object>
            {
                { "order-list", GetOrderListSpec() },
                { "quote-list", GetQuoteListSpec() },
                { "customer-list", GetCustomerListSpec() },
                { "product-list", GetProductListSpec() }
            }
        };

        return Ok(spec);
    }

    /// <summary>Get navigation structure for the mobile app.</summary>
    [HttpGet("navigation")]
    public ActionResult<ModuleNavigationMap> GetNavigation() => Ok(GetNavigationMap());

    [HttpGet("ui-spec/order-list")]
    public ActionResult<ListScreenSpec> GetOrderListSpecification() => Ok(GetOrderListSpec());

    [HttpGet("ui-spec/quote-list")]
    public ActionResult<ListScreenSpec> GetQuoteListSpecification() => Ok(GetQuoteListSpec());

    [HttpGet("ui-spec/customer-list")]
    public ActionResult<ListScreenSpec> GetCustomerListSpecification() => Ok(GetCustomerListSpec());

    // ----------------------------------------------------------------------
    // Navigation
    // ----------------------------------------------------------------------
    private static ModuleNavigationMap GetNavigationMap() => new()
    {
        ModuleId = "sales",
        ModuleName = "Sales",
        Icon = "point_of_sale",
        Items = new List<NavigationItem>
        {
            new() { Id = "dashboard", Label = "Dashboard", Icon = "dashboard", Screen = "dashboard", Route = "/sales" },
            new() { Id = "orders", Label = "Orders", Icon = "receipt_long", Screen = "order-list", Route = "/sales/orders" },
            new() { Id = "quotes", Label = "Quotes", Icon = "description", Screen = "quote-list", Route = "/sales/quotes" },
            new() { Id = "customers", Label = "Customers", Icon = "people", Screen = "customer-list", Route = "/sales/customers" },
            new() { Id = "products", Label = "Products", Icon = "inventory", Screen = "product-list", Route = "/sales/products" },
            new()
            {
                Id = "reports",
                Label = "Reports",
                Icon = "assessment",
                Screen = "report-dashboard",
                Route = "/sales/reports",
                Children = new List<NavigationItem>
                {
                    new() { Id = "sales-summary", Label = "Sales Summary", Icon = "trending_up", Screen = "sales-summary-report", Route = "/sales/reports/summary" },
                    new() { Id = "revenue", Label = "Revenue", Icon = "attach_money", Screen = "revenue-report", Route = "/sales/reports/revenue" },
                    new() { Id = "performance", Label = "Performance", Icon = "speed", Screen = "performance-report", Route = "/sales/reports/performance" }
                }
            }
        }
    };

    // ----------------------------------------------------------------------
    // Screen Specifications
    // ----------------------------------------------------------------------
    private static ListScreenSpec GetOrderListSpec() => new()
    {
        Type = "list",
        Title = "Sales Orders",
        SearchPlaceholder = "Search orders...",
        EnableSearch = true,
        EnableFilter = true,
        Columns = new List<ColumnDefinition>
        {
            new() { Name = "orderNumber", Label = "Order #", Type = "text", Sortable = true, Width = 120 },
            new() { Name = "customer", Label = "Customer", Type = "text", Sortable = true, Width = 180 },
            new() { Name = "orderDate", Label = "Order Date", Type = "date", Sortable = true, Width = 120 },
            new() { Name = "status", Label = "Status", Type = "badge", Sortable = true, Width = 120 },
            new() { Name = "total", Label = "Total", Type = "currency", Sortable = true, Width = 120 },
            new() { Name = "balanceDue", Label = "Balance Due", Type = "currency", Sortable = true, Width = 120 }
        },
        Actions = new List<ActionButton>
        {
            new() { Id = "add", Label = "Create Order", Icon = "add", Action = "navigate", NavigateTo = "order-form" },
            new() { Id = "view", Label = "View", Icon = "visibility", Action = "navigate", NavigateTo = "order-detail" },
            new() { Id = "confirm", Label = "Confirm", Icon = "check_circle", Action = "api-call", ApiEndpoint = "/api/sales/orders/{id}/confirm", RequiresConfirmation = true, ConfirmationMessage = "Confirm this order?" },
            new() { Id = "ship", Label = "Ship", Icon = "local_shipping", Action = "api-call", ApiEndpoint = "/api/sales/orders/{id}/ship", RequiresConfirmation = true, ConfirmationMessage = "Mark as shipped?" }
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
                    new() { Id = "confirmed", Label = "Confirmed", Value = "confirmed" },
                    new() { Id = "shipped", Label = "Shipped", Value = "shipped" },
                    new() { Id = "delivered", Label = "Delivered", Value = "delivered" },
                    new() { Id = "cancelled", Label = "Cancelled", Value = "cancelled" }
                }
            }
        },
        Stats = new List<StatCard>
        {
            new() { Id = "total-orders", Label = "Total Orders", Value = "0", Icon = "receipt_long", Color = "primary" },
            new() { Id = "pending", Label = "Pending", Value = "0", Icon = "pending", Color = "warning" },
            new() { Id = "shipped", Label = "Shipped", Value = "0", Icon = "local_shipping", Color = "success" }
        },
        EmptyStateMessage = "No orders found"
    };

    private static ListScreenSpec GetQuoteListSpec() => new()
    {
        Type = "list",
        Title = "Sales Quotes",
        SearchPlaceholder = "Search quotes...",
        EnableSearch = true,
        EnableFilter = true,
        Columns = new List<ColumnDefinition>
        {
            new() { Name = "quoteNumber", Label = "Quote #", Type = "text", Sortable = true, Width = 120 },
            new() { Name = "customer", Label = "Customer", Type = "text", Sortable = true, Width = 180 },
            new() { Name = "quoteDate", Label = "Quote Date", Type = "date", Sortable = true, Width = 120 },
            new() { Name = "expiryDate", Label = "Expiry Date", Type = "date", Sortable = true, Width = 120 },
            new() { Name = "status", Label = "Status", Type = "badge", Sortable = true, Width = 120 },
            new() { Name = "total", Label = "Total", Type = "currency", Sortable = true, Width = 120 }
        },
        Actions = new List<ActionButton>
        {
            new() { Id = "add", Label = "Create Quote", Icon = "add", Action = "navigate", NavigateTo = "quote-form" },
            new() { Id = "view", Label = "View", Icon = "visibility", Action = "navigate", NavigateTo = "quote-detail" },
            new() { Id = "send", Label = "Send", Icon = "send", Action = "api-call", ApiEndpoint = "/api/sales/quotes/{id}/send", RequiresConfirmation = true, ConfirmationMessage = "Send this quote?" },
            new() { Id = "convert", Label = "Convert to Order", Icon = "transform", Action = "api-call", ApiEndpoint = "/api/sales/quotes/{id}/convert", RequiresConfirmation = true, ConfirmationMessage = "Convert to order?" }
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
                    new() { Id = "sent", Label = "Sent", Value = "sent" },
                    new() { Id = "accepted", Label = "Accepted", Value = "accepted" },
                    new() { Id = "rejected", Label = "Rejected", Value = "rejected" },
                    new() { Id = "expired", Label = "Expired", Value = "expired" }
                }
            }
        },
        Stats = new List<StatCard>
        {
            new() { Id = "total-quotes", Label = "Total Quotes", Value = "0", Icon = "description", Color = "primary" },
            new() { Id = "pending", Label = "Pending", Value = "0", Icon = "pending", Color = "warning" },
            new() { Id = "accepted", Label = "Accepted", Value = "0", Icon = "check_circle", Color = "success" }
        },
        EmptyStateMessage = "No quotes found"
    };

    private static ListScreenSpec GetCustomerListSpec() => new()
    {
        Type = "list",
        Title = "Customers",
        SearchPlaceholder = "Search customers...",
        EnableSearch = true,
        Columns = new List<ColumnDefinition>
        {
            new() { Name = "name", Label = "Customer Name", Type = "text", Sortable = true, Width = 200 },
            new() { Name = "email", Label = "Email", Type = "text", Sortable = true, Width = 200 },
            new() { Name = "phone", Label = "Phone", Type = "text", Sortable = false, Width = 150 },
            new() { Name = "totalOrders", Label = "Orders", Type = "number", Sortable = true, Width = 100 },
            new() { Name = "totalRevenue", Label = "Revenue", Type = "currency", Sortable = true, Width = 120 }
        },
        Actions = new List<ActionButton>
        {
            new() { Id = "add", Label = "Add Customer", Icon = "add", Action = "navigate", NavigateTo = "customer-form" },
            new() { Id = "view", Label = "View", Icon = "visibility", Action = "navigate", NavigateTo = "customer-detail" }
        },
        EmptyStateMessage = "No customers found"
    };

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
            new() { Name = "price", Label = "Price", Type = "currency", Sortable = true, Width = 120 },
            new() { Name = "stock", Label = "Stock", Type = "number", Sortable = true, Width = 100 }
        },
        Actions = new List<ActionButton>
        {
            new() { Id = "view", Label = "View", Icon = "visibility", Action = "navigate", NavigateTo = "product-detail" }
        },
        EmptyStateMessage = "No products found"
    };
}
