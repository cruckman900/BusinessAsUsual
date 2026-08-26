using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Platform.Application.DTOs.Import;
using Platform.Application.Services;
using Platform.Domain.Entities;
using Platform.Infrastructure.Data;
using Platform.Infrastructure.Services;

namespace Platform.Tests.Services;

public class BatchImportServiceTests : IDisposable
{
    private readonly PlatformDbContext _dbContext;
    private readonly Mock<ILogger<BatchImportService>> _loggerMock;
    private readonly Mock<IDataTransformationService> _transformServiceMock;
    private readonly Mock<ISchemaIntrospectionService> _schemaServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly BatchImportService _sut;

    public BatchImportServiceTests()
    {
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new PlatformDbContext(options);
        _loggerMock = new Mock<ILogger<BatchImportService>>();
        _transformServiceMock = new Mock<IDataTransformationService>();
        _schemaServiceMock = new Mock<ISchemaIntrospectionService>();
        _configurationMock = new Mock<IConfiguration>();

        // Mock IConfiguration indexer for connection string
        var connectionStringSection = new Mock<IConfigurationSection>();
        connectionStringSection.Setup(x => x.Value).Returns("Server=(localdb)\\mssqllocaldb;Database=Test;");
        _configurationMock
            .Setup(x => x.GetSection("ConnectionStrings:PlatformDb"))
            .Returns(connectionStringSection.Object);

        _sut = new BatchImportService(
            _loggerMock.Object,
            _transformServiceMock.Object,
            _schemaServiceMock.Object,
            _configurationMock.Object,
            _dbContext);
    }

    [Fact]
    public async Task GetImportHistoryAsync_ShouldReturnHistoryForCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var history1 = new ImportHistory
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            TableName = "Table1",
            ImportedBy = Guid.NewGuid(),
            StartedAt = DateTime.UtcNow.AddHours(-2),
            Status = "Completed",
            TotalRows = 100,
            SuccessfulRows = 100,
            FailedRows = 0
        };

        var history2 = new ImportHistory
        {
            Id = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(), // Different company
            TableName = "Table2",
            ImportedBy = Guid.NewGuid(),
            StartedAt = DateTime.UtcNow.AddHours(-1),
            Status = "Completed",
            TotalRows = 50,
            SuccessfulRows = 50,
            FailedRows = 0
        };

        _dbContext.ImportHistories.AddRange(history1, history2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetImportHistoryAsync(companyId);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(history1.Id);
        result.First().CompanyId.Should().Be(companyId);
    }

    [Fact]
    public async Task GetImportHistoryDetailAsync_ShouldReturnDetailForExistingImport()
    {
        // Arrange
        var importId = Guid.NewGuid();
        var history = new ImportHistory
        {
            Id = importId,
            CompanyId = Guid.NewGuid(),
            TableName = "TestTable",
            ImportedBy = Guid.NewGuid(),
            StartedAt = DateTime.UtcNow,
            Status = "Completed",
            TotalRows = 10,
            SuccessfulRows = 10,
            FailedRows = 0,
            MappingConfiguration = "{\"mappings\": []}"
        };

        _dbContext.ImportHistories.Add(history);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetImportHistoryDetailAsync(importId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(importId);
        result.MappingConfiguration.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetImportHistoryDetailAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _sut.GetImportHistoryDetailAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RollbackImportAsync_ShouldMarkImportAsRolledBack()
    {
        // Arrange
        var importId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var history = new ImportHistory
        {
            Id = importId,
            CompanyId = Guid.NewGuid(),
            TableName = "TestTable",
            ImportedBy = Guid.NewGuid(),
            StartedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow,
            Status = "Completed",
            TotalRows = 10,
            SuccessfulRows = 10,
            FailedRows = 0
        };

        _dbContext.ImportHistories.Add(history);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.RollbackImportAsync(importId, userId);

        // Assert
        result.Should().BeTrue();

        var updatedHistory = await _dbContext.ImportHistories.FindAsync(importId);
        updatedHistory.Should().NotBeNull();
        updatedHistory!.RolledBackAt.Should().NotBeNull();
        updatedHistory.RolledBackBy.Should().Be(userId);
    }

    [Fact]
    public async Task RollbackImportAsync_WithNonExistentImport_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var result = await _sut.RollbackImportAsync(nonExistentId, userId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetImportHistoryAsync_WithNoHistory_ShouldReturnEmptyList()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        // Act
        var result = await _sut.GetImportHistoryAsync(companyId);

        // Assert
        result.Should().BeEmpty();
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}
