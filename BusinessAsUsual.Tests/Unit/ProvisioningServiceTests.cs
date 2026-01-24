using BusinessAsUsual.Admin.Areas.Admin.Models;
using BusinessAsUsual.Admin.Database;
using BusinessAsUsual.Admin.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Moq;

namespace BusinessAsUsual.Tests.Unit
{
    /// <summary>
    /// Contains unit tests for the ProvisioningService, verifying tenant provisioning scenarios and expected outcomes.
    /// </summary>
    /// <remarks>This test class uses xUnit and Moq to validate the behavior of ProvisioningService methods
    /// under various conditions. Tests are categorized as unit tests and are intended to ensure that tenant
    /// provisioning succeeds when all required steps complete successfully.</remarks>
    public class ProvisioningServiceTests
    {
        /// <summary>
        /// Verifies that ProvisionTenantAsync returns a successful result when all provisioning steps complete without
        /// errors.
        /// </summary>
        /// <remarks>This test ensures that the tenant provisioning workflow completes successfully and
        /// that all required database operations are invoked exactly once. It validates that the result contains a
        /// non-null company ID and tenant database name, indicating successful provisioning.</remarks>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        [Trait("Category", "Unit")]
        [Fact]
        public async Task ProvisionTenantAsync_ReturnsSuccess_WhenAllStepsSucceed()
        {
            // Arrange
            Environment.SetEnvironmentVariable(
                "AWS_SQL_CONNECTION_STRING",
                "Server=localhost;Database=BusinessAsUsual;User Id=test;Password=test;"
            );

            var env = new Mock<IWebHostEnvironment>();
            var root = Directory.GetCurrentDirectory();
            env.Setup(e => e.ContentRootPath).Returns(root);

            // Mock IProvisioningDb
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

            // Create service
            var service = new ProvisioningService(mockDb.Object, env.Object);

            var request = new ProvisioningRequest
            {
                CompanyName = "TestCo",
                AdminEmail = "admin@testco.com",
                BillingPlan = "standard",
                Modules = new[] { "Billing", "Inventory" }
            };

            // Act
            var result = await service.ProvisionTenantAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.CompanyId);
            Assert.NotNull(result.TenantDbName);

            // Verify calls
            mockDb.Verify(d => d.EnsureMasterDatabaseExistsAsync(), Times.Once);
            mockDb.Verify(d => d.SaveCompanyInfoAsync(It.IsAny<Company>()), Times.Once);
            mockDb.Verify(d => d.CreateTenantDatabaseAsync(It.IsAny<string>()), Times.Once);
            mockDb.Verify(d => d.ApplyTenantSchemaAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}