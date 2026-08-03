using FluentAssertions;
using Inventory.Application.Services;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Moq;

namespace Inventory.Tests.Services;

public class SupplierServiceTests
{
    private readonly Mock<ISupplierRepository> _supplierRepositoryMock;
    private readonly SupplierService _service;

    public SupplierServiceTests()
    {
        _supplierRepositoryMock = new Mock<ISupplierRepository>();
        _service = new SupplierService(_supplierRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllSuppliersAsync_ReturnsAllSuppliers()
    {
        // Arrange
        var suppliers = new List<Supplier>
        {
            new() { Id = Guid.NewGuid(), Name = "Supplier A", Code = "SUP-001", IsActive = true },
            new() { Id = Guid.NewGuid(), Name = "Supplier B", Code = "SUP-002", IsActive = true }
        };
        _supplierRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(suppliers);

        // Act
        var result = await _service.GetAllSuppliersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }
}
