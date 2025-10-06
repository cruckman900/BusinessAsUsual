using BusinessAsUsual.Admin.Services;
using BusinessAsUsual.Tests.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace BusinessAsUsual.Tests.Unit
{
    /// <summary>
    /// Unit tests for the <see cref="CompanyProvisioner"/> class.
    /// Validates provisioning logic in isolation using mocked dependencies.
    /// </summary>
    public class CompanyProvisionerTests
    {
        /// <summary>
        /// Verifies that <see cref="CompanyProvisioner.CreateCompanyDatabaseAsync"/> returns true
        /// when all provisioning steps succeed using mocked configuration, environment, and metadata service.
        /// </summary>
        [Trait("Category", "Unit")]
        [Fact]
        public async Task CreateCompanyDatabaseAsync_ReturnsTrue_WhenProvisioningSucceeds()
        {
            // Setup mocked dependencies
            var config = new Mock<IConfiguration>();
            var env = new Mock<IHostEnvironment>();
            var metadata = new Mock<TenantMetadataService>(config.Object);
            var logger = new Mock<ILogger<CompanyProvisioner>>();

            // Simulate environment and config values
            env.Setup(e => e.ContentRootPath).Returns("/fake/path");
            config.Setup(c => c.GetConnectionString("MasterAdmin"))
                  .Returns("Server=.;Database=master;Trusted_Connection=True;");

            // Create provisioner with mocks
            var provisioner = new CompanyProvisioner(
                config.Object,
                env.Object,
                metadata.Object,
                logger.Object);

            // Execute provisioning logic
            var result = await provisioner.CreateCompanyDatabaseAsync("TestCo", "admin@testco.com");

            // Assert success
            Assert.True(result);

            // Optional cleanup (integration only)
            //TestDbCleaner.Cleanup("bau_testco");
        }
    }
}