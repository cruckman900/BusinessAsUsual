using CRM.Application.Services;
using CRM.Application.Interfaces;
using CRM.Web.Components;
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

// Configure HttpClient for API calls
var crmApiUrl = builder.Configuration["CrmApi:Url"] ?? "http://localhost:5004";
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(crmApiUrl) 
});

// Register CRM services
builder.Services.AddScoped<ILeadService, MockLeadService>();
builder.Services.AddScoped<IOpportunityService, MockOpportunityService>();
builder.Services.AddScoped<ICustomerService, MockCustomerService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IActivityService, MockActivityService>();

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
