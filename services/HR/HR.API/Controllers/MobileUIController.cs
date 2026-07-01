using HR.Contracts.Navigation;
using HR.Contracts.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

[ApiController]
[Route("api/hr/mobile")]
public class MobileUIController : ControllerBase
{
    /// <summary>
    /// Get the complete mobile UI specification for the HR module
    /// </summary>
    [HttpGet("ui-spec")]
    public ActionResult<MobileUISpecification> GetUISpecification()
    {
        var spec = new MobileUISpecification
        {
            ModuleId = "hr",
            ModuleName = "Human Resources",
            Version = "1.0.0",
            Navigation = GetNavigationMap(),
            Screens = new Dictionary<string, object>
            {
                { "employee-list", GetEmployeeListSpec() },
                { "employee-detail", GetEmployeeDetailSpec() },
                { "employee-form", GetEmployeeFormSpec() }
            }
        };

        return Ok(spec);
    }

    /// <summary>
    /// Get navigation structure for mobile app
    /// </summary>
    [HttpGet("navigation")]
    public ActionResult<HRNavigationMap> GetNavigation()
    {
        return Ok(GetNavigationMap());
    }

    /// <summary>
    /// Get employee list screen specification
    /// </summary>
    [HttpGet("ui-spec/employee-list")]
    public ActionResult<EmployeeListSpec> GetEmployeeListSpecification()
    {
        return Ok(GetEmployeeListSpec());
    }

    /// <summary>
    /// Get employee detail screen specification
    /// </summary>
    [HttpGet("ui-spec/employee-detail")]
    public ActionResult<EmployeeDetailSpec> GetEmployeeDetailSpecification()
    {
        return Ok(GetEmployeeDetailSpec());
    }

    /// <summary>
    /// Get employee form specification
    /// </summary>
    [HttpGet("ui-spec/employee-form")]
    public ActionResult<EmployeeFormSpec> GetEmployeeFormSpecification()
    {
        return Ok(GetEmployeeFormSpec());
    }

    private HRNavigationMap GetNavigationMap()
    {
        return new HRNavigationMap
        {
            ModuleId = "hr",
            ModuleName = "Human Resources",
            Icon = "person",
            Items = new List<NavigationItem>
            {
                new NavigationItem
                {
                    Id = "employees",
                    Label = "Employees",
                    Icon = "people",
                    Screen = "employee-list",
                    Route = "/hr/employees"
                },
                new NavigationItem
                {
                    Id = "onboarding",
                    Label = "Onboarding",
                    Icon = "person_add",
                    Screen = "onboarding-list",
                    Route = "/hr/onboarding"
                },
                new NavigationItem
                {
                    Id = "benefits",
                    Label = "Benefits",
                    Icon = "health_and_safety",
                    Screen = "benefits-list",
                    Route = "/hr/benefits"
                },
                new NavigationItem
                {
                    Id = "departments",
                    Label = "Departments",
                    Icon = "corporate_fare",
                    Screen = "department-list",
                    Route = "/hr/departments"
                }
            }
        };
    }

    private EmployeeListSpec GetEmployeeListSpec()
    {
        return new EmployeeListSpec
        {
            Title = "Employees",
            SearchPlaceholder = "Search employees by name...",
            EnableSearch = true,
            EnableFilter = true,
            Columns = new List<ColumnDefinition>
            {
                new ColumnDefinition { Name = "photoUrl", Label = "Photo", Type = "image", Width = 60 },
                new ColumnDefinition { Name = "fullName", Label = "Name", Type = "text", Sortable = true, Width = 200 },
                new ColumnDefinition { Name = "jobTitle", Label = "Title", Type = "text", Sortable = true, Width = 150 },
                new ColumnDefinition { Name = "department", Label = "Department", Type = "text", Sortable = true, Width = 150 },
                new ColumnDefinition { Name = "status", Label = "Status", Type = "badge", Width = 100 }
            },
            Actions = new List<ActionButton>
            {
                new ActionButton
                {
                    Id = "add",
                    Label = "Add Employee",
                    Icon = "add",
                    Action = "navigate",
                    NavigateTo = "/hr/employees/new"
                },
                new ActionButton
                {
                    Id = "view",
                    Label = "View Details",
                    Icon = "visibility",
                    Action = "navigate",
                    NavigateTo = "/hr/employees/{id}"
                },
                new ActionButton
                {
                    Id = "edit",
                    Label = "Edit",
                    Icon = "edit",
                    Action = "navigate",
                    NavigateTo = "/hr/employees/{id}/edit"
                }
            },
            Filters = new List<FilterOption>
            {
                new FilterOption
                {
                    Id = "department",
                    Label = "Department",
                    Type = "select",
                    Values = new List<FilterValue>
                    {
                        new FilterValue { Id = "all", Label = "All Departments", Value = "" },
                        new FilterValue { Id = "eng", Label = "Engineering", Value = "engineering" },
                        new FilterValue { Id = "sales", Label = "Sales", Value = "sales" },
                        new FilterValue { Id = "hr", Label = "Human Resources", Value = "hr" }
                    }
                },
                new FilterOption
                {
                    Id = "status",
                    Label = "Status",
                    Type = "select",
                    Values = new List<FilterValue>
                    {
                        new FilterValue { Id = "all", Label = "All Statuses", Value = "" },
                        new FilterValue { Id = "active", Label = "Active", Value = "active" },
                        new FilterValue { Id = "onboarding", Label = "Onboarding", Value = "onboarding" },
                        new FilterValue { Id = "inactive", Label = "Inactive", Value = "inactive" }
                    }
                }
            },
            EmptyStateMessage = "No employees found. Tap + to add your first employee."
        };
    }

