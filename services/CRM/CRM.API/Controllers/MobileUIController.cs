using CRM.Application.Services;
using CRM.Contracts.Navigation;
using CRM.Contracts.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/crm/mobile")]
public class MobileUIController : ControllerBase
{
    private readonly ILeadService _leadService;
    private readonly IOpportunityService _opportunityService;
    private readonly ICustomerService _customerService;

    public MobileUIController(
        ILeadService leadService,
        IOpportunityService opportunityService,
        ICustomerService customerService)
    {
        _leadService = leadService;
        _opportunityService = opportunityService;
        _customerService = customerService;
    }

    /// <summary>
    /// Get the complete mobile UI specification for the CRM module.
    /// </summary>
    [HttpGet("ui-spec")]
    public ActionResult<MobileUISpecification> GetUISpecification()
    {
        var spec = new MobileUISpecification
        {
            ModuleId = "crm",
            ModuleName = "Customer Relationship Management",
            Version = "1.0.0",
            Navigation = GetNavigationMap(),
            Screens = new Dictionary<string, object>
            {
                { "lead-list", GetLeadListSpec() },
                { "opportunity-list", GetOpportunityListSpec() },
                { "customer-list", GetCustomerListSpec() },
                { "activity-list", GetActivityListSpec() },
                { "lead-detail", GetLeadDetailSpec() },
                { "lead-form", GetLeadFormSpec() },
                { "report-dashboard", GetReportDashboardSpec() }
            }
        };

        return Ok(spec);
    }

    /// <summary>Get navigation structure for the mobile app.</summary>
    [HttpGet("navigation")]
    public ActionResult<ModuleNavigationMap> GetNavigation() => Ok(GetNavigationMap());

    /// <summary>Get lead list screen specification.</summary>
    [HttpGet("ui-spec/lead-list")]
    public ActionResult<ListScreenSpec> GetLeadListSpecification() => Ok(GetLeadListSpec());

    /// <summary>Get lead detail screen specification.</summary>
    [HttpGet("ui-spec/lead-detail")]
    public ActionResult<DetailScreenSpec> GetLeadDetailSpecification() => Ok(GetLeadDetailSpec());

    /// <summary>Get lead form specification.</summary>
    [HttpGet("ui-spec/lead-form")]
    public ActionResult<FormScreenSpec> GetLeadFormSpecification() => Ok(GetLeadFormSpec());

    /// <summary>
    /// Get row data for a given mobile list screen. Rows are dictionaries keyed
    /// by the column Name values defined in that screen's specification.
    /// Lead/opportunity/customer data is live; other screens return sample rows.
    /// </summary>
    [HttpGet("data/{screenKey}")]
    public async Task<ActionResult<List<Dictionary<string, string>>>> GetScreenData(string screenKey)
    {
        var rows = screenKey switch
        {
            "lead-list" => await GetLeadRows(),
            "opportunity-list" => await GetOpportunityRows(),
            "customer-list" => await GetCustomerRows(),
            "activity-list" => GetActivityRows(),
            _ => new List<Dictionary<string, string>>()
        };

        return Ok(rows);
    }

    // ---- Live data rows (with sample fallback so screens are never empty in a demo) ----

    private async Task<List<Dictionary<string, string>>> GetLeadRows()
    {
        var leads = await _leadService.GetAllLeadsAsync();
        var rows = leads.Select(l => new Dictionary<string, string>
        {
            ["fullName"] = l.FullName,
            ["company"] = l.Company ?? string.Empty,
            ["email"] = l.Email,
            ["phone"] = l.Phone ?? string.Empty,
            ["source"] = l.Source,
            ["estimatedValue"] = l.EstimatedValue.HasValue ? l.EstimatedValue.Value.ToString("C0") : string.Empty,
            ["status"] = l.Status
        }).ToList();

        if (rows.Count == 0)
        {
            rows = Rows(
                new() { ["fullName"] = "Taylor Brooks", ["company"] = "Northwind Traders", ["email"] = "taylor.brooks@northwind.com", ["phone"] = "(415) 555-0102", ["source"] = "Website", ["estimatedValue"] = "$48,000", ["status"] = "New" },
                new() { ["fullName"] = "Chris Nguyen", ["company"] = "Contoso Ltd", ["email"] = "chris.nguyen@contoso.com", ["phone"] = "(415) 555-0148", ["source"] = "Referral", ["estimatedValue"] = "$120,000", ["status"] = "Qualified" },
                new() { ["fullName"] = "Morgan Ellis", ["company"] = "Fabrikam Inc", ["email"] = "morgan.ellis@fabrikam.com", ["phone"] = "(415) 555-0177", ["source"] = "LinkedIn", ["estimatedValue"] = "$32,500", ["status"] = "Contacted" }
            );
        }

        return rows;
    }

