using Finance.Contracts.Navigation;
using Finance.Contracts.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/finance/mobile")]
public class MobileUIController : ControllerBase
{
    /// <summary>Get the complete mobile UI specification for the Finance module.</summary>
    [HttpGet("ui-spec")]
    public ActionResult<MobileUISpecification> GetUISpecification()
    {
        var spec = new MobileUISpecification
        {
            ModuleId = "finance",
            ModuleName = "Finance",
            DisplayName = "Finance",
            Version = "1.0.0",
            Navigation = GetNavigationMap(),
            Screens = new Dictionary<string, object>
            {
                { "invoice-list", GetInvoiceListSpec() },
                { "invoice-detail", GetInvoiceDetailSpec() },
                { "invoice-form", GetInvoiceFormSpec() },
                { "payment-list", GetPaymentListSpec() },
                { "payment-form", GetPaymentFormSpec() },
                { "report-dashboard", GetReportDashboardSpec() }
            }
        };

        return Ok(spec);
    }

    /// <summary>Get navigation structure for the mobile app.</summary>
    [HttpGet("navigation")]
    public ActionResult<ModuleNavigationMap> GetNavigation() => Ok(GetNavigationMap());

    [HttpGet("ui-spec/invoice-list")]
    public ActionResult<ListScreenSpec> GetInvoiceListSpecification() => Ok(GetInvoiceListSpec());

    [HttpGet("ui-spec/invoice-detail")]
    public ActionResult<DetailScreenSpec> GetInvoiceDetailSpecification() => Ok(GetInvoiceDetailSpec());

    [HttpGet("ui-spec/invoice-form")]
    public ActionResult<FormScreenSpec> GetInvoiceFormSpecification() => Ok(GetInvoiceFormSpec());

    [HttpGet("ui-spec/payment-list")]
    public ActionResult<ListScreenSpec> GetPaymentListSpecification() => Ok(GetPaymentListSpec());

    [HttpGet("ui-spec/report-dashboard")]
    public ActionResult<ChartScreenSpec> GetReportDashboardSpecification() => Ok(GetReportDashboardSpec());

    // ----------------------------------------------------------------------
    // Navigation
    // ----------------------------------------------------------------------
    private static ModuleNavigationMap GetNavigationMap() => new()
    {
        ModuleId = "finance",
        ModuleName = "Finance",
        Icon = "account_balance",
        Items = new List<NavigationItem>
        {
            new() { Id = "invoices", Label = "Invoices", Icon = "receipt", Screen = "invoice-list", Route = "/finance/invoices" },
            new() { Id = "payments", Label = "Payments", Icon = "payments", Screen = "payment-list", Route = "/finance/payments" },
            new() { Id = "reports", Label = "Reports", Icon = "assessment", Screen = "report-dashboard", Route = "/finance/reports" }
        }
    };

