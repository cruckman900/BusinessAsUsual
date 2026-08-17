using HR.Domain.Entities;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HR.Infrastructure.Data;

/// <summary>
/// Seeds the HR database with realistic demo data for employees and departments
/// </summary>
public class HRSeedData
{
    private readonly HRDbContext _context;
    private readonly ILogger<HRSeedData> _logger;

    public HRSeedData(HRDbContext context, ILogger<HRSeedData> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        // Check if data already exists
        if (await _context.Employees.AnyAsync())
        {
            _logger.LogInformation("HR database already seeded, skipping...");
            return;
        }

        _logger.LogInformation("Seeding HR database with demo data...");

        // Create departments first
        var departments = CreateDepartments();
        await _context.Departments.AddRangeAsync(departments);
        await _context.SaveChangesAsync();

        // Create employees
        var employees = CreateEmployees(departments);
        await _context.Employees.AddRangeAsync(employees);
        await _context.SaveChangesAsync();

        // Create employee-department relationships
        var employeeDepartments = CreateEmployeeDepartments(employees, departments);
        await _context.EmployeeDepartments.AddRangeAsync(employeeDepartments);
        await _context.SaveChangesAsync();

        // Create sample training completions
        var trainingCompletions = CreateTrainingCompletions(employees);
        await _context.TrainingCompletions.AddRangeAsync(trainingCompletions);
        await _context.SaveChangesAsync();

        _logger.LogInformation("HR database seeded successfully!");
    }

    private List<Department> CreateDepartments()
    {
        return new List<Department>
        {
            new Department
            {
                Id = "DEPT001",
                Name = "Engineering",
                Code = "ENG",
                Description = "Software development and technical teams",
                Location = "Main Office - Floor 3",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddYears(-2)
            },
            new Department
            {
                Id = "DEPT002",
                Name = "Project Management",
                Code = "PM",
                Description = "Project planning, coordination, and delivery",
                Location = "Main Office - Floor 2",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddYears(-2)
            },
            new Department
            {
                Id = "DEPT003",
                Name = "Human Resources",
                Code = "HR",
                Description = "People operations, recruitment, and employee relations",
                Location = "Main Office - Floor 1",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddYears(-2)
            },
            new Department
            {
                Id = "DEPT004",
                Name = "Data & Analytics",
                Code = "DATA",
                Description = "Data engineering, analytics, and business intelligence",
                Location = "Main Office - Floor 3",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddYears(-2)
            }
        };
    }

    private List<Employee> CreateEmployees(List<Department> departments)
    {
        return new List<Employee>
        {
            new Employee
            {
                Id = "EMP001",
                FirstName = "Sarah",
                LastName = "Johnson",
                Email = "sarah.johnson@businessasusual.com",
                PhoneNumber = "(555) 101-2001",
                JobTitle = "Senior Developer",
                EmploymentType = EmploymentType.FullTime,
                Status = EmploymentStatus.Active,
                HireDate = DateTime.UtcNow.AddYears(-3).AddMonths(-2),
                WorkLocation = "Main Office - Floor 3",
                SalaryGrade = "L4",
                Department = "Engineering",
                CreatedAt = DateTime.UtcNow.AddYears(-3).AddMonths(-2),
                UpdatedAt = DateTime.UtcNow.AddYears(-3).AddMonths(-2)
            },
            new Employee
            {
                Id = "EMP002",
                FirstName = "Michael",
                LastName = "Chen",
                Email = "michael.chen@businessasusual.com",
                PhoneNumber = "(555) 101-2002",
                JobTitle = "Junior Developer",
                EmploymentType = EmploymentType.FullTime,
                Status = EmploymentStatus.Active,
                HireDate = DateTime.UtcNow.AddMonths(-8),
                WorkLocation = "Main Office - Floor 3",
                ManagerId = "EMP001",
                SalaryGrade = "L2",
                Department = "Engineering",
                CreatedAt = DateTime.UtcNow.AddMonths(-8),
                UpdatedAt = DateTime.UtcNow.AddMonths(-8)
            },
            new Employee
            {
                Id = "EMP003",
                FirstName = "Emily",
                LastName = "Rodriguez",
                Email = "emily.rodriguez@businessasusual.com",
                PhoneNumber = "(555) 101-2003",
                JobTitle = "Project Manager",
                EmploymentType = EmploymentType.FullTime,
                Status = EmploymentStatus.Active,
                HireDate = DateTime.UtcNow.AddYears(-2).AddMonths(-6),
                WorkLocation = "Main Office - Floor 2",
                SalaryGrade = "L5",
                Department = "Project Management",
                CreatedAt = DateTime.UtcNow.AddYears(-2).AddMonths(-6),
                UpdatedAt = DateTime.UtcNow.AddYears(-2).AddMonths(-6)
            },
            new Employee
            {
                Id = "EMP004",
                FirstName = "David",
                LastName = "Kim",
                Email = "david.kim@businessasusual.com",
                PhoneNumber = "(555) 101-2004",
                JobTitle = "Database Administrator",
                EmploymentType = EmploymentType.FullTime,
                Status = EmploymentStatus.Active,
                HireDate = DateTime.UtcNow.AddYears(-1).AddMonths(-10),
                WorkLocation = "Main Office - Floor 3",
                SalaryGrade = "L4",
                Department = "Data & Analytics",
                CreatedAt = DateTime.UtcNow.AddYears(-1).AddMonths(-10),
                UpdatedAt = DateTime.UtcNow.AddYears(-1).AddMonths(-10)
            },
            new Employee
            {
                Id = "EMP005",
                FirstName = "Lisa",
                LastName = "Anderson",
                Email = "lisa.anderson@businessasusual.com",
                PhoneNumber = "(555) 101-2005",
                JobTitle = "HR Manager",
                EmploymentType = EmploymentType.FullTime,
                Status = EmploymentStatus.Active,
                HireDate = DateTime.UtcNow.AddYears(-4),
                WorkLocation = "Main Office - Floor 1",
                SalaryGrade = "L6",
                Department = "Human Resources",
                CreatedAt = DateTime.UtcNow.AddYears(-4),
                UpdatedAt = DateTime.UtcNow.AddYears(-4)
            }
        };
    }

