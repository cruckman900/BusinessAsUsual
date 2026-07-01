using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/mobile/ui")]
public class MobileUIController : ControllerBase
{
    [HttpGet("navigation")]
    public IActionResult GetNavigation()
    {
        var navigation = new
        {
            moduleName = "CRM",
            navigationItems = new object[]
            {
                new { label = "Home", route = "/crm", icon = "home", children = (object[]?)null },
                new { 
                    label = "Leads", 
                    route = "/crm/leads", 
                    icon = "person_search",
                    children = new object[]
                    {
                        new { label = "All Leads", route = "/crm/leads", icon = "list" },
                        new { label = "Add Lead", route = "/crm/leads/new", icon = "person_add" }
                    }
                },
                new { 
                    label = "Opportunities", 
                    route = "/crm/opportunities", 
                    icon = "trending_up",
                    children = new object[]
                    {
                        new { label = "Pipeline", route = "/crm/opportunities", icon = "list" },
                        new { label = "Add Opportunity", route = "/crm/opportunities/new", icon = "add_circle" }
                    }
                },
                new { 
                    label = "Customers", 
                    route = "/crm/customers", 
                    icon = "business",
                    children = new object[]
                    {
                        new { label = "All Customers", route = "/crm/customers", icon = "list" },
                        new { label = "Add Customer", route = "/crm/customers/new", icon = "add_business" }
                    }
                },
                new { label = "Activities", route = "/crm/activities", icon = "event", children = (object[]?)null },
                new { label = "Reports", route = "/crm/reports", icon = "bar_chart", children = (object[]?)null },
                new { label = "Settings", route = "/crm/settings", icon = "settings", children = (object[]?)null }
            }
        };

        return Ok(navigation);
    }
}
