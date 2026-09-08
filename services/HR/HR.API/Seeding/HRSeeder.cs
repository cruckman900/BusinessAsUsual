using BusinessAsUsual.Infrastructure.SeedData;
using HR.Domain.Entities;
using HR.Infrastructure.Persistence;

namespace HR.API.Seeding;

/// <summary>
/// Dev/demo-only in-memory seed data for the HR module. Deliberately kept out of
/// Program.cs so the host bootstrap file stays small and readable; this class is the
/// single place to look when the demo dataset needs to change.
/// Follows the same instantiable-seeder shape as Services.Infrastructure.Seeding.DataSeeder,
/// Sales.API.Seeding.SalesSeeder, and Inventory.API.Seeding.InventorySeeder so every module's
/// Program.cs wires up seeding the same way: `new {Module}Seeder(...).SeedAsync()`.
/// </summary>
public class HRSeeder : IModuleSeeder<HRDbContext>
{
    private readonly HRDbContext _context;

    public HRSeeder(HRDbContext context)
    {
        _context = context;
    }

    // Explicit IModuleSeeder<HRDbContext> implementation: this seeder is constructed with its
    // DbContext up front, so the interface's context parameter is only honored here for
    // contract compliance/testability; the constructor-provided context remains the source
    // of truth. Callers should normally use the parameterless SeedAsync() below.
    Task IModuleSeeder<HRDbContext>.SeedAsync(HRDbContext context) => SeedAsync();