    private async Task<List<Dictionary<string, string>>> GetOpportunityRows()
    {
        var opps = await _opportunityService.GetAllOpportunitiesAsync();
        var rows = opps.Select(o => new Dictionary<string, string>
        {
            ["name"] = o.Name,
            ["customerName"] = o.CustomerName ?? string.Empty,
            ["stage"] = o.Stage,
            ["amount"] = o.Amount.ToString("C0"),
            ["probability"] = $"{o.Probability:0}%",
            ["expectedCloseDate"] = o.ExpectedCloseDate.HasValue ? o.ExpectedCloseDate.Value.ToString("yyyy-MM-dd") : string.Empty
        }).ToList();

        if (rows.Count == 0)
        {
            rows = Rows(
                new() { ["name"] = "Northwind Platform Renewal", ["customerName"] = "Northwind Traders", ["stage"] = "Proposal", ["amount"] = "$85,000", ["probability"] = "60%", ["expectedCloseDate"] = "2026-08-15" },
                new() { ["name"] = "Contoso Expansion", ["customerName"] = "Contoso Ltd", ["stage"] = "Negotiation", ["amount"] = "$140,000", ["probability"] = "75%", ["expectedCloseDate"] = "2026-07-30" },
                new() { ["name"] = "Fabrikam Pilot", ["customerName"] = "Fabrikam Inc", ["stage"] = "Qualification", ["amount"] = "$28,000", ["probability"] = "30%", ["expectedCloseDate"] = "2026-09-20" }
            );
        }

        return rows;
    }

    private async Task<List<Dictionary<string, string>>> GetCustomerRows()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        var rows = customers.Select(c => new Dictionary<string, string>
        {
            ["name"] = c.Name,
            ["companyName"] = c.CompanyName ?? string.Empty,
            ["email"] = c.Email,
            ["phone"] = c.Phone ?? string.Empty,
            ["industry"] = c.Industry ?? string.Empty,
            ["annualRevenue"] = c.AnnualRevenue.HasValue ? c.AnnualRevenue.Value.ToString("C0") : string.Empty
        }).ToList();

        if (rows.Count == 0)
        {
            rows = Rows(
                new() { ["name"] = "Northwind Traders", ["companyName"] = "Northwind Traders", ["email"] = "hello@northwind.com", ["phone"] = "(415) 555-0190", ["industry"] = "Retail", ["annualRevenue"] = "$12,000,000" },
                new() { ["name"] = "Contoso Ltd", ["companyName"] = "Contoso Ltd", ["email"] = "sales@contoso.com", ["phone"] = "(415) 555-0133", ["industry"] = "Technology", ["annualRevenue"] = "$45,000,000" },
                new() { ["name"] = "Fabrikam Inc", ["companyName"] = "Fabrikam Inc", ["email"] = "info@fabrikam.com", ["phone"] = "(415) 555-0125", ["industry"] = "Manufacturing", ["annualRevenue"] = "$8,500,000" }
            );
        }

