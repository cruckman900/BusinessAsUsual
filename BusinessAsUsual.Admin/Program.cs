using BusinessAsUsual.Admin.Extensions;
using BusinessAsUsual.Admin.Services;
using BusinessAsUsual.Infrastructure;
using DotNetEnv;
using Microsoft.Data.SqlClient;

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
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            // Register your services
            builder.Services.AddBusinessAsUsualServices();

            var connString = Environment.GetEnvironmentVariable("AWS_SQL_CONNECTION_STRING");

            if (string.IsNullOrWhiteSpace(connString))
            {
                Console.WriteLine("❌ AWS_SQL_CONNECTION_STRING is missing or empty.");
            }
            else
            {
                var retries = 10;
                while (retries > 0)
                {
                    try
                    {
                        using var conn = new SqlConnection(connString);
                        await conn.OpenAsync();
                        Console.WriteLine("✅ SQL Server is ready.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⏳ Waiting for SQL Server... {ex.Message}");
                        Thread.Sleep(2000);
                        retries--;
                    }
                }

                if (retries == 0)
                {
                    Console.WriteLine("❌ SQL Server did not become ready in time.");
                }
            }

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