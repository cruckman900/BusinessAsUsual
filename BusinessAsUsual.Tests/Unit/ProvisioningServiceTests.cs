using BusinessAsUsual.Application.Common;
using BusinessAsUsual.Application.Contracts;
using BusinessAsUsual.Application.Database;
using BusinessAsUsual.Domain.Entities;
using BusinessAsUsual.Infrastructure.Provisioning;
using Moq;

namespace BusinessAsUsual.Tests.Unit
{
    /// <summary>
    /// Contains unit tests for the ProvisioningService class, verifying tenant provisioning scenarios and expected
    /// outcomes.
    /// </summary>
    /// <remarks>This test class uses xUnit and Moq to validate the behavior of ProvisioningService methods
    /// under various conditions. Tests are organized to ensure that all provisioning steps succeed and that the service
    /// interacts correctly with its dependencies.</remarks>
    public class ProvisioningServiceTests
    {
        /// <summary>
        /// Verifies that ProvisionTenantAsync returns a successful result when all provisioning steps complete without
        /// errors.
        /// </summary>
        /// <remarks>This unit test ensures that the tenant provisioning workflow succeeds when all
        /// dependencies and steps execute as expected. It checks that the result indicates success and that required
        /// identifiers are present in the response.</remarks>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        [Trait("Category", "Unit")]
        [Fact]
        public async Task ProvisionTenantAsync_ReturnsSuccess_WhenAllStepsSucceed()
        {
            // Arrange
            var mockDb = new Mock<IProvisioningDb>();
            var mockEnv = new Mock<IAppEnvironment>();

            mockEnv.Setup(e => e.ContentRootPath)
                   .Returns(Directory.GetCurrentDirectory());

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

            var service = new ProvisioningService(
                mockDb.Object,
                mockEnv.Object
            );

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

            mockDb.Verify(d => d.EnsureMasterDatabaseExistsAsync(), Times.Once);
            mockDb.Verify(d => d.SaveCompanyInfoAsync(It.IsAny<Company>()), Times.Once);
            mockDb.Verify(d => d.CreateTenantDatabaseAsync(It.IsAny<string>()), Times.Once);
            mockDb.Verify(d => d.ApplyTenantSchemaAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}