    private EmployeeDetailSpec GetEmployeeDetailSpec()
    {
        return new EmployeeDetailSpec
        {
            Title = "Employee Details",
            Sections = new List<SectionDefinition>
            {
                new SectionDefinition
                {
                    Id = "personal",
                    Title = "Personal Information",
                    Fields = new List<FieldDefinition>
                    {
                        new FieldDefinition { Name = "photoUrl", Label = "Photo", Type = "image" },
                        new FieldDefinition { Name = "fullName", Label = "Full Name", Type = "text" },
                        new FieldDefinition { Name = "email", Label = "Email", Type = "email", Icon = "email" },
                        new FieldDefinition { Name = "phoneNumber", Label = "Phone", Type = "phone", Icon = "phone" },
                        new FieldDefinition { Name = "hireDate", Label = "Hire Date", Type = "date", Format = "MMM dd, yyyy" }
                    }
                },
                new SectionDefinition
                {
                    Id = "employment",
                    Title = "Employment Details",
                    Fields = new List<FieldDefinition>
                    {
                        new FieldDefinition { Name = "jobTitle", Label = "Job Title", Type = "text" },
                        new FieldDefinition { Name = "department", Label = "Department", Type = "text" },
                        new FieldDefinition { Name = "status", Label = "Status", Type = "badge" }
                    }
                }
            },
            Actions = new List<ActionButton>
            {
                new ActionButton
                {
                    Id = "edit",
                    Label = "Edit",
                    Icon = "edit",
                    Action = "navigate",
                    NavigateTo = "/hr/employees/{id}/edit"
                },
                new ActionButton
                {
                    Id = "deactivate",
                    Label = "Deactivate",
                    Icon = "block",
                    Action = "api-call",
                    ApiEndpoint = "/api/employees/{id}/deactivate",
                    RequiresConfirmation = true,
                    ConfirmationMessage = "Are you sure you want to deactivate this employee?"
                }
            }
        };
    }

    private EmployeeFormSpec GetEmployeeFormSpec()
    {
        return new EmployeeFormSpec
        {
            Title = "Employee Information",
            Sections = new List<FormSectionDefinition>
            {
                new FormSectionDefinition
                {
                    Id = "personal",
                    Title = "Personal Information",
                    Fields = new List<FormFieldDefinition>
                    {
                        new FormFieldDefinition
                        {
                            Name = "firstName",
                            Label = "First Name",
                            Type = "text",
                            Required = true,
                            MaxLength = 50,
                            ValidationMessage = "First name is required"
                        },
                        new FormFieldDefinition
                        {
                            Name = "lastName",
                            Label = "Last Name",
                            Type = "text",
                            Required = true,
                            MaxLength = 50,
                            ValidationMessage = "Last name is required"
                        },
                        new FormFieldDefinition
                        {
                            Name = "email",
                            Label = "Email",
                            Type = "email",
                            Required = true,
                            Pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                            ValidationMessage = "Please enter a valid email address"
                        },
                        new FormFieldDefinition
                        {
                            Name = "phoneNumber",
                            Label = "Phone Number",
                            Type = "phone",
                            Required = false,
                            Placeholder = "(555) 123-4567"
                        }
                    }
                },
                new FormSectionDefinition
                {
                    Id = "employment",
                    Title = "Employment Details",
                    Fields = new List<FormFieldDefinition>
                    {
                        new FormFieldDefinition
                        {
                            Name = "departmentId",
                            Label = "Department",
                            Type = "select",
                            Required = true,
                            Options = new List<SelectOption>
                            {
                                new SelectOption { Value = "1", Label = "Engineering" },
                                new SelectOption { Value = "2", Label = "Sales" },
                                new SelectOption { Value = "3", Label = "Human Resources" },
                                new SelectOption { Value = "4", Label = "Marketing" }
                            },
                            ValidationMessage = "Please select a department"
                        },
                        new FormFieldDefinition
                        {
                            Name = "jobTitle",
                            Label = "Job Title",
                            Type = "text",
                            Required = true,
                            MaxLength = 100,
                            ValidationMessage = "Job title is required"
                        },
                        new FormFieldDefinition
                        {
                            Name = "hireDate",
                            Label = "Hire Date",
                            Type = "date",
                            Required = true,
                            ValidationMessage = "Hire date is required"
                        }
                    }
                }
            },
            Actions = new List<ActionButton>
            {
                new ActionButton
                {
                    Id = "save",
                    Label = "Save",
                    Icon = "save",
                    Action = "api-call",
                    ApiEndpoint = "/api/employees"
                },
                new ActionButton
                {
                    Id = "cancel",
                    Label = "Cancel",
                    Icon = "close",
                    Action = "navigate",
                    NavigateTo = "/hr/employees"
                }
            },
            Validation = new ValidationRules
            {
                ErrorMessages = new Dictionary<string, string>
                {
                    { "required", "This field is required" },
                    { "email", "Please enter a valid email address" },
                    { "maxLength", "Value is too long" }
                }
            }
        };
    }
}

/// <summary>
/// Root mobile UI specification object
/// </summary>
public class MobileUISpecification
{
    public string ModuleId { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public HRNavigationMap Navigation { get; set; } = new();
    public Dictionary<string, object> Screens { get; set; } = new();
}
