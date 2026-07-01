using Amazon.CloudWatch;
using ApexCharts;
using BusinessAsUsual.Infrastructure.Monitoring;
using BusinessAsUsual.Web.Modules.HR.Services;
using BusinessAsUsual.Web.Services;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

namespace BusinessAsUsual.Web
{
    /// <summary>
    /// Entry point for the BusinessAsUsual.Web application.
    /// Initializes services, configures middleware, and launches the Blazor Server runtime.
    /// </summary>
    /// <remarks>
    /// Contributor: Christopher Ruckman  
    /// Created: 2025-10-05  
    /// Tags: #startup #blazor #mudblazor #server #entrypoint  
    /// </remarks>
    public static class Program
    {
        /// <summary>
        /// Configures and starts the BusinessAsUsual.Web application.
        /// Sets up Razor Components, MudBlazor services, and request pipeline middleware.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        /// <returns>A task representing the asynchronous operation of launching the web host.</returns>
        /// <remarks>
        /// Contributor: Christopher Ruckman  
        /// Last updated: 2025-10-05  
        /// Tags: #startup #async #middleware #mudblazor #razorcomponents  
        /// </remarks>
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddMudServices();

            // Add ApexCharts for CRM reports
            builder.Services.AddApexCharts();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });

            // Register ThemeContext as a singleton (global theme state)
            builder.Services.AddSingleton<ThemeContext>();

            // Register theme sync service for iframe modules
            builder.Services.AddScoped<BusinessAsUsual.Web.Themes.ThemeSyncService>();

            // Register Scoped services
            builder.Services.AddScoped<CircuitHandler, LoggingCircuitHandler>();
            builder.Services.AddScoped<PageHeaderService>();

            // DI Registration
            builder.Services.AddScoped<IHRService, HRService>();

            // Register Module Discovery Service
            builder.Services.AddHttpClient<IModuleDiscoveryService, ModuleDiscoveryService>();

            // Register Master Navigation Orchestrator
            builder.Services.AddScoped<ModuleRouteInterceptor>();

            // Register HR Module services (for embedded HR.Web components)
            RegisterHRModuleServices(builder.Services, builder.Configuration);

            // Register CRM Module services (for embedded CRM.Web components)
            RegisterCRMModuleServices(builder.Services, builder.Configuration);

            if (builder.Environment.IsProduction())
            {
                builder.Services.AddAWSService<IAmazonCloudWatch>();
                builder.Services.AddSingleton<IMetricPublisher, CloudWatchMetricPublisher>();
            }

            builder.Services.AddControllers(); // tiny MVC controller for testing errors and metrics

            // Register Circuit Logging
            builder.Logging.SetMinimumLevel(LogLevel.Debug);
            builder.Logging.AddConsole();

            // Register Razor Components and MudBlazor services

            var app = builder.Build();

            // Seed HR data in development
            if (app.Environment.IsDevelopment())
            {
                await SeedHRDataAsync(app.Services);
            }

            if (app.Environment.IsProduction())
            {
                app.UseMiddleware<RequestMetricsMiddleware>();
            }

            // Configure middleware for production environments
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                app.UseHsts();
            }

            // Map static assets and Razor components
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAntiforgery();

            app.UseHttpsRedirection();

            app.MapControllers(); // Map MVC controllers for API endpoints for testing purposes
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            // Launch the application
            await app.RunAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Registers HR module services needed for embedded HR.Web components.
        /// </summary>
        private static void RegisterHRModuleServices(IServiceCollection services, IConfiguration configuration)
        {
            // Database configuration - use in-memory for development
            var useInMemory = configuration.GetValue<bool>("UseInMemoryDatabase", true);

            if (useInMemory)
            {
                services.AddDbContext<HR.Infrastructure.Persistence.HRDbContext>(options =>
                    options.UseInMemoryDatabase("HR_Shell"));
            }
            else
            {
                var connectionString = configuration.GetConnectionString("HRDatabase")
                    ?? "Server=localhost;Database=BusinessAsUsual_HR;Trusted_Connection=True;TrustServerCertificate=True;";
                services.AddDbContext<HR.Infrastructure.Persistence.HRDbContext>(options =>
                    options.UseSqlServer(connectionString));
            }

            // Register repositories
            services.AddScoped<HR.Domain.Repositories.IEmployeeRepository, HR.Infrastructure.Repositories.EmployeeRepository>();
            services.AddScoped<HR.Domain.Repositories.IDepartmentRepository, HR.Infrastructure.Repositories.DepartmentRepository>();

            // Register services
            services.AddScoped<HR.Application.Services.IEmployeeService, HR.Application.Services.EmployeeService>();
            services.AddScoped<HR.Application.Services.IDepartmentService, HR.Application.Services.DepartmentService>();
        }

        /// <summary>
        /// Registers CRM module services and infrastructure for embedded CRM.Web components.
        /// Configures in-memory database for development and SQL Server for production.
        /// </summary>
        private static void RegisterCRMModuleServices(IServiceCollection services, IConfiguration configuration)
        {
            // Register mock CRM services (for development/demo)
            services.AddScoped<CRM.Application.Services.ILeadService, CRM.Application.Services.MockLeadService>();
            services.AddScoped<CRM.Application.Services.IOpportunityService, CRM.Application.Services.MockOpportunityService>();
            services.AddScoped<CRM.Application.Services.ICustomerService, CRM.Application.Services.MockCustomerService>();
            services.AddScoped<CRM.Application.Interfaces.IReportService, CRM.Application.Services.ReportService>();
            services.AddScoped<CRM.Application.Interfaces.IActivityService, CRM.Application.Services.MockActivityService>();
        }

        /// <summary>
        /// Seeds sample HR data in development mode.
        /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete - intentionally using legacy fields for seed data
        private static async Task SeedHRDataAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<HR.Infrastructure.Persistence.HRDbContext>();

            // Clear existing data if needed (for development/testing)
            if (dbContext.Employees.Any() || dbContext.Departments.Any())
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

            dbContext.Employees.AddRange(employees);
            await dbContext.SaveChangesAsync();

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

            dbContext.Departments.AddRange(departments);
            await dbContext.SaveChangesAsync();

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

            dbContext.EmployeeDepartments.AddRange(employeeDepartments);
            await dbContext.SaveChangesAsync();

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

            dbContext.DepartmentManagers.AddRange(departmentManagers);
            await dbContext.SaveChangesAsync();

            Console.WriteLine("✓ Seeded comprehensive HR data:");
            Console.WriteLine("  - 7 employees with full details");
            Console.WriteLine("  - 7 departments (5 main + 2 sub-departments/pods)");
            Console.WriteLine("  - Multiple managers per department");
            Console.WriteLine("  - Employees in multiple departments");
            Console.WriteLine("  - Department hierarchy (Engineering with Frontend/Backend teams)");
        }
#pragma warning restore CS0618
    }
}
