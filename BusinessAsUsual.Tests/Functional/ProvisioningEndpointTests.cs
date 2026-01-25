using BusinessAsUsual.Application.Common;
using BusinessAsUsual.Application.Contracts;
using BusinessAsUsual.Application.Database;
using BusinessAsUsual.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace BusinessAsUsual.Tests.Functional
{
    /// <summary>
    /// Provides a custom web application factory for integration testing the BusinessAsUsual API, allowing
    /// configuration of test-specific services and environment settings.
    /// </summary>
    /// <remarks>Use this class to create test server instances with mocked dependencies and environment
    /// variables tailored for integration tests. The factory replaces certain services, such as database and
    /// environment providers, with test doubles to isolate tests from external systems. Inherit from this class when
    /// writing integration tests that require a controlled application host.</remarks>
    public class ApiFactory : WebApplicationFactory<BusinessAsUsual.API.Program>
    {
        /// <summary>
        /// Configures the web host for test execution by customizing service registrations and environment variables.
        /// </summary>
        /// <remarks>This method sets the AWS SQL connection string environment variable and replaces
        /// certain service registrations with mock implementations to facilitate isolated testing. It is intended for
        /// use in test scenarios where dependencies such as database access and environment information should be faked
        /// or controlled.</remarks>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> instance used to configure the web host for the test environment.</param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Set environment variable BEFORE the host builds
            Environment.SetEnvironmentVariable(
                "AWS_SQL_CONNECTION_STRING",
                "Server=localhost;Database=TestDb;User Id=test;Password=test;TrustServerCertificate=True;"
            );

            builder.ConfigureServices(services =>
            {
                // Remove the real ProvisioningDb registration
                var dbDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IProvisioningDb));

                if (dbDescriptor != null)
                    services.Remove(dbDescriptor);

                // Add a fake ProvisioningDb
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
                
                // Remove real IAppEnvironment if present
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAppEnvironment));

                if (descriptor != null)
                    services.Remove(descriptor);

                // Add fake environment
                var mockEnv = new Mock<IAppEnvironment>();
                mockEnv.Setup(e => e.ContentRootPath)
                       .Returns(Directory.GetCurrentDirectory());
                mockEnv.Setup(e => e.EnvironmentName)
                       .Returns("Development");

                services.AddSingleton<IAppEnvironment>(mockEnv.Object);
            });
        }
    }

    /// <summary>
    /// Provides integration tests for the provisioning API endpoints, verifying company provisioning scenarios using a
    /// test HTTP client.
    /// </summary>
    /// <remarks>This test class uses the xUnit test framework and is configured with an <see
    /// cref="ApiFactory"/> fixture to initialize the test server and HTTP client. Output from test executions is
    /// written to the provided <see cref="ITestOutputHelper"/> instance for diagnostic purposes. Each test method
    /// exercises a specific API endpoint and asserts expected results, helping ensure the reliability of the
    /// provisioning functionality.</remarks>
    public class ProvisioningEndpointTests : IClassFixture<ApiFactory>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes a new instance of the ProvisioningEndpointTests class using the specified API factory and test
        /// output helper.
        /// </summary>
        /// <param name="factory">The factory used to create an HTTP client for API integration testing.</param>
        /// <param name="output">The output helper used to capture and display test output during execution.</param>
        public ProvisioningEndpointTests(ApiFactory factory, ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _output = output;
        }

        /// <summary>
        /// Verifies that provisioning a company via the API returns a successful payload when provided with valid
        /// request data.
        /// </summary>
        /// <remarks>This test ensures that the API endpoint '/api/provisioning/provision-company'
        /// responds with a success result when a company is provisioned using valid parameters. The test asserts that
        /// the response status is successful and the returned payload indicates success.</remarks>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        [Fact]
        public async Task ProvisionCompany_ReturnsSuccessPayload()
        {
            var request = new ProvisioningRequest
            {
                CompanyName = "TestCo",
                AdminEmail = "admin@testco.com",
                BillingPlan = "Standard",
                Modules = new[] { "Billing", "Inventory" }
            };

            var response = await _client.PostAsJsonAsync(
                "/api/provisioning/provision-company",
                request);

            var body = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"Status: {response.StatusCode}");
            _output.WriteLine($"Body: {body}");

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ProvisioningResult>();

            Assert.NotNull(result);
            Assert.True(result.Success);
        }
    }
}