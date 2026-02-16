using Amazon.CloudWatch;
using BusinessAsUsual.API.Common;
using BusinessAsUsual.Application.Common;
using BusinessAsUsual.Application.Database;
using BusinessAsUsual.Application.Services.Provisioning;
using BusinessAsUsual.Infrastructure;
using BusinessAsUsual.Infrastructure.Database;
using BusinessAsUsual.Infrastructure.Extensions;
using BusinessAsUsual.Infrastructure.Monitoring;
using BusinessAsUsual.Infrastructure.Provisioning;
using DotNetEnv;
using Microsoft.Data.SqlClient;

namespace BusinessAsUsual.API
{
    /// <summary>
    /// Entry point for BusinessAsUsual.API
    /// </summary>
#pragma warning disable S1118 // Utility classes should not have public constructors
    public class Program
#pragma warning restore S1118
    {
        /// <summary>
        /// Main entry point for the BusinessAsUsual.API application.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            // Load environment variables from .env
            await ConfigLoader.LoadEnvironmentVariables();
            Env.Load();

            var connString = ConfigLoader.Get("AWS_SQL_CONNECTION_STRING");

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            // Optional: register shared services
            //TODO: builder.Services.AddBusinessAsUsualServices();

            builder.Services.AddSingleton<IAppEnvironment, AppEnvironment>();
            builder.Services.AddScoped<IProvisioningService, ProvisioningService>();
            builder.Services.AddScoped<IProvisioningDb, ProvisioningDb>();

            builder.Services.AddPlatformMetrics();

            builder.Services.AddAWSService<IAmazonCloudWatch>();
            builder.Services.AddSingleton<IMetricPublisher, CloudWatchMetricPublisher>();
            builder.Services.AddSingleton<RequestMetricsMiddleware>();

            // Validate connection string
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

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAdmin",
                    policy =>
                    {
                        policy.WithOrigins(
                            "https://localhost:7229",               // local web
                            "https://localhost:7238",               // local Admin
                            "https://businessasusual.work",         // live web
                            "https://www.businessasusual.work",     // live web (www)
                            "https://admin.businessasusual.work"    // live admin
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    });
            });

            var app = builder.Build();

            app.UseMiddleware<RequestMetricsMiddleware>();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseCors("AllowAdmin");

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
