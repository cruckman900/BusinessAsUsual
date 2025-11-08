using BusinessAsUsual.Application.Services.Provisioning;
using BusinessAsUsual.Infrastructure;
using BusinessAsUsual.Infrastructure.Data;
using BusinessAsUsual.Infrastructure.Persistence;
using DotNetEnv;
using Microsoft.Data.SqlClient;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.EntityFrameworkCore;

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

            builder.Services.AddDbContext<MasterDbContext>(options =>
                options.UseMySql(connString,
                new MySqlServerVersion(ServerVersion.AutoDetect(
                    connString
                ))));
            builder.Services.AddDbContext<BusinessDbContext>(options =>
                options.UseMySql(connString,
                new MySqlServerVersion(ServerVersion.AutoDetect(
                    connString
                ))));

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            // Optional: register shared services
            //TODO: builder.Services.AddBusinessAsUsualServices();

            builder.Services.AddScoped<CompanyProvisioningService>();
            builder.Services.AddScoped<TenantProvisioningService>();

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

            var app = builder.Build();

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

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
