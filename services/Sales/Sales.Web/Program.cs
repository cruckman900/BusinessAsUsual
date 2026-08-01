using Sales.Web.Components;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container - using Blazor Server to match shell
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add MudBlazor services
builder.Services.AddMudServices();

// Named HTTP client for the Sales API
var salesApiUrl = builder.Configuration["SalesApi:Url"] ?? "http://localhost:5143";
builder.Services.AddHttpClient("SalesApi", client =>
{
    client.BaseAddress = new Uri(salesApiUrl);
});

// CORS configuration for iframe embedding
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebShell", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5000",  // Main web shell
            "https://localhost:7000"  // Main web shell HTTPS
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowWebShell");

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapBlazorHub();

app.Run();
