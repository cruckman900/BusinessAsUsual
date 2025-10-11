using BusinessAsUsual.Admin.Extensions;
using BusinessAsUsual.Admin.Services;
using BusinessAsUsual.Infrastructure;
using DotNetEnv;

namespace BusinessAsUsual.Admin
{
    /// <summary>
    /// BusinessAsUsual.Admin project Program.cs
    /// </summary>
#pragma warning disable S1118 // Utility classes should not have public constructors
    public class Program
#pragma warning restore S1118 // Utility classes should not have public constructors
    {
        /// <summary>
        /// Entry point of BusinessAsUsual.Admin
        /// </summary>
        /// <param name="args"></param>
        public static async Task Main(string[] args)
        {
            // Load environment variables from .env
            ConfigLoader.LoadEnvironmentVariables();
            Env.Load("/app/.env");

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
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
            app.UseDeveloperExceptionPage();

            app.MapHub<ProvisioningHub>("/provisioningHub");

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );

#pragma warning disable ASP0014
            app.UseEndpoints(static endpoints =>
            {
                endpoints.MapHub<ProvisioningHub>("/provisioningHub");
            });
#pragma warning restore ASP0014

            app.MapStaticAssets();
            app.MapRazorPages().WithStaticAssets();

            await app.RunAsync();
        }
    }
}