using System.Text;
using FluentAssertions;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;

namespace HR.Application.Tests.Services;

public class ExportServiceTests
{
    private readonly ExportService _service;

    public ExportServiceTests()
    {
        _service = new ExportService();
    }

    [Fact]
    public void GenerateEmployeeCsv_Should_Generate_Valid_CSV_With_Headers()
    {
        // Arrange
        var employees = new List<EmployeeDto>
        {
            new EmployeeDto
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Department = "Engineering",
                HireDate = new DateTime(2020, 1, 15)
            }
        };

        // Act
        var (base64Content, fileName) = _service.GenerateEmployeeCsv(employees);

        // Assert
        base64Content.Should().NotBeNullOrEmpty();
        fileName.Should().StartWith("employees-export-");
        fileName.Should().EndWith(".csv");

        // Decode and verify content
        var csvBytes = Convert.FromBase64String(base64Content);
        var csvContent = Encoding.UTF8.GetString(csvBytes);

        csvContent.Should().Contain("Full Name,Email,Department,Hire Date");
        csvContent.Should().Contain("\"John Doe\",\"john.doe@example.com\",\"Engineering\",\"2020-01-15\"");
    }

    [Fact]
    public void GenerateEmployeeCsv_Should_Handle_Empty_List()
    {
        // Arrange
        var employees = new List<EmployeeDto>();

        // Act
        var (base64Content, fileName) = _service.GenerateEmployeeCsv(employees);

        // Assert
        base64Content.Should().NotBeNullOrEmpty();
        fileName.Should().Contain("employees-export-");

        var csvBytes = Convert.FromBase64String(base64Content);
        var csvContent = Encoding.UTF8.GetString(csvBytes);

        // Should only have headers
        csvContent.Should().Contain("Full Name,Email,Department,Hire Date");
        csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries).Should().HaveCount(1);
    }

    [Fact]
    public void GenerateEmployeeCsv_Should_Handle_Null_Input()
    {
        // Act
        var (base64Content, fileName) = _service.GenerateEmployeeCsv(null!);

        // Assert
        base64Content.Should().NotBeNullOrEmpty();

        var csvBytes = Convert.FromBase64String(base64Content);
        var csvContent = Encoding.UTF8.GetString(csvBytes);

        csvContent.Should().Contain("Full Name,Email,Department,Hire Date");
    }

    [Fact]
    public void GenerateEmployeeCsv_Should_Handle_Null_Department()
    {
        // Arrange
        var employees = new List<EmployeeDto>
        {
            new EmployeeDto
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                Department = null,  // Null department
                HireDate = new DateTime(2021, 3, 10)
            }
        };

        // Act
        var (base64Content, fileName) = _service.GenerateEmployeeCsv(employees);

        // Assert
        var csvBytes = Convert.FromBase64String(base64Content);
        var csvContent = Encoding.UTF8.GetString(csvBytes);

        csvContent.Should().Contain("\"Jane Smith\",\"jane@example.com\",\"\",\"2021-03-10\"");
    }

    [Fact]
    public void GenerateEmployeeCsv_Should_Handle_Multiple_Employees()
    {
        // Arrange
        var employees = new List<EmployeeDto>
        {
            new EmployeeDto
            {
                Id = Guid.NewGuid(),
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice@example.com",
                Department = "HR",
                HireDate = new DateTime(2019, 5, 20)
            },
            new EmployeeDto
            {
                Id = Guid.NewGuid(),
                FirstName = "Bob",
                LastName = "Williams",
                Email = "bob@example.com",
                Department = "Sales",
                HireDate = new DateTime(2020, 11, 1)
            },
            new EmployeeDto
            {
                Id = Guid.NewGuid(),
                FirstName = "Charlie",
                LastName = "Brown",
                Email = "charlie@example.com",
                Department = "IT",
                HireDate = new DateTime(2022, 2, 14)
            }
        };

        // Act
        var (base64Content, fileName) = _service.GenerateEmployeeCsv(employees);

        // Assert
        var csvBytes = Convert.FromBase64String(base64Content);
        var csvContent = Encoding.UTF8.GetString(csvBytes);

        csvContent.Should().Contain("Alice Johnson");
        csvContent.Should().Contain("Bob Williams");
        csvContent.Should().Contain("Charlie Brown");

        // Should have header + 3 data rows
        var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        lines.Should().HaveCount(4);
    }

    [Fact]
    public void GenerateDepartmentCsv_Should_Generate_Valid_CSV_With_Headers()
    {
        // Arrange
        var departments = new List<DepartmentDto>
        {
            new DepartmentDto
            {
                Id = Guid.NewGuid(),
                Name = "Engineering",
                Description = "Software Engineering Department",
                EmployeeCount = 25
            }
        };

        // Act
        var (base64Content, fileName) = _service.GenerateDepartmentCsv(departments);

        // Assert
        base64Content.Should().NotBeNullOrEmpty();
        fileName.Should().StartWith("departments-export-");
        fileName.Should().EndWith(".csv");

        var csvBytes = Convert.FromBase64String(base64Content);
        var csvContent = Encoding.UTF8.GetString(csvBytes);

        csvContent.Should().Contain("Name,Employee Count,Description");
        csvContent.Should().Contain("\"Engineering\",\"25\",\"Software Engineering Department\"");
    }

    [Fact]
    public void GenerateDepartmentCsv_Should_Handle_Empty_List()
    {
        // Arrange
        var departments = new List<DepartmentDto>();

        // Act
        var (base64Content, fileName) = _service.GenerateDepartmentCsv(departments);

        // Assert
        base64Content.Should().NotBeNullOrEmpty();

        var csvBytes = Convert.FromBase64String(base64Content);
        var csvContent = Encoding.UTF8.GetString(csvBytes);

        csvContent.Should().Contain("Name,Employee Count,Description");
        csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries).Should().HaveCount(1);
    }

    [Fact]
    public void GenerateDepartmentCsv_Should_Handle_Null_Description()
    {
        // Arrange
        var departments = new List<DepartmentDto>
        {
            new DepartmentDto
            {
                Id = Guid.NewGuid(),
                Name = "Sales",
                Description = null,  // Null description
                EmployeeCount = 10
            }
        };

        // Act
        var (base64Content, fileName) = _service.GenerateDepartmentCsv(departments);

        // Assert
        var csvBytes = Convert.FromBase64String(base64Content);
        var csvContent = Encoding.UTF8.GetString(csvBytes);

        csvContent.Should().Contain("\"Sales\",\"10\",\"\"");
    }

    [Fact]
    public void GenerateDepartmentCsv_Should_Handle_Multiple_Departments()
    {
        // Arrange
        var departments = new List<DepartmentDto>
        {
            new DepartmentDto { Id = Guid.NewGuid(), Name = "Engineering", Description = "Tech team", EmployeeCount = 30 },
            new DepartmentDto { Id = Guid.NewGuid(), Name = "Sales", Description = "Sales team", EmployeeCount = 15 },
            new DepartmentDto { Id = Guid.NewGuid(), Name = "HR", Description = "Human Resources", EmployeeCount = 5 }
        };

        // Act
        var (base64Content, fileName) = _service.GenerateDepartmentCsv(departments);

        // Assert
        var csvBytes = Convert.FromBase64String(base64Content);
        var csvContent = Encoding.UTF8.GetString(csvBytes);

        csvContent.Should().Contain("Engineering");
        csvContent.Should().Contain("Sales");
        csvContent.Should().Contain("HR");

        var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        lines.Should().HaveCount(4); // header + 3 rows
    }
}
