using BusinessAsUsual.Admin.Services;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Moq;

namespace BusinessAsUsual.Admin.Tests.Unit;

public class TenantMetadataServiceTests
{
    private TenantMetadataService NewService() => new();

    [Fact]
    public void GetCompanyRegistryScript_ReturnsValidSql()
    {
        // Arrange
        var service = NewService();

        // Act
        var script = service.GetCompanyRegistryScript();

        // Assert
        script.Should().NotBeNullOrEmpty();
        script.Should().Contain("CREATE TABLE Companies");
        script.Should().Contain("Id UNIQUEIDENTIFIER PRIMARY KEY");
        script.Should().Contain("Name NVARCHAR(100) NOT NULL");
    }

    [Fact]
    public void GetProvisioningLogScript_ReturnsValidSql()
    {
        // Arrange
        var service = NewService();

        // Act
        var script = service.GetProvisioningLogScript();

        // Assert
        script.Should().NotBeNullOrEmpty();
        script.Should().Contain("CREATE TABLE ProvisioningLog");
        script.Should().Contain("TenantName NVARCHAR(100)");
        script.Should().Contain("Status NVARCHAR(50)");
    }

    [Fact]
    public void GetCompanyRegistryScript_CreatesTableIfNotExists()
    {
        // Arrange
        var service = NewService();

        // Act
        var script = service.GetCompanyRegistryScript();

        // Assert
        script.Should().Contain("IF NOT EXISTS");
        script.Should().Contain("INFORMATION_SCHEMA.TABLES");
    }

    [Fact]
    public void GetProvisioningLogScript_CreatesTableIfNotExists()
    {
        // Arrange
        var service = NewService();

        // Act
        var script = service.GetProvisioningLogScript();

        // Assert
        script.Should().Contain("IF NOT EXISTS");
        script.Should().Contain("sys.tables");
    }

    [Fact]
    public void GetCompanyRegistryScript_IncludesDefaultValues()
    {
        // Arrange
        var service = NewService();

        // Act
        var script = service.GetCompanyRegistryScript();

        // Assert
        script.Should().Contain("DEFAULT 'Standard'"); // BillingPlan default
        script.Should().Contain("DEFAULT 0"); // IsDisabled default
        script.Should().Contain("DEFAULT GETUTCDATE()"); // CreatedAt default
    }

    [Fact]
    public void GetProvisioningLogScript_IncludesIdentityColumn()
    {
        // Arrange
        var service = NewService();

        // Act
        var script = service.GetProvisioningLogScript();

        // Assert
        script.Should().Contain("IDENTITY");
    }
}
