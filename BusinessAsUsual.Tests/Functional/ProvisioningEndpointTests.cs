using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;

namespace BusinessAsUsual.Tests.Functional
{
    /// <summary>
    /// Functional tests for the provisioning endpoint.
    /// Validates that the API correctly handles tenant provisioning requests
    /// and returns appropriate HTTP responses.
    /// </summary>
    public class ProvisioningEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes the test client using the application factory.
        /// </summary>
        /// <param name="factory">
        /// The web application factory used to create the test server and client.
        /// </param>
        public ProvisioningEndpointTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        /// <summary>
        /// Sends a POST request to the provisioning endpoint with a sample payload
        /// and asserts that the response status code indicates success.
        /// </summary>
        [Fact]
        public async Task PostProvisioning_ReturnsSuccess()
        {
            var payload = new
            {
                companyName = "TestCo",
                adminEmail = "admin@testco.com",
                billingPlan = "Standard",
                modules = new[] { "Billing", "Inventory" }
            };

            var response = await _client.PostAsJsonAsync("/api/provision", payload);

            // Assert that the response status code is 200 OK
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}