    private List<EmployeeDepartment> CreateEmployeeDepartments(List<Employee> employees, List<Department> departments)
    {
        var engineeringDept = departments.First(d => d.Code == "ENG");
        var pmDept = departments.First(d => d.Code == "PM");
        var hrDept = departments.First(d => d.Code == "HR");
        var dataDept = departments.First(d => d.Code == "DATA");

        return new List<EmployeeDepartment>
        {
            new EmployeeDepartment
            {
                EmployeeId = "EMP001",
                DepartmentId = engineeringDept.Id,
                IsPrimary = true,
                JoinedDate = employees[0].HireDate
            },
            new EmployeeDepartment
            {
                EmployeeId = "EMP002",
                DepartmentId = engineeringDept.Id,
                IsPrimary = true,
                JoinedDate = employees[1].HireDate
            },
            new EmployeeDepartment
            {
                EmployeeId = "EMP003",
                DepartmentId = pmDept.Id,
                IsPrimary = true,
                JoinedDate = employees[2].HireDate
            },
            new EmployeeDepartment
            {
                EmployeeId = "EMP004",
                DepartmentId = dataDept.Id,
                IsPrimary = true,
                JoinedDate = employees[3].HireDate
            },
            new EmployeeDepartment
            {
                EmployeeId = "EMP005",
                DepartmentId = hrDept.Id,
                IsPrimary = true,
                JoinedDate = employees[4].HireDate
            }
        };
    }

    private List<TrainingCompletion> CreateTrainingCompletions(List<Employee> employees)
    {
        // Sample course IDs that match the LMS seed data
        var csharpCourseId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var agileCourseId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var databaseCourseId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var safetyFundamentalsId = Guid.NewGuid();

        return new List<TrainingCompletion>
        {
            // Alice (EMP001) - Software Engineer - completed C# Fundamentals
            new TrainingCompletion
            {
                EmployeeId = "EMP001",
                CourseId = csharpCourseId,
                CourseName = "C# Fundamentals",
                CompletionDate = DateTime.UtcNow.AddDays(-30),
                Score = 95,
                CertificateNumber = "CERT-2024-001",
                TimeSpentMinutes = 180,
                SourceEventId = Guid.NewGuid(),
                RecordedAt = DateTime.UtcNow.AddDays(-30)
            },
            // Alice also completed Agile Methodology
            new TrainingCompletion
            {
                EmployeeId = "EMP001",
                CourseId = agileCourseId,
                CourseName = "Agile Methodology",
                CompletionDate = DateTime.UtcNow.AddDays(-15),
                Score = 88,
                CertificateNumber = "CERT-2024-002",
                TimeSpentMinutes = 120,
                SourceEventId = Guid.NewGuid(),
                RecordedAt = DateTime.UtcNow.AddDays(-15)
            },
            // Bob (EMP002) - Senior Developer - completed Database Design
            new TrainingCompletion
            {
                EmployeeId = "EMP002",
                CourseId = databaseCourseId,
                CourseName = "Database Design Principles",
                CompletionDate = DateTime.UtcNow.AddDays(-20),
                Score = 92,
                CertificateNumber = "CERT-2024-003",
                TimeSpentMinutes = 240,
                SourceEventId = Guid.NewGuid(),
                RecordedAt = DateTime.UtcNow.AddDays(-20)
            },
            // Carol (EMP003) - Project Manager - completed Agile
            new TrainingCompletion
            {
                EmployeeId = "EMP003",
                CourseId = agileCourseId,
                CourseName = "Agile Methodology",
                CompletionDate = DateTime.UtcNow.AddDays(-10),
                Score = 98,
                CertificateNumber = "CERT-2024-004",
                TimeSpentMinutes = 150,
                SourceEventId = Guid.NewGuid(),
                RecordedAt = DateTime.UtcNow.AddDays(-10)
            },
            // David (EMP004) - Data Analyst - completed C# Fundamentals
            new TrainingCompletion
            {
                EmployeeId = "EMP004",
                CourseId = csharpCourseId,
                CourseName = "C# Fundamentals",
                CompletionDate = DateTime.UtcNow.AddDays(-5),
                Score = 85,
                CertificateNumber = "CERT-2024-005",
                TimeSpentMinutes = 210,
                SourceEventId = Guid.NewGuid(),
                RecordedAt = DateTime.UtcNow.AddDays(-5)
            },
            // Emily (EMP005) - HR Manager - completed Workplace Safety
            new TrainingCompletion
            {
                EmployeeId = "EMP005",
                CourseId = safetyFundamentalsId,
                CourseName = "Workplace Safety Fundamentals",
                CompletionDate = DateTime.UtcNow.AddDays(-7),
                Score = 100,
                CertificateNumber = "CERT-2024-006",
                TimeSpentMinutes = 90,
                SourceEventId = Guid.NewGuid(),
                RecordedAt = DateTime.UtcNow.AddDays(-7)
            }
        };
    }
}
