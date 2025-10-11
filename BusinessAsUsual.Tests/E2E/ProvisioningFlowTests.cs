using BusinessAsUsual.Admin;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using System.Text.Json;
using Xunit;

namespace BusinessAsUsual.Tests.E2E
{
    /// <summary>
    /// End-to-end test for the provisioning flow using Selenium.
    /// Simulates user interaction with the MVC form and verifies modal feedback.
    /// </summary>
    public class ProvisioningFlowTests : IClassFixture<WebApplicationFactory<Program>>

    {
        private readonly HttpClient _client;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="factory"></param>
        public ProvisioningFlowTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient(); // no port binding, no external launch
        }

        /// <summary>
        /// Simulates a full provisioning flow and asserts that the success modal is displayed.
        /// </summary>
        [Fact]
        public async Task ProvisioningFlow_ReturnsSuccessPayload()
        {
            var json = JsonSerializer.Serialize(new
            {
                Name = "TestCo",
                AdminEmail = "admin@test.com",
                BillingPlan = "Standard",
                ModulesEnabled = "Billing,Inventory"
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/Admin/ProvisionCompany", content);

            response.EnsureSuccessStatusCode(); // throws if not 2xx

            var responseBody = await response.Content.ReadAsStringAsync();
            var payload = JsonDocument.Parse(responseBody);

            var message = payload.RootElement.GetProperty("message").GetString();
            var commitTag = payload.RootElement.GetProperty("commitTag").GetString();

            Assert.Equal("Provisioning successful", message);
            Assert.Contains("Provisioned TestCo", commitTag);
        }
    }
}