using Inventory.Contracts.Mobile;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/inventory/mobile")]
public class MobileUIController : ControllerBase
{
    [HttpGet("navigation")]
    public ActionResult<ModuleNavigationMap> GetNavigation()
    {
        var navigation = new ModuleNavigationMap
        {
            ModuleName = "Inventory",
            ModuleIcon = "inventory_2",
            DisplayOrder = 60,
            BaseRoute = "/inventory",
            NavigationItems = new List<NavigationItem>
            {
                new NavigationItem
                {
                    Label = "Dashboard",
                    Icon = "dashboard",
                    Route = "/inventory"
                },
                new NavigationItem
                {
                    Label = "Products",
                    Icon = "inventory",
                    Route = "/inventory/products"
                },
                new NavigationItem
                {
                    Label = "Warehouses",
                    Icon = "warehouse",
                    Route = "/inventory/warehouses"
                },
                new NavigationItem
                {
                    Label = "Stock Management",
                    Icon = "inventory_2",
                    Route = "/inventory/stock",
                    Children = new List<NavigationItem>
                    {
                        new NavigationItem
                        {
                            Label = "Stock Items",
                            Icon = "list_alt",
                            Route = "/inventory/stock/items"
                        },
                        new NavigationItem
                        {
                            Label = "Adjustments",
                            Icon = "edit",
                            Route = "/inventory/stock/adjustments"
                        },
                        new NavigationItem
                        {
                            Label = "Transfers",
                            Icon = "swap_horiz",
                            Route = "/inventory/stock/transfers"
                        },
                        new NavigationItem
                        {
                            Label = "Cycle Counts",
                            Icon = "fact_check",
                            Route = "/inventory/stock/cycle-counts"
                        }
                    }
                },
                new NavigationItem
                {
                    Label = "Purchase Orders",
                    Icon = "shopping_cart",
                    Route = "/inventory/purchase-orders"
                },
                new NavigationItem
                {
                    Label = "Suppliers",
                    Icon = "local_shipping",
                    Route = "/inventory/suppliers"
                },
                new NavigationItem
                {
                    Label = "Reports",
                    Icon = "assessment",
                    Route = "/inventory/reports",
                    Children = new List<NavigationItem>
                    {
                        new NavigationItem
                        {
                            Label = "Stock Valuation",
                            Icon = "attach_money",
                            Route = "/inventory/reports/valuation"
                        },
                        new NavigationItem
                        {
                            Label = "Movement History",
                            Icon = "history",
                            Route = "/inventory/reports/movements"
                        },
                        new NavigationItem
                        {
                            Label = "Low Stock Alert",
                            Icon = "warning",
                            Route = "/inventory/reports/low-stock"
                        }
                    }
                }
            }
        };

        return Ok(navigation);
    }
}