    // ----------------------------------------------------------------------
    // Invoices
    // ----------------------------------------------------------------------
    private static ListScreenSpec GetInvoiceListSpec() => new()
    {
        Title = "Invoices",
        SearchPlaceholder = "Search invoices...",
        EnableSearch = true,
        EnableFilter = true,
        Columns = new List<ColumnDefinition>
        {
            new() { Name = "invoiceNumber", Label = "Invoice #", Type = "text", Sortable = true, Width = 120 },
            new() { Name = "customerName", Label = "Customer", Type = "text", Sortable = true, Width = 180 },
            new() { Name = "total", Label = "Total", Type = "currency", Sortable = true, Width = 120 },
            new() { Name = "balanceDue", Label = "Balance", Type = "currency", Sortable = true, Width = 120 },
            new() { Name = "status", Label = "Status", Type = "badge", Sortable = true, Width = 110 },
            new() { Name = "dueDate", Label = "Due", Type = "date", Sortable = true, Width = 120 }
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
                    new() { Id = "draft", Label = "Draft", Value = "Draft" },
                    new() { Id = "sent", Label = "Sent", Value = "Sent" },
                    new() { Id = "paid", Label = "Paid", Value = "Paid" },
                    new() { Id = "overdue", Label = "Overdue", Value = "Overdue" }
                }
            }
        },
        Actions = new List<ActionButton>
        {
            new() { Id = "add", Label = "New Invoice", Icon = "add", Action = "navigate", NavigateTo = "invoice-form" }
        },
        EmptyStateMessage = "No invoices yet. Create your first invoice."
    };

    private static DetailScreenSpec GetInvoiceDetailSpec() => new()
    {
        Title = "Invoice",
        Sections = new List<SectionDefinition>
        {
            new()
            {
                Id = "overview",
                Title = "Overview",
                Fields = new List<FieldDefinition>
                {
                    new() { Name = "invoiceNumber", Label = "Invoice #", Type = "text" },
                    new() { Name = "customerName", Label = "Customer", Type = "text" },
                    new() { Name = "status", Label = "Status", Type = "badge" },
                    new() { Name = "issueDate", Label = "Issued", Type = "date" },
                    new() { Name = "dueDate", Label = "Due", Type = "date" }
                }
            },
            new()
            {
                Id = "financials",
                Title = "Financials",
                Fields = new List<FieldDefinition>
                {
                    new() { Name = "subtotal", Label = "Subtotal", Type = "currency" },
                    new() { Name = "totalTax", Label = "Tax", Type = "currency" },
                    new() { Name = "total", Label = "Total", Type = "currency" },
                    new() { Name = "amountPaid", Label = "Paid", Type = "currency" },
                    new() { Name = "balanceDue", Label = "Balance Due", Type = "currency" }
                }
            }
        },
        Actions = new List<ActionButton>
        {
            new() { Id = "send", Label = "Send", Icon = "send", Action = "api-call", ApiEndpoint = "api/invoices/{id}/send" },
            new() { Id = "record-payment", Label = "Record Payment", Icon = "payments", Action = "navigate", NavigateTo = "payment-form" }
        }
    };

    private static FormScreenSpec GetInvoiceFormSpec() => new()
    {
        Title = "New Invoice",
        Sections = new List<FormSectionDefinition>
        {
            new()
            {
                Id = "customer",
                Title = "Customer",
                Fields = new List<FormFieldDefinition>
                {
                    new() { Name = "customerName", Label = "Customer Name", Type = "text", Required = true },
                    new() { Name = "customerEmail", Label = "Email", Type = "email" }
                }
            },
            new()
            {
                Id = "terms",
                Title = "Terms",
                Fields = new List<FormFieldDefinition>
                {
                    new() { Name = "dueDate", Label = "Due Date", Type = "date" },
                    new()
                    {
                        Name = "currency", Label = "Currency", Type = "select",
                        Options = new List<SelectOption>
                        {
                            new() { Value = "USD", Label = "USD" },
                            new() { Value = "EUR", Label = "EUR" },
                            new() { Value = "GBP", Label = "GBP" }
                        }
                    },
                    new() { Name = "notes", Label = "Notes", Type = "text" }
                }
            }
        },
        Actions = new List<ActionButton>
        {
            new() { Id = "save", Label = "Save", Icon = "save", Action = "api-call", ApiEndpoint = "api/invoices" }
        },
        Validation = new ValidationRules
        {
            ErrorMessages = new Dictionary<string, string>
            {
                { "customerName", "Customer name is required." }
            }
        }
    };

    // ----------------------------------------------------------------------
    // Payments
    // ----------------------------------------------------------------------
    private static ListScreenSpec GetPaymentListSpec() => new()
    {
        Title = "Payments",
        SearchPlaceholder = "Search payments...",
        Columns = new List<ColumnDefinition>
        {
            new() { Name = "invoiceNumber", Label = "Invoice #", Type = "text", Width = 120 },
            new() { Name = "amount", Label = "Amount", Type = "currency", Sortable = true, Width = 120 },
            new() { Name = "method", Label = "Method", Type = "text", Width = 130 },
            new() { Name = "status", Label = "Status", Type = "badge", Width = 110 },
            new() { Name = "paymentDate", Label = "Date", Type = "date", Sortable = true, Width = 120 }
        },
        Actions = new List<ActionButton>
        {
            new() { Id = "add", Label = "Record Payment", Icon = "add", Action = "navigate", NavigateTo = "payment-form" }
        },
        EmptyStateMessage = "No payments recorded yet."
    };

    private static FormScreenSpec GetPaymentFormSpec() => new()
    {
        Title = "Record Payment",
        Sections = new List<FormSectionDefinition>
        {
            new()
            {
                Id = "payment",
                Title = "Payment",
                Fields = new List<FormFieldDefinition>
                {
                    new() { Name = "amount", Label = "Amount", Type = "number", Required = true },
                    new()
                    {
                        Name = "method", Label = "Method", Type = "select",
                        Options = new List<SelectOption>
                        {
                            new() { Value = "CreditCard", Label = "Credit Card" },
                            new() { Value = "BankTransfer", Label = "Bank Transfer" },
                            new() { Value = "Cash", Label = "Cash" },
                            new() { Value = "Check", Label = "Check" }
                        }
                    },
                    new() { Name = "paymentDate", Label = "Date", Type = "date" },
                    new() { Name = "transactionReference", Label = "Reference", Type = "text" }
                }
            }
        },
        Actions = new List<ActionButton>
        {
            new() { Id = "save", Label = "Save", Icon = "save", Action = "api-call", ApiEndpoint = "api/payments" }
        }
    };

    // ----------------------------------------------------------------------
    // Reports
    // ----------------------------------------------------------------------
    private static ChartScreenSpec GetReportDashboardSpec() => new()
    {
        Title = "Finance Analytics",
        Charts = new List<ChartSpec>
        {
            new()
            {
                Id = "revenue-trend",
                Title = "Revenue (Last 6 Months)",
                ChartType = "bar",
                Series = new List<ChartSeries>
                {
                    new() { Name = "Revenue", Color = "#4caf50" }
                }
            },
            new()
            {
                Id = "status-breakdown",
                Title = "Invoices by Status",
                ChartType = "donut",
                Series = new List<ChartSeries>
                {
                    new() { Name = "Invoices" }
                }
            }
        },
        EmptyStateMessage = "No analytics available yet."
    };
}
