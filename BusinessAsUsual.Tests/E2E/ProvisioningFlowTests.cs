using BusinessAsUsual.Admin;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace BusinessAsUsual.Tests.E2E
{
    /// <summary>
    /// Provides integration tests for the company provisioning API to verify correct behavior and response payloads
    /// when provisioning a company.
    /// </summary>
    /// <remarks>This test class uses a web application factory to create an HTTP client for sending requests
    /// to the API endpoints in a test environment. Tests within this class validate that the provisioning flow returns
    /// the expected results when given valid input data.</remarks>
    public class ProvisioningFlowTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the ProvisioningFlowTests class using the specified web application factory.
        /// </summary>
        /// <param name="factory">The WebApplicationFactory used to create an HTTP client for integration testing. Cannot be null.</param>
        public ProvisioningFlowTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        /// <summary>
        /// Verifies that the company provisioning API returns a successful JSON payload when provided with valid input
        /// data.
        /// </summary>
        /// <remarks>This test ensures that a POST request to the '/api/provision-company' endpoint with
        /// valid company details results in a successful response containing the expected success message and content
        /// type.</remarks>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        [Fact]
        public async Task ProvisioningFlow_ReturnsSuccessPayload()
        {
            var payload = new
            {
                companyName = "NEXTRiff",
                adminEmail = "admin@nextriff.com",
                billingPlan = "Standard",
                modules = new[] { "Billing", "Inventory" }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/provisioning/provision-company", content);
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