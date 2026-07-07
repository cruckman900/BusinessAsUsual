using HR.Application.Services;
using HR.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HR.Tests.Functional;

/// <summary>
/// Boots the HR API in-process for functional tests. Forces the EF Core in-memory
/// provider and stubs out Module Registry registration so no external services are required.
/// </summary>
public class HrApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Use Production and force the in-memory provider so startup takes the
        // EnsureCreated() path instead of applying SQL Server migrations.
        builder.UseEnvironment("Production");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["UseInMemoryDatabase"] = "true"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Program.cs registers the DbContext with SQL Server based on config read before
            // this runs, so strip every EF Core registration and re-add the in-memory provider.
            // In EF Core 9 the provider is applied via IDbContextOptionsConfiguration<T>, which
            // must also be removed or both providers end up registered.
            RemoveAll(services, typeof(DbContextOptions<HRDbContext>));
            RemoveAll(services, typeof(DbContextOptions));
            RemoveAll(services, typeof(IDbContextOptionsConfiguration<HRDbContext>));
            RemoveAll(services, typeof(HRDbContext));

            services.AddDbContext<HRDbContext>(options =>
                options.UseInMemoryDatabase("HR-Tests"));

            // Replace the real registration service with a no-op stub.
            RemoveAll(services, typeof(IModuleRegistrationService));
            services.AddScoped<IModuleRegistrationService, NoOpModuleRegistrationService>();
        });
    }

    private static void RemoveAll(IServiceCollection services, Type serviceType)
    {
        var descriptors = services.Where(d => d.ServiceType == serviceType).ToList();
        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }
    }

    private sealed class NoOpModuleRegistrationService : IModuleRegistrationService
    {
        public Task RegisterWithModuleRegistryAsync() => Task.CompletedTask;
    }
}