        return rows;
    }

    private static List<Dictionary<string, string>> GetActivityRows() => Rows(
        new() { ["subject"] = "Discovery call", ["type"] = "Call", ["relatedTo"] = "Contoso Ltd", ["owner"] = "Marcus Lee", ["dueDate"] = "2026-07-14", ["status"] = "Open" },
        new() { ["subject"] = "Send proposal", ["type"] = "Email", ["relatedTo"] = "Northwind Traders", ["owner"] = "Marcus Lee", ["dueDate"] = "2026-07-16", ["status"] = "Open" },
        new() { ["subject"] = "Contract review", ["type"] = "Meeting", ["relatedTo"] = "Fabrikam Inc", ["owner"] = "Dana White", ["dueDate"] = "2026-07-10", ["status"] = "Completed" }
    );

    private static List<Dictionary<string, string>> Rows(params Dictionary<string, string>[] rows) => rows.ToList();

    // ---- Navigation ----

    private ModuleNavigationMap GetNavigationMap()
    {
        return new ModuleNavigationMap
        {
            ModuleId = "crm",
            ModuleName = "Customer Relationship Management",
            Icon = "contact_phone",
            Items = new List<NavigationItem>
            {
                new NavigationItem { Id = "leads", Label = "Leads", Icon = "person_search", Screen = "lead-list", Route = "/crm/leads" },
                new NavigationItem { Id = "opportunities", Label = "Opportunities", Icon = "trending_up", Screen = "opportunity-list", Route = "/crm/opportunities" },
                new NavigationItem { Id = "customers", Label = "Customers", Icon = "business", Screen = "customer-list", Route = "/crm/customers" },
                new NavigationItem { Id = "activities", Label = "Activities", Icon = "event", Screen = "activity-list", Route = "/crm/activities" },
                new NavigationItem { Id = "analytics", Label = "Analytics", Icon = "insights", Screen = "report-dashboard", Route = "/crm/analytics" }
            }
        };
    }

    // ---- List screens (shared helpers keep specs concise) ----

    private static ListScreenSpec ListSpec(string title, string searchPlaceholder, string addLabel, string addRoute, string emptyMessage, ActionButton[] rowActions, params ColumnDefinition[] columns)
    {
        var actions = new List<ActionButton>
        {
            new ActionButton { Id = "add", Label = addLabel, Icon = "add", Action = "navigate", NavigateTo = addRoute }
        };
        actions.AddRange(rowActions);

        return new ListScreenSpec
        {
            Title = title,
            SearchPlaceholder = searchPlaceholder,
            EnableSearch = true,
            EnableFilter = false,
            Columns = columns.ToList(),
            Actions = actions,
            EmptyStateMessage = emptyMessage
        };
    }

    private static ColumnDefinition Col(string name, string label, string type = "text", int width = 150, bool sortable = false)
        => new ColumnDefinition { Name = name, Label = label, Type = type, Width = width, Sortable = sortable };

    // ---- Per-row action factories (rendered in the Android overflow menu) ----
    // Convention: any action whose Id != "add" is treated as a per-row action.
    private static ActionButton RowView(string baseRoute) => new ActionButton
    { Id = "view", Label = "View Details", Icon = "visibility", Action = "navigate", NavigateTo = $"{baseRoute}/{{id}}" };

    private static ActionButton RowEdit(string baseRoute) => new ActionButton
    { Id = "edit", Label = "Edit", Icon = "edit", Action = "navigate", NavigateTo = $"{baseRoute}/{{id}}/edit" };

    private static ActionButton RowDelete(string baseRoute, string noun) => new ActionButton
    {
        Id = "delete", Label = "Delete", Icon = "delete", Action = "api-call",
        ApiEndpoint = $"{baseRoute}/{{id}}", RequiresConfirmation = true,
        ConfirmationMessage = $"Delete this {noun}? This action cannot be undone."
    };

    private static ActionButton RowConvert(string baseRoute) => new ActionButton
    {
        Id = "convert", Label = "Convert to Customer", Icon = "how_to_reg", Action = "api-call",
        ApiEndpoint = $"{baseRoute}/{{id}}/convert", RequiresConfirmation = true,
        ConfirmationMessage = "Convert this lead into a customer?"
    };

    /// <summary>Standard View / Edit / Delete row actions for a base route (e.g. "/crm/customers").</summary>
    private static ActionButton[] StdRowActions(string baseRoute, string noun) => new[]
    {
        RowView(baseRoute), RowEdit(baseRoute), RowDelete(baseRoute, noun)
    };

    private ListScreenSpec GetLeadListSpec() => ListSpec(
        "Leads", "Search leads...", "New Lead", "/crm/leads/new", "No leads found. Tap + to add your first lead.",
        new[] { RowView("/crm/leads"), RowConvert("/crm/leads"), RowEdit("/crm/leads"), RowDelete("/crm/leads", "lead") },
        Col("fullName", "Name", width: 200, sortable: true), Col("company", "Company", width: 180),
        Col("email", "Email", width: 220), Col("phone", "Phone", width: 150),
        Col("source", "Source", width: 130), Col("estimatedValue", "Est. Value", width: 120),
        Col("status", "Status", "badge", 120));

    private ListScreenSpec GetOpportunityListSpec() => ListSpec(
        "Opportunities", "Search opportunities...", "New Opportunity", "/crm/opportunities/new", "No opportunities in the pipeline.",
        StdRowActions("/crm/opportunities", "opportunity"),
        Col("name", "Opportunity", width: 220, sortable: true), Col("customerName", "Customer", width: 180),
        Col("stage", "Stage", "badge", 140), Col("amount", "Amount", width: 130),
        Col("probability", "Probability", width: 120), Col("expectedCloseDate", "Close Date", "date", 140));

    private ListScreenSpec GetCustomerListSpec() => ListSpec(
        "Customers", "Search customers...", "New Customer", "/crm/customers/new", "No customers found.",
        StdRowActions("/crm/customers", "customer"),
        Col("name", "Name", width: 200, sortable: true), Col("companyName", "Company", width: 200),
        Col("email", "Email", width: 220), Col("phone", "Phone", width: 150),
        Col("industry", "Industry", width: 150), Col("annualRevenue", "Revenue", width: 140));

    private ListScreenSpec GetActivityListSpec() => ListSpec(
        "Activities", "Search activities...", "New Activity", "/crm/activities/new", "No activities scheduled.",
        StdRowActions("/crm/activities", "activity"),
        Col("subject", "Subject", width: 220, sortable: true), Col("type", "Type", width: 120),
        Col("relatedTo", "Related To", width: 180), Col("owner", "Owner", width: 160),
        Col("dueDate", "Due", "date", 130), Col("status", "Status", "badge", 110));

    // ---- Detail screen ----

    private DetailScreenSpec GetLeadDetailSpec()
    {
        return new DetailScreenSpec
        {
            Title = "Lead Details",
            Sections = new List<SectionDefinition>
            {
                new SectionDefinition
                {
                    Id = "contact",
                    Title = "Contact Information",
                    Fields = new List<FieldDefinition>
                    {
                        new FieldDefinition { Name = "fullName", Label = "Full Name", Type = "text" },
                        new FieldDefinition { Name = "email", Label = "Email", Type = "email", Icon = "email" },
                        new FieldDefinition { Name = "phone", Label = "Phone", Type = "phone", Icon = "phone" },
                        new FieldDefinition { Name = "company", Label = "Company", Type = "text" },
                        new FieldDefinition { Name = "jobTitle", Label = "Job Title", Type = "text" }
                    }
                },
                new SectionDefinition
                {
                    Id = "qualification",
                    Title = "Qualification",
                    Fields = new List<FieldDefinition>
                    {
                        new FieldDefinition { Name = "status", Label = "Status", Type = "badge" },
                        new FieldDefinition { Name = "source", Label = "Source", Type = "text" },
                        new FieldDefinition { Name = "estimatedValue", Label = "Estimated Value", Type = "text" },
                        new FieldDefinition { Name = "assignedToEmployeeName", Label = "Assigned To", Type = "text" },
                        new FieldDefinition { Name = "createdDate", Label = "Created", Type = "date", Format = "MMM dd, yyyy" }
                    }
                }
            },
            Actions = new List<ActionButton>
            {
                new ActionButton { Id = "edit", Label = "Edit", Icon = "edit", Action = "navigate", NavigateTo = "/crm/leads/{id}/edit" },
                new ActionButton
                {
                    Id = "convert", Label = "Convert to Customer", Icon = "how_to_reg", Action = "api-call",
                    ApiEndpoint = "/api/crm/leads/{id}/convert", RequiresConfirmation = true,
                    ConfirmationMessage = "Convert this lead into a customer?"
                }
            }
        };
    }

    // ---- Form screen ----

    private FormScreenSpec GetLeadFormSpec()
    {
        return new FormScreenSpec
        {
            Title = "Lead Information",
            Sections = new List<FormSectionDefinition>
            {
                new FormSectionDefinition
                {
                    Id = "contact",
                    Title = "Contact Information",
                    Fields = new List<FormFieldDefinition>
                    {
                        new FormFieldDefinition { Name = "firstName", Label = "First Name", Type = "text", Required = true, MaxLength = 50, ValidationMessage = "First name is required" },
                        new FormFieldDefinition { Name = "lastName", Label = "Last Name", Type = "text", Required = true, MaxLength = 50, ValidationMessage = "Last name is required" },
                        new FormFieldDefinition { Name = "email", Label = "Email", Type = "email", Required = true, Pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$", ValidationMessage = "Please enter a valid email address" },
                        new FormFieldDefinition { Name = "phone", Label = "Phone", Type = "phone", Required = false, Placeholder = "(555) 123-4567" },
                        new FormFieldDefinition { Name = "company", Label = "Company", Type = "text", Required = false, MaxLength = 100 },
                        new FormFieldDefinition { Name = "jobTitle", Label = "Job Title", Type = "text", Required = false, MaxLength = 100 }
                    }
                },
                new FormSectionDefinition
                {
                    Id = "qualification",
                    Title = "Qualification",
                    Fields = new List<FormFieldDefinition>
                    {
                        new FormFieldDefinition
                        {
                            Name = "source", Label = "Source", Type = "select", Required = true,
                            Options = new List<SelectOption>
                            {
                                new SelectOption { Value = "Website", Label = "Website" },
                                new SelectOption { Value = "Referral", Label = "Referral" },
                                new SelectOption { Value = "LinkedIn", Label = "LinkedIn" },
                                new SelectOption { Value = "ColdCall", Label = "Cold Call" },
                                new SelectOption { Value = "Event", Label = "Event" }
                            },
                            ValidationMessage = "Please select a source"
                        },
                        new FormFieldDefinition { Name = "estimatedValue", Label = "Estimated Value", Type = "number", Required = false, Placeholder = "0" },
                        new FormFieldDefinition { Name = "description", Label = "Description", Type = "text", Required = false, MaxLength = 500 }
                    }
                }
            },
            Actions = new List<ActionButton>
            {
                new ActionButton { Id = "save", Label = "Save", Icon = "save", Action = "api-call", ApiEndpoint = "/api/crm/leads" },
                new ActionButton { Id = "cancel", Label = "Cancel", Icon = "close", Action = "navigate", NavigateTo = "/crm/leads" }
            },
            Validation = new ValidationRules
            {
                ErrorMessages = new Dictionary<string, string>
                {
                    { "required", "This field is required" },
                    { "email", "Please enter a valid email address" },
                    { "maxLength", "Value is too long" }
                }
            }
        };
    }

    // ---- Analytics dashboard (contract-driven charts) ----

    private ChartScreenSpec GetReportDashboardSpec()
    {
        return new ChartScreenSpec
        {
            Title = "CRM Analytics",
            EmptyStateMessage = "No analytics available.",
            Charts = new List<ChartSpec>
            {
                new ChartSpec
                {
                    Id = "pipeline-trend",
                    Title = "Pipeline Value Trend",
                    Subtitle = "Total open pipeline over the last 6 months",
                    ChartType = "line",
                    Labels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                    Series = new List<ChartSeries>
                    {
                        new ChartSeries
                        {
                            Name = "Pipeline ($K)", Color = "#1565C0",
                            Points = new List<ChartDataPoint>
                            {
                                new() { Value = 420 }, new() { Value = 468 }, new() { Value = 512 },
                                new() { Value = 495 }, new() { Value = 560 }, new() { Value = 610 }
                            }
                        }
                    }
                },
                new ChartSpec
                {
                    Id = "won-vs-lost",
                    Title = "Won vs. Lost",
                    Subtitle = "Monthly deal outcomes",
                    ChartType = "bar",
                    Labels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                    Series = new List<ChartSeries>
                    {
                        new ChartSeries
                        {
                            Name = "Won", Color = "#1B5E20",
                            Points = new List<ChartDataPoint>
                            {
                                new() { Value = 8 }, new() { Value = 6 }, new() { Value = 9 },
                                new() { Value = 7 }, new() { Value = 11 }, new() { Value = 10 }
                            }
                        },
                        new ChartSeries
                        {
                            Name = "Lost", Color = "#B3261E",
                            Points = new List<ChartDataPoint>
                            {
                                new() { Value = 3 }, new() { Value = 4 }, new() { Value = 2 },
                                new() { Value = 5 }, new() { Value = 3 }, new() { Value = 4 }
                            }
                        }
                    }
                },
                new ChartSpec
                {
                    Id = "pipeline-by-stage",
                    Title = "Pipeline by Stage",
                    Subtitle = "Share of open opportunities by stage",
                    ChartType = "donut",
                    Series = new List<ChartSeries>
                    {
                        new ChartSeries
                        {
                            Name = "Stages",
                            Points = new List<ChartDataPoint>
                            {
                                new() { Label = "Qualification", Value = 32, Color = "#1565C0" },
                                new() { Label = "Proposal", Value = 24, Color = "#1B5E20" },
                                new() { Label = "Negotiation", Value = 18, Color = "#B26A00" },
                                new() { Label = "Closing", Value = 12, Color = "#6A1B9A" }
                            }
                        }
                    }
                },
                new ChartSpec
                {
                    Id = "conversion-rate",
                    Title = "Lead Conversion Rate",
                    Subtitle = "Weekly conversion (%)",
                    ChartType = "sparkline",
                    Series = new List<ChartSeries>
                    {
                        new ChartSeries
                        {
                            Name = "Conversion", Color = "#00838F",
                            Points = new List<ChartDataPoint>
                            {
                                new() { Value = 18 }, new() { Value = 21 }, new() { Value = 19 },
                                new() { Value = 24 }, new() { Value = 26 }, new() { Value = 23 },
                                new() { Value = 28 }, new() { Value = 30 }
                            }
                        }
                    }
                }
            }
        };
    }
}
