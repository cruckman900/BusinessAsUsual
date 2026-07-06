using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using HR.Domain.Repositories;
using Moq;

namespace HR.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="EmployeeService"/> using a mocked repository.
/// </summary>
public class EmployeeServiceTests
{
    private static Employee SampleEmployee(string id = "E1") => new()
    {
        Id = id,
        FirstName = "Ada",
        LastName = "Lovelace",
        Email = "ada@example.com",
        JobTitle = "Engineer",
        HireDate = DateTime.UtcNow.AddYears(-2)
    };

    private static CreateEmployeeRequest NewRequest() => new()
    {
        FirstName = "Grace",
        LastName = "Hopper",
        Email = "grace@example.com",
        JobTitle = "Admiral",
        EmploymentType = "FullTime",
        Status = "Active",
        HireDate = DateTime.UtcNow
    };

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllEmployeesAsync_MapsRepositoryResults()
    {
        var repo = new Mock<IEmployeeRepository>();
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { SampleEmployee("E1"), SampleEmployee("E2") });
        var service = new EmployeeService(repo.Object);

        var result = (await service.GetAllEmployeesAsync()).ToList();

        Assert.Equal(2, result.Count);
        repo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetEmployeeByIdAsync_ReturnsNull_WhenMissing()
    {
        var repo = new Mock<IEmployeeRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Employee?)null);
        var service = new EmployeeService(repo.Object);

        Assert.Null(await service.GetEmployeeByIdAsync("missing"));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetEmployeeByIdAsync_ReturnsDto_WhenFound()
    {
        var repo = new Mock<IEmployeeRepository>();
        repo.Setup(r => r.GetByIdAsync("E1")).ReturnsAsync(SampleEmployee("E1"));
        var service = new EmployeeService(repo.Object);

        var dto = await service.GetEmployeeByIdAsync("E1");

        Assert.NotNull(dto);
        Assert.Equal("ada@example.com", dto!.Email);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task CreateEmployeeAsync_PersistsAndReturnsDto()
    {
        var repo = new Mock<IEmployeeRepository>();
        repo.Setup(r => r.CreateAsync(It.IsAny<Employee>()))
            .ReturnsAsync((Employee e) => e);
        var service = new EmployeeService(repo.Object);

        var dto = await service.CreateEmployeeAsync(NewRequest());

        Assert.Equal("Grace", dto.FirstName);
        Assert.Equal("Active", dto.Status);
        repo.Verify(r => r.CreateAsync(It.IsAny<Employee>()), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task CreateEmployeeAsync_DefaultsInvalidEnums()
    {
        var repo = new Mock<IEmployeeRepository>();
        repo.Setup(r => r.CreateAsync(It.IsAny<Employee>())).ReturnsAsync((Employee e) => e);
        var service = new EmployeeService(repo.Object);
        var request = NewRequest();
        request.Status = "NotARealStatus";
        request.EmploymentType = "NotARealType";

        var dto = await service.CreateEmployeeAsync(request);

        Assert.Equal(nameof(EmploymentStatus.Active), dto.Status);
        Assert.Equal(nameof(EmploymentType.FullTime), dto.EmploymentType);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateEmployeeAsync_Throws_WhenMissing()
    {
        var repo = new Mock<IEmployeeRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((Employee?)null);
        var service = new EmployeeService(repo.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.UpdateEmployeeAsync("missing", new UpdateEmployeeRequest
            {
                FirstName = "X",
                LastName = "Y",
                Email = "x@y.com",
                EmploymentType = "FullTime",
                Status = "Active"
            }));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateEmployeeAsync_UpdatesFields_WhenFound()
    {
        var existing = SampleEmployee("E1");
        var repo = new Mock<IEmployeeRepository>();
        repo.Setup(r => r.GetByIdAsync("E1")).ReturnsAsync(existing);
        repo.Setup(r => r.UpdateAsync(It.IsAny<Employee>())).ReturnsAsync((Employee e) => e);
        var service = new EmployeeService(repo.Object);

        var dto = await service.UpdateEmployeeAsync("E1", new UpdateEmployeeRequest
        {
            FirstName = "Adele",
            LastName = "Lovelace",
            Email = "adele@example.com",
            JobTitle = "Lead Engineer",
            EmploymentType = "FullTime",
            Status = "Active"
        });

        Assert.Equal("Adele", dto.FirstName);
        Assert.Equal("adele@example.com", dto.Email);
        repo.Verify(r => r.UpdateAsync(It.IsAny<Employee>()), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteEmployeeAsync_DelegatesToRepository()
    {
        var repo = new Mock<IEmployeeRepository>();
        repo.Setup(r => r.DeleteAsync("E1")).Returns(Task.CompletedTask);
        var service = new EmployeeService(repo.Object);

        await service.DeleteEmployeeAsync("E1");

        repo.Verify(r => r.DeleteAsync("E1"), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task SearchEmployeesAsync_MapsResults()
    {
        var repo = new Mock<IEmployeeRepository>();
        repo.Setup(r => r.SearchAsync("ada")).ReturnsAsync(new[] { SampleEmployee("E1") });
        var service = new EmployeeService(repo.Object);

        var results = (await service.SearchEmployeesAsync("ada")).ToList();

        Assert.Single(results);
    }
}
