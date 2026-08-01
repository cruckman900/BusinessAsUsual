using Finance.Application.Services;
using Finance.Web.Components;
using MudBlazor.Services;
using ApexCharts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor
builder.Services.AddMudServices();

// Add ApexCharts
builder.Services.AddApexCharts();

// Configure HttpClient for Finance API calls
var financeApiUrl = builder.Configuration["FinanceApi:Url"] ?? "http://localhost:5007";
builder.Services.AddHttpClient("FinanceApi", client =>
{
    client.BaseAddress = new Uri(financeApiUrl);
});

// Configure HttpClient for HR API calls (for employee lookup)
var hrApiUrl = builder.Configuration["HrService:Url"] ?? "http://localhost:5041";
builder.Services.AddHttpClient("HrApi", client =>
{
    client.BaseAddress = new Uri(hrApiUrl);
});

// Register Finance services (mock implementations for now, sharing an in-memory store)
builder.Services.AddSingleton<FinanceDataStore>();
builder.Services.AddScoped<IInvoiceService, MockInvoiceService>();
builder.Services.AddScoped<IPaymentService, MockPaymentService>();
builder.Services.AddScoped<IFinanceReportService, MockFinanceReportService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
