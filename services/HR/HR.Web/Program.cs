using HR.Web.Components;
using HR.Application.Services;
using HR.Domain.Repositories;
using HR.Infrastructure.Persistence;
using HR.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container - using Blazor Server to match shell
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add MudBlazor services
builder.Services.AddMudServices();

// Database configuration - use in-memory for development
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", true);

if (useInMemory)
{
    Console.WriteLine("⚠️  HR.Web using in-memory database");
    builder.Services.AddDbContext<HRDbContext>(options =>
        options.UseInMemoryDatabase("HR_Web"));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("HRDatabase") 
        ?? "Server=localhost;Database=BusinessAsUsual_HR;Trusted_Connection=True;TrustServerCertificate=True;";
    builder.Services.AddDbContext<HRDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Register repositories
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

// Register services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();

// CORS configuration for iframe embedding
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebShell", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5269",  // Main web shell
            "https://localhost:7229"  // Main web shell HTTPS
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Seed some sample data in development
if (app.Environment.IsDevelopment() && useInMemory)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<HRDbContext>();
    await SeedSampleData(dbContext);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowWebShell");

app.UseRouting();

app.UseAntiforgery();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

Console.WriteLine("✓ HR Web UI ready on http://localhost:5002");

app.Run();

#pragma warning disable CS0618 // Type or member is obsolete - intentionally using legacy fields for seed data
async Task SeedSampleData(HRDbContext context)
{
    // Clear existing data if needed (for development/testing)
    if (await context.Employees.AnyAsync() || await context.Departments.AnyAsync())
    {
        return; // Skip seeding if data exists
    }

    // Step 1: Create Employees (without department assignments yet)
    var employees = new[]
    {
        new HR.Domain.Entities.Employee
        {
            Id = "emp001",
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith@company.com",
            PersonalEmail = "jsmith@email.com",
            PhoneNumber = "555-0101",
            DateOfBirth = new DateTime(1985, 3, 15),
            Department = "Engineering",  // Legacy field
            JobTitle = "Senior Software Engineer",
            EmploymentType = HR.Domain.Entities.EmploymentType.FullTime,
            HireDate = DateTime.Now.AddYears(-2),
            Status = HR.Domain.Entities.EmploymentStatus.Active,
            WorkLocation = "Seattle Office",
            SalaryGrade = "L4",
            AddressLine1 = "123 Tech Street",
            City = "Seattle",
            State = "WA",
            PostalCode = "98101",
            Country = "USA",
            EmergencyContactName = "Jane Smith",
            EmergencyContactPhone = "555-0199",
            EmergencyContactRelationship = "Spouse"
        },
        new HR.Domain.Entities.Employee
        {
            Id = "emp002",
            FirstName = "Sarah",
            LastName = "Johnson",
            Email = "sarah.johnson@company.com",
            PersonalEmail = "sjohnson@email.com",
            PhoneNumber = "555-0102",
            DateOfBirth = new DateTime(1988, 7, 22),
            Department = "Sales",  // Legacy field
            JobTitle = "Sales Manager",
            EmploymentType = HR.Domain.Entities.EmploymentType.FullTime,
            HireDate = DateTime.Now.AddYears(-1).AddMonths(-6),
            Status = HR.Domain.Entities.EmploymentStatus.Active,
            WorkLocation = "Seattle Office",
            SalaryGrade = "M2",
            AddressLine1 = "456 Market Ave",
            City = "Seattle",
            State = "WA",
            PostalCode = "98102",
            Country = "USA",
            EmergencyContactName = "Mike Johnson",
            EmergencyContactPhone = "555-0198",
            EmergencyContactRelationship = "Sibling"
        },
        new HR.Domain.Entities.Employee
        {
            Id = "emp003",
            FirstName = "Michael",
            LastName = "Williams",
            Email = "michael.williams@company.com",
            PhoneNumber = "555-0103",
            DateOfBirth = new DateTime(1982, 11, 5),
            Department = "Marketing",  // Legacy field
            JobTitle = "Marketing Director",
            EmploymentType = HR.Domain.Entities.EmploymentType.FullTime,
            HireDate = DateTime.Now.AddMonths(-6),
            Status = HR.Domain.Entities.EmploymentStatus.Active,
            WorkLocation = "Remote",
            SalaryGrade = "M3",
            City = "Portland",
            State = "OR",
            Country = "USA"
        },
        new HR.Domain.Entities.Employee
        {
            Id = "emp004",
            FirstName = "Emily",
            LastName = "Davis",
            Email = "emily.davis@company.com",
            PhoneNumber = "555-0104",
            DateOfBirth = new DateTime(1990, 1, 18),
            Department = "Human Resources",  // Legacy field
            JobTitle = "HR Specialist",
            EmploymentType = HR.Domain.Entities.EmploymentType.FullTime,
            HireDate = DateTime.Now.AddYears(-3),
            Status = HR.Domain.Entities.EmploymentStatus.Active,
            WorkLocation = "Seattle Office",
            SalaryGrade = "L3",
            City = "Seattle",
            State = "WA",
            Country = "USA"
        },
        new HR.Domain.Entities.Employee
        {
            Id = "emp005",
            FirstName = "David",
            LastName = "Martinez",
            Email = "david.martinez@company.com",
            PhoneNumber = "555-0105",
            DateOfBirth = new DateTime(1987, 9, 30),
            Department = "Finance",  // Legacy field
            JobTitle = "Financial Analyst",
            EmploymentType = HR.Domain.Entities.EmploymentType.FullTime,
            HireDate = DateTime.Now.AddMonths(-9),
            Status = HR.Domain.Entities.EmploymentStatus.Active,
            WorkLocation = "Seattle Office",
            SalaryGrade = "L4",
            City = "Seattle",
            State = "WA",
            Country = "USA"
        },
        new HR.Domain.Entities.Employee
        {
            Id = "emp006",
            FirstName = "Lisa",
            LastName = "Anderson",
            Email = "lisa.anderson@company.com",
            PhoneNumber = "555-0106",
            DateOfBirth = new DateTime(1984, 4, 12),
            Department = "Engineering",  // Legacy field
            JobTitle = "Engineering Manager",
            EmploymentType = HR.Domain.Entities.EmploymentType.FullTime,
            HireDate = DateTime.Now.AddYears(-4),
            Status = HR.Domain.Entities.EmploymentStatus.Active,
            WorkLocation = "Seattle Office",
            SalaryGrade = "M2",
            City = "Seattle",
            State = "WA",
            Country = "USA"
        },
        new HR.Domain.Entities.Employee
        {
            Id = "emp007",
            FirstName = "Robert",
            LastName = "Taylor",
            Email = "robert.taylor@company.com",
            PhoneNumber = "555-0107",
            DateOfBirth = new DateTime(1992, 6, 8),
            Department = "Sales",  // Legacy field
            JobTitle = "Account Executive",
            EmploymentType = HR.Domain.Entities.EmploymentType.FullTime,
            HireDate = DateTime.Now.AddMonths(-3),
            Status = HR.Domain.Entities.EmploymentStatus.Active,
            WorkLocation = "Remote",
            SalaryGrade = "L2",
            City = "Austin",
            State = "TX",
            Country = "USA"
        }
    };

    // Set manager relationships
    employees[0].ManagerId = "emp006";  // John reports to Lisa (Engineering Manager)
    employees[1].ManagerId = null;       // Sarah is a manager
    employees[2].ManagerId = null;       // Michael is a director
    employees[3].ManagerId = null;       // Emily is independent
    employees[4].ManagerId = null;       // David is independent
    employees[5].ManagerId = null;       // Lisa is a manager
    employees[6].ManagerId = "emp002";  // Robert reports to Sarah (Sales Manager)

    context.Employees.AddRange(employees);
    await context.SaveChangesAsync();

    // Step 2: Create Departments
    var departments = new[]
    {
        new HR.Domain.Entities.Department
        {
            Id = "dept001",
            Name = "Engineering",
            Description = "Software development and technical operations",
            Code = "ENG",
            Location = "Seattle Office",
            CostCenter = "CC-1000",
            IsActive = true,
            ManagerEmployeeId = "emp006"  // Lisa Anderson - legacy field
            // Note: EmployeeCount is now calculated dynamically from EmployeeDepartments
        },
        new HR.Domain.Entities.Department
        {
            Id = "dept002",
            Name = "Sales",
            Description = "Customer acquisition and account management",
            Code = "SAL",
            Location = "Seattle Office",
            CostCenter = "CC-2000",
            IsActive = true,
            ManagerEmployeeId = "emp002"  // Sarah Johnson - legacy field
            // Note: EmployeeCount is now calculated dynamically
        },
        new HR.Domain.Entities.Department
        {
            Id = "dept003",
            Name = "Marketing",
            Description = "Brand management and promotional campaigns",
            Code = "MKT",
            Location = "Remote",
            CostCenter = "CC-3000",
            IsActive = true,
            ManagerEmployeeId = "emp003"  // Michael Williams - legacy field
            // Note: EmployeeCount is now calculated dynamically
        },
        new HR.Domain.Entities.Department
        {
            Id = "dept004",
            Name = "Human Resources",
            Description = "Employee relations and talent management",
            Code = "HR",
            Location = "Seattle Office",
            CostCenter = "CC-4000",
            IsActive = true,
            ManagerEmployeeId = "emp004"  // Emily Davis - legacy field
            // Note: EmployeeCount is now calculated dynamically
        },
        new HR.Domain.Entities.Department
        {
            Id = "dept005",
            Name = "Finance",
            Description = "Financial planning and accounting",
            Code = "FIN",
            Location = "Seattle Office",
            CostCenter = "CC-5000",
            IsActive = true,
            ManagerEmployeeId = "emp005"  // David Martinez - legacy field
            // Note: EmployeeCount is now calculated dynamically
        },
        new HR.Domain.Entities.Department
        {
            Id = "dept006",
            Name = "Engineering - Frontend Team",
            Description = "Frontend development specialists",
            Code = "ENG-FE",
            Location = "Seattle Office",
            CostCenter = "CC-1100",
            ParentDepartmentId = "dept001",  // Sub-department of Engineering
            IsActive = true
        },
        new HR.Domain.Entities.Department
        {
            Id = "dept007",
            Name = "Engineering - Backend Team",
            Description = "Backend and infrastructure development",
            Code = "ENG-BE",
            Location = "Seattle Office",
            CostCenter = "CC-1200",
            ParentDepartmentId = "dept001",  // Sub-department of Engineering
            IsActive = true
        }
    };

    context.Departments.AddRange(departments);
    await context.SaveChangesAsync();

    // Step 3: Create Employee-Department relationships (many-to-many)
    var employeeDepartments = new[]
    {
        // John - Engineering (primary) + Frontend Team
        new HR.Domain.Entities.EmployeeDepartment { EmployeeId = "emp001", DepartmentId = "dept001", IsPrimary = true, AllocationPercentage = 70, JoinedDate = DateTime.Now.AddYears(-2) },
        new HR.Domain.Entities.EmployeeDepartment { EmployeeId = "emp001", DepartmentId = "dept006", IsPrimary = false, AllocationPercentage = 30, JoinedDate = DateTime.Now.AddYears(-1) },

        // Sarah - Sales (primary)
        new HR.Domain.Entities.EmployeeDepartment { EmployeeId = "emp002", DepartmentId = "dept002", IsPrimary = true, AllocationPercentage = 100, JoinedDate = DateTime.Now.AddYears(-1).AddMonths(-6) },

        // Michael - Marketing (primary) + occasional Engineering collaboration
        new HR.Domain.Entities.EmployeeDepartment { EmployeeId = "emp003", DepartmentId = "dept003", IsPrimary = true, AllocationPercentage = 90, JoinedDate = DateTime.Now.AddMonths(-6) },
        new HR.Domain.Entities.EmployeeDepartment { EmployeeId = "emp003", DepartmentId = "dept001", IsPrimary = false, AllocationPercentage = 10, JoinedDate = DateTime.Now.AddMonths(-3) },

        // Emily - HR (primary)
        new HR.Domain.Entities.EmployeeDepartment { EmployeeId = "emp004", DepartmentId = "dept004", IsPrimary = true, AllocationPercentage = 100, JoinedDate = DateTime.Now.AddYears(-3) },

        // David - Finance (primary)
        new HR.Domain.Entities.EmployeeDepartment { EmployeeId = "emp005", DepartmentId = "dept005", IsPrimary = true, AllocationPercentage = 100, JoinedDate = DateTime.Now.AddMonths(-9) },

        // Lisa - Engineering (primary)
        new HR.Domain.Entities.EmployeeDepartment { EmployeeId = "emp006", DepartmentId = "dept001", IsPrimary = true, AllocationPercentage = 100, JoinedDate = DateTime.Now.AddYears(-4) },

        // Robert - Sales (primary)
        new HR.Domain.Entities.EmployeeDepartment { EmployeeId = "emp007", DepartmentId = "dept002", IsPrimary = true, AllocationPercentage = 100, JoinedDate = DateTime.Now.AddMonths(-3) }
    };

    context.EmployeeDepartments.AddRange(employeeDepartments);
    await context.SaveChangesAsync();

    // Step 4: Create Department-Manager relationships (many-to-many)
    var departmentManagers = new[]
    {
        // Engineering has Lisa as primary manager + John as team lead
        new HR.Domain.Entities.DepartmentManager { DepartmentId = "dept001", ManagerId = "emp006", ManagerRole = "Engineering Manager", IsPrimary = true, StartDate = DateTime.Now.AddYears(-4) },
        new HR.Domain.Entities.DepartmentManager { DepartmentId = "dept001", ManagerId = "emp001", ManagerRole = "Tech Lead", IsPrimary = false, StartDate = DateTime.Now.AddYears(-1) },

        // Sales has Sarah as primary manager
        new HR.Domain.Entities.DepartmentManager { DepartmentId = "dept002", ManagerId = "emp002", ManagerRole = "Sales Manager", IsPrimary = true, StartDate = DateTime.Now.AddYears(-1).AddMonths(-6) },

        // Marketing has Michael as director
        new HR.Domain.Entities.DepartmentManager { DepartmentId = "dept003", ManagerId = "emp003", ManagerRole = "Marketing Director", IsPrimary = true, StartDate = DateTime.Now.AddMonths(-6) },

        // HR has Emily
        new HR.Domain.Entities.DepartmentManager { DepartmentId = "dept004", ManagerId = "emp004", ManagerRole = "HR Manager", IsPrimary = true, StartDate = DateTime.Now.AddYears(-3) },

        // Finance has David
        new HR.Domain.Entities.DepartmentManager { DepartmentId = "dept005", ManagerId = "emp005", ManagerRole = "Finance Manager", IsPrimary = true, StartDate = DateTime.Now.AddMonths(-9) },

        // Frontend Team has John as lead
        new HR.Domain.Entities.DepartmentManager { DepartmentId = "dept006", ManagerId = "emp001", ManagerRole = "Frontend Team Lead", IsPrimary = true, StartDate = DateTime.Now.AddYears(-1) },

        // Backend Team has Lisa overseeing
        new HR.Domain.Entities.DepartmentManager { DepartmentId = "dept007", ManagerId = "emp006", ManagerRole = "Backend Team Supervisor", IsPrimary = true, StartDate = DateTime.Now.AddYears(-2) }
    };

    context.DepartmentManagers.AddRange(departmentManagers);
    await context.SaveChangesAsync();

    Console.WriteLine("✓ Seeded comprehensive HR data:");
    Console.WriteLine("  - 7 employees with full details");
    Console.WriteLine("  - 7 departments (5 main + 2 sub-departments/pods)");
    Console.WriteLine("  - Multiple managers per department");
    Console.WriteLine("  - Employees in multiple departments");
    Console.WriteLine("  - Department hierarchy (Engineering with Frontend/Backend teams)");
}
#pragma warning restore CS0618
