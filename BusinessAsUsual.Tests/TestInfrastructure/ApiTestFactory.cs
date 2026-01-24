using Microsoft.AspNetCore.Hosting;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Provides a custom WebApplicationFactory for integration testing the BusinessAsUsual API application.
/// </summary>
/// <remarks>This factory configures the test web host to use an in-memory configuration with a predefined
/// SQL connection string. Use this class to create test server instances for end-to-end or integration tests
/// targeting the BusinessAsUsual API. The configuration ensures that tests run against a local test database rather
/// than production resources.</remarks>
public class ApiTestFactory : WebApplicationFactory<BusinessAsUsual.API.Program>
{
    /// <summary>
    /// Configures the application's web host by customizing the configuration sources for the web host builder.
    /// </summary>
    /// <remarks>Override this method to add or modify configuration sources for the web host. This
    /// method is typically called during application startup to set up environment-specific or custom configuration
    /// values.</remarks>
    /// <param name="builder">The <see cref="IWebHostBuilder"/> instance used to construct and configure the application's web host.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var dict = new Dictionary<string, string?>
            {
                ["AWS_SQL_CONNECTION_STRING"] =
                    "Server=localhost;Database=BusinessAsUsual;User Id=test;Password=test;"
            };

            config.AddInMemoryCollection(dict);
        });
    }
}
