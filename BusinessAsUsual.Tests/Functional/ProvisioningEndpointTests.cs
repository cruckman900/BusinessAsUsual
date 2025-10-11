using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using BusinessAsUsual.Admin.Areas.Admin.Models;
using Xunit.Abstractions;

namespace BusinessAsUsual.Tests.Functional
{
    /// <summary>
    /// Functional tests for the provisioning endpoint.
    /// Validates that the API correctly handles tenant provisioning requests
    /// and returns appropriate HTTP responses.
    /// </summary>
    public class ProvisioningEndpointTests : IClassFixture<WebApplicationFactory<Admin.Program>>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes the test client using the application factory.
        /// </summary>
        /// <param name="factory">
        /// The web application factory used to create the test server and client.
        /// </param>
        /// <param name="output"></param>
        public ProvisioningEndpointTests(WebApplicationFactory<Admin.Program> factory, ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _output = output;
        }

        /// <summary>
        /// Sends a POST request to the provisioning endpoint with a sample payload
        /// and asserts that the response status code indicates success.
        /// </summary>
        [Fact]
        public async Task PostProvisioning_ReturnsSuccess()
        {
            var payload = new Company
            {
                Name = "TestCo",
                AdminEmail = "admin@testco.com",
                BillingPlan = "Standard",
                ModulesEnabled = "Billing,Inventory"
            };

            var response = await _client.PostAsJsonAsync("/Admin/ProvisionCompany", payload);
            var body = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"Status: {response.StatusCode}");
            _output.WriteLine($"body: {body}");

            // Assert that the response status code is 200 OK
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}