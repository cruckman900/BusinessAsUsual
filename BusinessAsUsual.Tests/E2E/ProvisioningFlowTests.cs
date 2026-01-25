using BusinessAsUsual.Application.Common;
using BusinessAsUsual.Application.Contracts;
using BusinessAsUsual.Application.Database;
using BusinessAsUsual.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net.Http.Json;
using System.Text.Json;

namespace BusinessAsUsual.Tests.E2E
{
    /// <summary>
    /// Provides a custom WebApplicationFactory for integration testing the BusinessAsUsual.API application. Configures
    /// the test host with mock implementations of environment and database services to enable isolated and reliable API
    /// tests.
    /// </summary>
    /// <remarks>This factory replaces IAppEnvironment and IProvisioningDb with mock implementations to
    /// prevent dependencies on external resources and to ensure consistent test behavior. Use this class to create test
    /// servers and clients for end-to-end API tests without requiring a real database or production environment
    /// configuration.</remarks>
    public class ApiTestFactory : WebApplicationFactory<BusinessAsUsual.API.Program>
    {
        /// <summary>
        /// Configures the web host for integration testing by customizing service registrations and environment
        /// settings.
        /// </summary>
        /// <remarks>This method replaces certain services with mock implementations to facilitate safe
        /// and isolated integration tests. It sets a test connection string and substitutes dependencies such as <see
        /// cref="IAppEnvironment"/> and <see cref="IProvisioningDb"/> with test-friendly versions. This ensures that
        /// tests do not interact with real infrastructure or databases.</remarks>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> instance used to configure the application's web host and services.</param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // 1. Fake connection string so ProvisioningDb doesn't explode
            Environment.SetEnvironmentVariable(
                "AWS_SQL_CONNECTION_STRING",
                "Server=localhost;Database=TestDb;User Id=test;Password=test;TrustServerCertificate=True;"
            );

            builder.ConfigureServices(services =>
            {
                //
                // 2. Replace IAppEnvironment
                //
                var envDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAppEnvironment));

                if (envDescriptor != null)
                    services.Remove(envDescriptor);

                var mockEnv = new Mock<IAppEnvironment>();
                mockEnv.Setup(e => e.ContentRootPath)
                       .Returns(Directory.GetCurrentDirectory());
                mockEnv.Setup(e => e.EnvironmentName)
                       .Returns("Development");

                services.AddSingleton<IAppEnvironment>(mockEnv.Object);

                //
                // 3. Replace IProvisioningDb with a harmless fake
                //
                var dbDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IProvisioningDb));

                if (dbDescriptor != null)
                    services.Remove(dbDescriptor);

                var mockDb = new Mock<IProvisioningDb>();

                mockDb.Setup(d => d.EnsureMasterDatabaseExistsAsync())
                      .Returns(Task.CompletedTask);

                mockDb.Setup(d => d.ApplyMasterSchemaAsync(It.IsAny<string>()))
                      .Returns(Task.CompletedTask);

                mockDb.Setup(d => d.SaveCompanyInfoAsync(It.IsAny<Company>()))
                      .Returns(Task.CompletedTask);

                mockDb.Setup(d => d.CreateTenantDatabaseAsync(It.IsAny<string>()))
                      .Returns(Task.CompletedTask);

                mockDb.Setup(d => d.ApplyTenantSchemaAsync(It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(Task.CompletedTask);

                services.AddSingleton<IProvisioningDb>(mockDb.Object);
            });
        }
    }

    /// <summary>
    /// Provides integration tests for the company provisioning API flow, verifying that provisioning requests return
    /// the expected success payload.
    /// </summary>
    /// <remarks>This test class uses the xUnit test framework and is configured with an instance of <see
    /// cref="ApiTestFactory"/> to create an HTTP client for API requests. The tests validate the end-to-end behavior of
    /// the provisioning endpoint, including response status, content type, and payload structure. Intended for use in
    /// automated test suites to ensure the reliability of the provisioning process.</remarks>
    public class ProvisioningFlowTests : IClassFixture<ApiTestFactory>
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the ProvisioningFlowTests class using the specified API test factory.
        /// </summary>
        /// <param name="factory">The factory used to create a test HTTP client for API integration tests. Cannot be null.</param>
        public ProvisioningFlowTests(ApiTestFactory factory)
        {
            _client = factory.CreateClient();
        }

        /// <summary>
        /// Verifies that the provisioning API returns a successful payload when a valid company provisioning request is
        /// submitted.
        /// </summary>
        /// <remarks>This test ensures that the API responds with a JSON payload indicating success and
        /// the expected message when provisioning a company with valid input data. The test also checks that the
        /// response content type is 'application/json'.</remarks>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        [Fact]
        public async Task ProvisioningFlow_ReturnsSuccessPayload()
        {
            var request = new ProvisioningRequest
            {
                CompanyName = "NEXTRiff",
                AdminEmail = "admin@nextriff.com",
                BillingPlan = "Standard",
                Modules = new[] { "Billing", "Inventory" }
            };

            var response = await _client.PostAsJsonAsync(
                "/api/provisioning/provision-company",
                request);

            var body = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Content-Type: {response.Content.Headers.ContentType}");
            Console.WriteLine($"Body: {body}");

            response.EnsureSuccessStatusCode();

            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

            var doc = JsonDocument.Parse(body);

            Assert.True(doc.RootElement.GetProperty("success").GetBoolean());
            Assert.Equal("Provisioning successful", doc.RootElement.GetProperty("message").GetString());
        }
    }
}