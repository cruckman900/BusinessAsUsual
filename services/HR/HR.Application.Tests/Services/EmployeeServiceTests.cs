using FluentAssertions;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using HR.Domain.Repositories;
using Moq;

namespace HR.Application.Tests.Services;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _mockRepository;
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        _mockRepository = new Mock<IEmployeeRepository>();
        _service = new EmployeeService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllEmployeesAsync_Should_Return_All_Employees()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                JobTitle = "Software Engineer",
                Department = "Engineering",
                HireDate = new DateTime(2020, 1, 1),
                Status = EmploymentStatus.Active,
                EmploymentType = EmploymentType.FullTime
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                JobTitle = "Product Manager",
                Department = "Product",
                HireDate = new DateTime(2021, 6, 15),
                Status = EmploymentStatus.Active,
                EmploymentType = EmploymentType.FullTime
            }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(employees);

        // Act
        var result = await _service.GetAllEmployeesAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(e => e.FirstName == "John" && e.LastName == "Doe");
        result.Should().Contain(e => e.FirstName == "Jane" && e.LastName == "Smith");
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_Should_Return_Employee_When_Found()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var employee = new Employee
        {
            Id = employeeId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            JobTitle = "Software Engineer",
            Department = "Engineering",
            HireDate = new DateTime(2020, 1, 1),
            Status = EmploymentStatus.Active,
            EmploymentType = EmploymentType.FullTime
        };

        _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(employee);

        // Act
        var result = await _service.GetEmployeeByIdAsync(employeeId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(employeeId);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_Should_Return_Null_When_Not_Found()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(nonExistentId)).ReturnsAsync((Employee?)null);

        // Act
        var result = await _service.GetEmployeeByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateEmployeeAsync_Should_Create_Employee_With_Valid_Data()
    {
        // Arrange
        var request = new CreateEmployeeRequest
        {
            FirstName = "Alice",
            LastName = "Johnson",
            Email = "alice.johnson@example.com",
            PersonalEmail = "alice@personal.com",
            PhoneNumber = "555-1234",
            DateOfBirth = new DateTime(1990, 5, 15),
            Department = "HR",
            JobTitle = "HR Manager",
            EmploymentType = "FullTime",
            HireDate = new DateTime(2023, 1, 1),
            Status = "Active",
            WorkLocation = "New York Office",
            SalaryGrade = "M3",
            City = "New York",
            State = "NY",
            Country = "USA"
        };

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Employee>()))
            .ReturnsAsync((Employee e) => e);

        // Act
        var result = await _service.CreateEmployeeAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("Alice");
        result.LastName.Should().Be("Johnson");
        result.Email.Should().Be("alice.johnson@example.com");
        result.JobTitle.Should().Be("HR Manager");
        _mockRepository.Verify(r => r.CreateAsync(It.Is<Employee>(e =>
            e.FirstName == "Alice" &&
            e.LastName == "Johnson" &&
            e.Status == EmploymentStatus.Active &&
            e.EmploymentType == EmploymentType.FullTime
        )), Times.Once);
    }

    [Fact]
    public async Task CreateEmployeeAsync_Should_Default_To_Active_Status_When_Invalid()
    {
        // Arrange
        var request = new CreateEmployeeRequest
        {
            FirstName = "Bob",
            LastName = "Williams",
            Email = "bob@example.com",
            Department = "Sales",
            JobTitle = "Sales Rep",
            EmploymentType = "FullTime",
            HireDate = DateTime.UtcNow,
            Status = "InvalidStatus" // Invalid status
        };

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Employee>()))
            .ReturnsAsync((Employee e) => e);

        // Act
        var result = await _service.CreateEmployeeAsync(request);

        // Assert
        _mockRepository.Verify(r => r.CreateAsync(It.Is<Employee>(e =>
            e.Status == EmploymentStatus.Active
        )), Times.Once);
    }

    [Fact]
    public async Task CreateEmployeeAsync_Should_Default_To_FullTime_When_Invalid_EmploymentType()
    {
        // Arrange
        var request = new CreateEmployeeRequest
        {
            FirstName = "Charlie",
            LastName = "Brown",
            Email = "charlie@example.com",
            Department = "IT",
            JobTitle = "Developer",
            EmploymentType = "InvalidType", // Invalid employment type
            HireDate = DateTime.UtcNow,
            Status = "Active"
        };

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Employee>()))
            .ReturnsAsync((Employee e) => e);

        // Act
        var result = await _service.CreateEmployeeAsync(request);

        // Assert
        _mockRepository.Verify(r => r.CreateAsync(It.Is<Employee>(e =>
            e.EmploymentType == EmploymentType.FullTime
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateEmployeeAsync_Should_Update_Existing_Employee()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var existingEmployee = new Employee
        {
            Id = employeeId,
            FirstName = "Old",
            LastName = "Name",
            Email = "old@example.com",
            JobTitle = "Old Title",
            Department = "Old Dept",
            HireDate = new DateTime(2020, 1, 1),
            Status = EmploymentStatus.Active,
            EmploymentType = EmploymentType.FullTime
        };

        var updateRequest = new UpdateEmployeeRequest
        {
            FirstName = "New",
            LastName = "Name",
            Email = "new@example.com",
            JobTitle = "New Title",
            Department = "New Dept",
            EmploymentType = "PartTime",
            HireDate = new DateTime(2020, 1, 1),
            Status = "OnLeave"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync(existingEmployee);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Employee>()))
            .ReturnsAsync((Employee e) => e);

        // Act
        var result = await _service.UpdateEmployeeAsync(employeeId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("New");
        result.LastName.Should().Be("Name");
        result.Email.Should().Be("new@example.com");
        result.JobTitle.Should().Be("New Title");
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<Employee>(e =>
            e.Id == employeeId &&
            e.FirstName == "New" &&
            e.Status == EmploymentStatus.OnLeave &&
            e.EmploymentType == EmploymentType.PartTime
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateEmployeeAsync_Should_Throw_When_Employee_Not_Found()
    {
        // Arrange
        var updateRequest = new UpdateEmployeeRequest
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            JobTitle = "Test",
            Department = "Test",
            EmploymentType = "FullTime",
            HireDate = DateTime.UtcNow,
            Status = "Active"
        };

        var nonExistentId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(nonExistentId)).ReturnsAsync((Employee?)null);

        // Act
        Func<Task> act = async () => await _service.UpdateEmployeeAsync(nonExistentId, updateRequest);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Employee with ID {nonExistentId} not found");
    }

    [Fact]
    public async Task DeleteEmployeeAsync_Should_Call_Repository_Delete()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        // Act
        await _service.DeleteEmployeeAsync(employeeId);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(employeeId), Times.Once);
    }

    [Fact]
    public async Task SearchEmployeesAsync_Should_Return_Matching_Employees()
    {
        // Arrange
        var searchTerm = "john";
        var matchingEmployees = new List<Employee>
        {
            new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                JobTitle = "Engineer",
                Department = "Engineering",
                HireDate = DateTime.UtcNow,
                Status = EmploymentStatus.Active,
                EmploymentType = EmploymentType.FullTime
            },
            new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Johnny",
                LastName = "Smith",
                Email = "johnny.smith@example.com",
                JobTitle = "Manager",
                Department = "Management",
                HireDate = DateTime.UtcNow,
                Status = EmploymentStatus.Active,
                EmploymentType = EmploymentType.FullTime
            }
        };

        _mockRepository.Setup(r => r.SearchAsync(searchTerm)).ReturnsAsync(matchingEmployees);

        // Act
        var result = await _service.SearchEmployeesAsync(searchTerm);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(e => e.FirstName == "John");
        result.Should().Contain(e => e.FirstName == "Johnny");
    }
}
