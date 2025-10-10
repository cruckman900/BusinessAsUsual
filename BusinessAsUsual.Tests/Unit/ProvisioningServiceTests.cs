using BusinessAsUsual.Admin.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace BusinessAsUsual.Tests.Unit
{
    /// <summary>
    /// Unit tests for the <see cref="ProvisioningService"/> class.
    /// Validates provisioning logic in isolation using mocked dependencies.
    /// </summary>
    public class ProvisioningServiceTests
    {
        /// <summary>
        /// Verifies that <see cref="ProvisioningService.ProvisionTenantAsync"/> returns true
        /// when all provisioning steps succeed using mocked configuration, environment, and metadata service.
        /// </summary>
        [Trait("Category", "Unit")]
        [Fact]
        public async Task ProvisionTenantAsync_ReturnsTrue_WhenProvisioningSucceeds()
        {
            // Setup mocked dependencies
            var hub = new Mock<IHubContext<ProvisioningHub>>();
            var config = new Mock<IConfiguration>();
            var env = new Mock<IHostEnvironment>();
            var metadata = new Mock<TenantMetadataService>(config.Object);
            var logger = new Mock<ILogger<ProvisioningService>>();

            // Simulate environment and config values
            env.Setup(e => e.ContentRootPath).Returns("/fake/path");
            config.Setup(c => c.GetConnectionString("MasterAdmin"))
                  .Returns("Server=.;Database=master;Trusted_Connection=True;");

            // Create provisioner with mocks
            var provisioner = new ProvisioningService(
                hub.Object,
                config.Object,
                env.Object,
                metadata.Object,
                logger.Object);

            // Execute provisioning logic
            var result = await provisioner.ProvisionTenantAsync("TestCo", "admin@testco.com", "standard", new[] { "Billing", "Inventory" });

            // Assert success
            Assert.True(result);

            // Optional cleanup (integration only)
            //TestDbCleaner.Cleanup("bau_testco");
        }
    }
}