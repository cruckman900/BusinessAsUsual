using Platform.Web.Components;
using MudBlazor.Services;
using Platform.Application;
using Platform.Infrastructure;
using Platform.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor
builder.Services.AddMudServices();

// Add Platform services
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<SmartDefaultsService>();

// Add Platform layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add HttpClient for Platform API
builder.Services.AddHttpClient("PlatformApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:7400");
});

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
