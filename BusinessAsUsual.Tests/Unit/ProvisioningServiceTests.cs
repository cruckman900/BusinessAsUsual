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
    /// Validates provisioning logic in isolation using mocked configuration, environment, and metadata service.
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
            // Setup mocked SignalR hub
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
            mockClientProxy
                .Setup(c => c.SendCoreAsync(
                    It.IsAny<string>(),
                    It.IsAny<object[]>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var hub = new Mock<IHubContext<ProvisioningHub>>();
            hub.Setup(h => h.Clients).Returns(mockClients.Object);

            // Setup mocked config and environment
            var config = new Mock<IConfiguration>();
            var env = new Mock<IHostEnvironment>();
            var logger = new Mock<ILogger<ProvisioningService>>();

            env.Setup(e => e.ContentRootPath).Returns("/fake/path");
            config.Setup(c => c["ConnectionStrings:DefaultConnection"])
                  .Returns("Server=bau-sql.ch64meuam1eh.us-east-2.rds.amazonaws.com,1433;Database=BusinessAsUsual;User Id=admin;Password=from-the-ashes-2025;Encrypt=True;TrustServerCertificate=True");

            // Setup mocked metadata service with in-memory SQL
            var metadata = new Mock<TenantMetadataService>(config.Object);
            metadata.Setup(m => m.GetCreateScript("TestCo"))
                    .Returns(@"
                        CREATE DATABASE [bau_testco];
                        USE [bau_testco];
                        CREATE TABLE ProvisioningLog (
                            Id INT PRIMARY KEY IDENTITY,
                            TenantName NVARCHAR(100),
                            Step NVARCHAR(50),
                            Status NVARCHAR(50),
                            Message NVARCHAR(MAX),
                            Timestamp DATETIME
                        );
                    ");

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