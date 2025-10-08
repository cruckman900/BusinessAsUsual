using BusinessAsUsual.Admin.Extensions;
using BusinessAsUsual.Admin.Services;
using BusinessAsUsual.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env
ConfigLoader.LoadEnvironmentVariables();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Register your services
builder.Services.AddBusinessAsUsualServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

await app.RunAsync();