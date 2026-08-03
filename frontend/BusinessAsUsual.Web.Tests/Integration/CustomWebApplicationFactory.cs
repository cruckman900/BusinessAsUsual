using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace BusinessAsUsual.Web.Tests.Integration;

/// <summary>
/// Custom test factory that configures the Web application for integration testing.
/// Uses a unique GUID in environment to ensure fresh in-memory databases per test run.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<BusinessAsUsual.Web.ProgramAssemblyMarker>
{
    private readonly string _uniqueId = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Use a Testing environment so seeding doesn't run, avoiding duplicate key errors
        builder.UseEnvironment("Testing");

        base.ConfigureWebHost(builder);
    }
}