    public async Task SeedAsync()
    {
        var context = _context;
        if (context.Employees.Any()) return; // Already seeded

        var demoCompanyId = DemoTenant.CompanyId;

        // === DEPARTMENTS ===
        var engineeringId = Guid.NewGuid();
        var salesId = Guid.NewGuid();
        var hrId = Guid.NewGuid();
        var financeId = Guid.NewGuid();
        var supportId = Guid.NewGuid(); // sub-department of Engineering

        var engineering = new Department
        {
            Id = engineeringId,
            CompanyId = demoCompanyId,
            Name = "Engineering",
            Description = "Product engineering and software development",
            Code = "ENG",
            Location = "Business City, CA",
            CostCenter = "CC-100",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var sales = new Department
        {
            Id = salesId,
            CompanyId = demoCompanyId,
            Name = "Sales",
            Description = "Sales and business development",
            Code = "SALES",
            Location = "Business City, CA",
            CostCenter = "CC-200",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var hr = new Department
        {
            Id = hrId,
            CompanyId = demoCompanyId,
            Name = "Human Resources",
            Description = "People operations, hiring, and employee relations",
            Code = "HR",
            Location = "Business City, CA",
            CostCenter = "CC-300",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var finance = new Department
        {
            Id = financeId,
            CompanyId = demoCompanyId,
            Name = "Finance",
            Description = "Accounting, payroll, and financial planning",
            Code = "FIN",
            Location = "Business City, CA",
            CostCenter = "CC-400",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var support = new Department
        {
            Id = supportId,
            CompanyId = demoCompanyId,
            Name = "Customer Support",
            Description = "Technical support and customer success",
            Code = "ENG-SUP",
            Location = "Remote",
            CostCenter = "CC-110",
            ParentDepartmentId = engineeringId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Departments.AddRange(engineering, sales, hr, finance, support);

        // === EMPLOYEES ===
        var ceoId = Guid.NewGuid();
        var engManagerId = Guid.NewGuid();
        var dev1Id = Guid.NewGuid();
        var dev2Id = Guid.NewGuid();
        var salesManagerId = Guid.NewGuid();
        var salesRepId = Guid.NewGuid();
        var hrManagerId = Guid.NewGuid();
        var financeManagerId = Guid.NewGuid();
        var supportRepId = Guid.NewGuid();

        var ceo = new Employee
        {
            Id = ceoId,
            CompanyId = demoCompanyId,
            FirstName = "Alexandra",
            LastName = "Reyes",
            Email = "alexandra.reyes@businessasusual.demo",
            JobTitle = "Chief Executive Officer",
            EmploymentType = EmploymentType.FullTime,
            Status = EmploymentStatus.Active,
            HireDate = DateTime.UtcNow.AddYears(-6),
            WorkLocation = "Business City, CA",
            SalaryGrade = "Executive",
            CreatedAt = DateTime.UtcNow
        };

        var engManager = new Employee
        {
            Id = engManagerId,
            CompanyId = demoCompanyId,
            FirstName = "Marcus",
            LastName = "Chen",
            Email = "marcus.chen@businessasusual.demo",
            JobTitle = "Engineering Manager",
            EmploymentType = EmploymentType.FullTime,
            Status = EmploymentStatus.Active,
            HireDate = DateTime.UtcNow.AddYears(-4),
            WorkLocation = "Business City, CA",
            ManagerId = ceoId,
            SalaryGrade = "L6",
            CreatedAt = DateTime.UtcNow
        };

        var dev1 = new Employee
        {
            Id = dev1Id,
            CompanyId = demoCompanyId,
            FirstName = "Priya",
            LastName = "Patel",
            Email = "priya.patel@businessasusual.demo",
            JobTitle = "Senior Software Engineer",
            EmploymentType = EmploymentType.FullTime,
            Status = EmploymentStatus.Active,
            HireDate = DateTime.UtcNow.AddYears(-3),
            WorkLocation = "Remote",
            ManagerId = engManagerId,
            SalaryGrade = "L5",
            CreatedAt = DateTime.UtcNow
        };

        var dev2 = new Employee
        {
            Id = dev2Id,
            CompanyId = demoCompanyId,
            FirstName = "Jordan",
            LastName = "Lee",
            Email = "jordan.lee@businessasusual.demo",
            JobTitle = "Software Engineer",
            EmploymentType = EmploymentType.FullTime,
            Status = EmploymentStatus.Active,
            HireDate = DateTime.UtcNow.AddYears(-1),
            WorkLocation = "Remote",
            ManagerId = engManagerId,
            SalaryGrade = "L3",
            CreatedAt = DateTime.UtcNow
        };

        var salesManager = new Employee
        {
            Id = salesManagerId,
            CompanyId = demoCompanyId,
            FirstName = "Diana",
            LastName = "Okafor",
            Email = "diana.okafor@businessasusual.demo",
            JobTitle = "Sales Manager",
            EmploymentType = EmploymentType.FullTime,
            Status = EmploymentStatus.Active,
            HireDate = DateTime.UtcNow.AddYears(-5),
            WorkLocation = "Business City, CA",
            ManagerId = ceoId,
            SalaryGrade = "L6",
            CreatedAt = DateTime.UtcNow
        };

        var salesRep = new Employee
        {
            Id = salesRepId,
            CompanyId = demoCompanyId,
            FirstName = "Tyler",
            LastName = "Brooks",
            Email = "tyler.brooks@businessasusual.demo",
            JobTitle = "Account Executive",
            EmploymentType = EmploymentType.FullTime,
            Status = EmploymentStatus.Active,
            HireDate = DateTime.UtcNow.AddMonths(-8),
            WorkLocation = "Business City, CA",
            ManagerId = salesManagerId,
            SalaryGrade = "L3",
            CreatedAt = DateTime.UtcNow
        };

        var hrManager = new Employee
        {
            Id = hrManagerId,
            CompanyId = demoCompanyId,
            FirstName = "Samuel",
            LastName = "Ibrahim",
            Email = "samuel.ibrahim@businessasusual.demo",
            JobTitle = "HR Manager",
            EmploymentType = EmploymentType.FullTime,
            Status = EmploymentStatus.Active,
            HireDate = DateTime.UtcNow.AddYears(-4),
            WorkLocation = "Business City, CA",
            ManagerId = ceoId,
            SalaryGrade = "L5",
            CreatedAt = DateTime.UtcNow
        };

        var financeManager = new Employee
        {
            Id = financeManagerId,
            CompanyId = demoCompanyId,
            FirstName = "Grace",
            LastName = "Kim",
            Email = "grace.kim@businessasusual.demo",
            JobTitle = "Finance Manager",
            EmploymentType = EmploymentType.FullTime,
            Status = EmploymentStatus.Active,
            HireDate = DateTime.UtcNow.AddYears(-3),
            WorkLocation = "Business City, CA",
            ManagerId = ceoId,
            SalaryGrade = "L5",
            CreatedAt = DateTime.UtcNow
        };

        var supportRep = new Employee
        {
            Id = supportRepId,
            CompanyId = demoCompanyId,
            FirstName = "Noah",
            LastName = "Martinez",
            Email = "noah.martinez@businessasusual.demo",
            JobTitle = "Support Engineer",
            EmploymentType = EmploymentType.PartTime,
            Status = EmploymentStatus.Active,
            HireDate = DateTime.UtcNow.AddMonths(-5),
            WorkLocation = "Remote",
            ManagerId = engManagerId,
            SalaryGrade = "L2",
            CreatedAt = DateTime.UtcNow
        };

        context.Employees.AddRange(ceo, engManager, dev1, dev2, salesManager, salesRep, hrManager, financeManager, supportRep);

        // === EMPLOYEE <-> DEPARTMENT ASSIGNMENTS ===
        context.EmployeeDepartments.AddRange(
            new EmployeeDepartment { CompanyId = demoCompanyId, EmployeeId = ceoId, DepartmentId = engineeringId, IsPrimary = false, AllocationPercentage = 0, JoinedDate = ceo.HireDate },
            new EmployeeDepartment { CompanyId = demoCompanyId, EmployeeId = engManagerId, DepartmentId = engineeringId, IsPrimary = true, AllocationPercentage = 100, JoinedDate = engManager.HireDate },
            new EmployeeDepartment { CompanyId = demoCompanyId, EmployeeId = dev1Id, DepartmentId = engineeringId, IsPrimary = true, AllocationPercentage = 100, JoinedDate = dev1.HireDate },
            new EmployeeDepartment { CompanyId = demoCompanyId, EmployeeId = dev2Id, DepartmentId = engineeringId, IsPrimary = true, AllocationPercentage = 100, JoinedDate = dev2.HireDate },
            new EmployeeDepartment { CompanyId = demoCompanyId, EmployeeId = supportRepId, DepartmentId = supportId, IsPrimary = true, AllocationPercentage = 100, JoinedDate = supportRep.HireDate },
            new EmployeeDepartment { CompanyId = demoCompanyId, EmployeeId = salesManagerId, DepartmentId = salesId, IsPrimary = true, AllocationPercentage = 100, JoinedDate = salesManager.HireDate },
            new EmployeeDepartment { CompanyId = demoCompanyId, EmployeeId = salesRepId, DepartmentId = salesId, IsPrimary = true, AllocationPercentage = 100, JoinedDate = salesRep.HireDate },
            new EmployeeDepartment { CompanyId = demoCompanyId, EmployeeId = hrManagerId, DepartmentId = hrId, IsPrimary = true, AllocationPercentage = 100, JoinedDate = hrManager.HireDate },
            new EmployeeDepartment { CompanyId = demoCompanyId, EmployeeId = financeManagerId, DepartmentId = financeId, IsPrimary = true, AllocationPercentage = 100, JoinedDate = financeManager.HireDate }
        );

        // === DEPARTMENT MANAGERS ===
        context.DepartmentManagers.AddRange(
            new DepartmentManager { CompanyId = demoCompanyId, DepartmentId = engineeringId, ManagerId = engManagerId, ManagerRole = "Department Head", IsPrimary = true, StartDate = engManager.HireDate },
            new DepartmentManager { CompanyId = demoCompanyId, DepartmentId = supportId, ManagerId = engManagerId, ManagerRole = "Department Head", IsPrimary = true, StartDate = engManager.HireDate },
            new DepartmentManager { CompanyId = demoCompanyId, DepartmentId = salesId, ManagerId = salesManagerId, ManagerRole = "Department Head", IsPrimary = true, StartDate = salesManager.HireDate },
            new DepartmentManager { CompanyId = demoCompanyId, DepartmentId = hrId, ManagerId = hrManagerId, ManagerRole = "Department Head", IsPrimary = true, StartDate = hrManager.HireDate },
            new DepartmentManager { CompanyId = demoCompanyId, DepartmentId = financeId, ManagerId = financeManagerId, ManagerRole = "Department Head", IsPrimary = true, StartDate = financeManager.HireDate }
        );

        await context.SaveChangesAsync();
        Console.WriteLine("✅ HR demo data seeded: 5 departments, 9 employees.");
    }
}
