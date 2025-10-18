using BusinessAsUsual.Admin;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
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
            try
            {
                var formData = new Dictionary<string, string>
                {
                    { "Name", "NEXTRiff" },
                    { "AdminEmail", "admin@nextriff.com" },
                    { "BillingPlan", "Standard" },
                    { "ModulesEnabled", "Billing,Inventory" }
                };

                var content = new FormUrlEncodedContent(formData);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var request = new HttpRequestMessage(HttpMethod.Post, "/admin/company/provision")
                {
                    Content = content
                };
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Content-Type: {response.Content.Headers.ContentType}");
                Console.WriteLine($"Body: {responseBody}");

                if (response.Content.Headers.ContentType?.MediaType != "application/json")
                {
                    throw new InvalidOperationException("Expected JSON response but got HTML.");
                }

                var payload = JsonDocument.Parse(responseBody);

                var message = payload.RootElement.GetProperty("message").GetString();

                Assert.Equal("Provisioning successful", message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test failed with exception: {ex}");
                throw;
            }
        }
    }
}