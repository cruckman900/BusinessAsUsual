using FluentAssertions;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using HR.Domain.Repositories;
using Moq;

namespace HR.Application.Tests.Services;

public class DepartmentServiceTests
{
    private readonly Mock<IDepartmentRepository> _mockDepartmentRepository;
    private readonly Mock<IEmployeeRepository> _mockEmployeeRepository;
    private readonly DepartmentService _service;

    public DepartmentServiceTests()
    {
        _mockDepartmentRepository = new Mock<IDepartmentRepository>();
        _mockEmployeeRepository = new Mock<IEmployeeRepository>();
        _service = new DepartmentService(_mockDepartmentRepository.Object, _mockEmployeeRepository.Object);
    }

    [Fact]
    public async Task GetAllDepartmentsAsync_Should_Return_All_Departments()
    {
        // Arrange
        var departments = new List<Department>
        {
            new Department
            {
                Id = "1",
                Name = "Engineering",
                Description = "Software Engineering",
                Code = "ENG",
                IsActive = true
            },
            new Department
            {
                Id = "2",
                Name = "Sales",
                Description = "Sales Department",
                Code = "SAL",
                IsActive = true
            }
        };

        _mockDepartmentRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(departments);

        // Act
        var result = await _service.GetAllDepartmentsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(d => d.Name == "Engineering");
        result.Should().Contain(d => d.Name == "Sales");
    }

    [Fact]
    public async Task GetDepartmentByIdAsync_Should_Return_Department_When_Found()
    {
        // Arrange
        var department = new Department
        {
            Id = "123",
            Name = "Engineering",
            Description = "Software Engineering Department",
            Code = "ENG",
            Location = "Building A",
            IsActive = true
        };

        _mockDepartmentRepository.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(department);

        // Act
        var result = await _service.GetDepartmentByIdAsync("123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("123");
        result.Name.Should().Be("Engineering");
        result.Code.Should().Be("ENG");
    }

    [Fact]
    public async Task GetDepartmentByIdAsync_Should_Return_Null_When_Not_Found()
    {
        // Arrange
        _mockDepartmentRepository.Setup(r => r.GetByIdAsync("999")).ReturnsAsync((Department?)null);

        // Act
        var result = await _service.GetDepartmentByIdAsync("999");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateDepartmentAsync_Should_Create_Department_With_Valid_Data()
    {
        // Arrange
        var request = new CreateDepartmentRequest
        {
            Name = "HR",
            Description = "Human Resources",
            Code = "HR",
            Location = "Building B",
            CostCenter = "CC-HR-001",
            IsActive = true
        };

        _mockDepartmentRepository.Setup(r => r.CreateAsync(It.IsAny<Department>()))
            .ReturnsAsync((Department d) => d);

        // Act
        var result = await _service.CreateDepartmentAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("HR");
        result.Description.Should().Be("Human Resources");
        result.Code.Should().Be("HR");
        _mockDepartmentRepository.Verify(r => r.CreateAsync(It.Is<Department>(d =>
            d.Name == "HR" &&
            d.Code == "HR" &&
            d.IsActive == true
        )), Times.Once);
    }

    [Fact]
    public async Task CreateDepartmentAsync_Should_Add_Managers_When_Specified()
    {
        // Arrange
        var managerId = "mgr-123";
        var request = new CreateDepartmentRequest
        {
            Name = "Engineering",
            Description = "Engineering Dept",
            Code = "ENG",
            IsActive = true,
            ManagerIds = new List<string> { managerId }
        };

        var manager = new Employee
        {
            Id = managerId,
            FirstName = "Manager",
            LastName = "User",
            Email = "manager@example.com",
            JobTitle = "Engineering Manager",
            Department = "Engineering",
            HireDate = DateTime.UtcNow,
            Status = EmploymentStatus.Active,
            EmploymentType = EmploymentType.FullTime
        };

        var createdDepartment = new Department
        {
            Id = "dept-123",
            Name = "Engineering",
            Description = "Engineering Dept",
            Code = "ENG",
            IsActive = true,
            DepartmentManagers = new List<DepartmentManager>()
        };

        _mockDepartmentRepository.Setup(r => r.CreateAsync(It.IsAny<Department>()))
            .ReturnsAsync(createdDepartment);
        _mockEmployeeRepository.Setup(r => r.GetByIdAsync(managerId)).ReturnsAsync(manager);
        _mockDepartmentRepository.Setup(r => r.UpdateAsync(It.IsAny<Department>()))
            .ReturnsAsync((Department d) => d);

        // Act
        var result = await _service.CreateDepartmentAsync(request);

        // Assert
        _mockDepartmentRepository.Verify(r => r.UpdateAsync(It.Is<Department>(d =>
            d.DepartmentManagers.Any(dm => dm.ManagerId == managerId && dm.IsPrimary)
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateDepartmentAsync_Should_Update_Existing_Department()
    {
        // Arrange
        var existingDepartment = new Department
        {
            Id = "123",
            Name = "Old Name",
            Description = "Old Description",
            Code = "OLD",
            IsActive = true,
            DepartmentManagers = new List<DepartmentManager>()
        };

        var updateRequest = new UpdateDepartmentRequest
        {
            Name = "New Name",
            Description = "New Description",
            Code = "NEW",
            Location = "New Location",
            IsActive = true
        };

        _mockDepartmentRepository.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(existingDepartment);
        _mockDepartmentRepository.Setup(r => r.UpdateAsync(It.IsAny<Department>()))
            .ReturnsAsync((Department d) => d);

        // Act
        var result = await _service.UpdateDepartmentAsync("123", updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Name");
        result.Description.Should().Be("New Description");
        result.Code.Should().Be("NEW");
        _mockDepartmentRepository.Verify(r => r.UpdateAsync(It.Is<Department>(d =>
            d.Id == "123" &&
            d.Name == "New Name" &&
            d.Code == "NEW"
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateDepartmentAsync_Should_Throw_When_Department_Not_Found()
    {
        // Arrange
        var updateRequest = new UpdateDepartmentRequest
        {
            Name = "Test",
            Description = "Test Dept",
            Code = "TST",
            IsActive = true
        };

        _mockDepartmentRepository.Setup(r => r.GetByIdAsync("999")).ReturnsAsync((Department?)null);

        // Act
        Func<Task> act = async () => await _service.UpdateDepartmentAsync("999", updateRequest);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Department with ID 999 not found");
    }

    [Fact]
    public async Task DeleteDepartmentAsync_Should_Call_Repository_Delete()
    {
        // Arrange
        var departmentId = "123";

        // Act
        await _service.DeleteDepartmentAsync(departmentId);

        // Assert
        _mockDepartmentRepository.Verify(r => r.DeleteAsync(departmentId), Times.Once);
    }
}
