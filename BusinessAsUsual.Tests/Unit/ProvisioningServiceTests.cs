using BusinessAsUsual.Admin.Areas.Admin.Models;
using BusinessAsUsual.Admin.Database;
using BusinessAsUsual.Admin.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace BusinessAsUsual.Tests.Unit
{
    /// <summary>
    /// Tests for the ProvisioningService class.
    /// </summary>
    public class ProvisioningServiceTests
    {
        /// <summary>
        /// Test if ProvisionTenantAsync returns true after attempting to provision company.
        /// </summary>
        /// <returns></returns>
        [Trait("Category", "Unit")]
        [Fact]
        public async Task ProvisionTenantAsync_ReturnsTrue_WhenProvisioningSucceeds()
        {
            // 🧪 Mock SignalR hub
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

            // 🧪 Mock config and environment
            var config = new Mock<IConfiguration>();
            var env = new Mock<IHostEnvironment>();
            var logger = new Mock<ILogger<ProvisioningService>>();

            env.Setup(e => e.ContentRootPath).Returns("/fake/path");
            config.Setup(c => c["ConnectionStrings:DefaultConnection"])
                  .Returns("Server=localhost;Database=BusinessAsUsual;User Id=test;Password=test;");

            // 🧪 Mock metadata service
            var metadata = new Mock<TenantMetadataService>();

            metadata.Setup(m => m.GetProvisioningLogScript())
                    .Returns("CREATE TABLE ProvisioningLog (Id INT PRIMARY KEY IDENTITY, TenantName NVARCHAR(100), Step NVARCHAR(50), Status NVARCHAR(50), Message NVARCHAR(MAX), Timestamp DATETIME);");

            metadata.Setup(m => m.GetCompanyRegistryScript())
                    .Returns("CREATE TABLE Companies (Id UNIQUEIDENTIFIER PRIMARY KEY, Name NVARCHAR(100), DbName NVARCHAR(100), AdminEmail NVARCHAR(100), BillingPlan NVARCHAR(50), ModulesEnabled NVARCHAR(MAX), IsDisabled BIT, CreatedAt DATETIME);");

            metadata.Setup(m => m.GetCreateScript(env.Object, "TestCo"))
                    .Returns("CREATE TABLE CompanySettings (CompanyId INT PRIMARY KEY IDENTITY, CompanyName NVARCHAR(100), AdminEmail NVARCHAR(100), BillingPlan NVARCHAR(50), CreatedAt DATETIME);");

            // 🧪 Mock ProvisioningDb
            var provDb = new Mock<ProvisioningDb>(config.Object);
            provDb.Setup(d => d.ApplySchemaAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            provDb.Setup(d => d.CreateDatabaseAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            provDb.Setup(d => d.SaveCompanyInfoAsync(It.IsAny<Company>())).Returns(Task.CompletedTask);

            // 🧪 Mock ProvisioningLogger
            var provLogger = new Mock<ProvisioningLogger>(hub.Object, config.Object);
            provLogger.Setup(l => l.LogStepAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(Task.CompletedTask);


            // 🧪 Create provisioner with mocks
            var provisioner = new ProvisioningService(
                env.Object,
                metadata.Object,
                logger.Object,
                provDb.Object,
                provLogger.Object);

            // 🧪 Execute provisioning logic
            var result = await provisioner.ProvisionTenantAsync("TestCo", "admin@testco.com", "standard", new[] { "Billing", "Inventory" });

            // ✅ Assert success
            Assert.True(result);
        }
    }
}