using HR.Application.DTOs;
using HR.Application.Services;
using HR.Contracts.Navigation;
using HR.Contracts.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace HR.API.Controllers;

[ApiController]
[Route("api/hr/mobile")]
public class MobileUIController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public MobileUIController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

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
                { "employee-form", GetEmployeeFormSpec() },
                { "department-list", GetDepartmentListSpec() },
                { "applicant-list", GetApplicantListSpec() },
                { "interview-list", GetInterviewListSpec() },
                { "review-list", GetReviewListSpec() },
                { "goal-list", GetGoalListSpec() },
                { "course-list", GetCourseListSpec() },
                { "certification-list", GetCertificationListSpec() },
                { "timesheet-list", GetTimesheetListSpec() },
                { "timeoff-list", GetTimeOffListSpec() },
                { "onboarding-list", GetOnboardingListSpec() },
                { "benefits-list", GetBenefitsListSpec() },
                { "compensation-list", GetCompensationListSpec() },
                { "report-list", GetReportListSpec() },
                { "report-dashboard", GetReportDashboardSpec() }
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

    /// <summary>
    /// Get row data for a given mobile list screen. Rows are dictionaries keyed
    /// by the column Name values defined in that screen's specification.
    /// Employee data is live; other screens return representative sample rows.
    /// </summary>
    [HttpGet("data/{screenKey}")]
    public async Task<ActionResult<List<Dictionary<string, string>>>> GetScreenData(string screenKey)
    {
        var rows = screenKey switch
        {
            "employee-list" => await GetEmployeeRows(),
            "department-list" => GetDepartmentRows(),
            "onboarding-list" => GetOnboardingRows(),
            "benefits-list" => GetBenefitsRows(),
            "applicant-list" => GetApplicantRows(),
            "interview-list" => GetInterviewRows(),
            "review-list" => GetReviewRows(),
            "goal-list" => GetGoalRows(),
            "course-list" => GetCourseRows(),
            "certification-list" => GetCertificationRows(),
            "timesheet-list" => GetTimesheetRows(),
            "timeoff-list" => GetTimeOffRows(),
            "compensation-list" => GetCompensationRows(),
            "report-list" => GetReportRows(),
            _ => new List<Dictionary<string, string>>()
        };

        return Ok(rows);
    }

    private async Task<List<Dictionary<string, string>>> GetEmployeeRows()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();
        var rows = employees.Select(e => new Dictionary<string, string>
        {
            ["fullName"] = e.FullName,
            ["jobTitle"] = e.JobTitle ?? string.Empty,
            ["department"] = e.Department ?? string.Empty,
            ["email"] = e.Email ?? string.Empty,
            ["phone"] = e.PhoneNumber ?? string.Empty,
            ["hireDate"] = e.HireDate == default ? string.Empty : e.HireDate.ToString("yyyy-MM-dd"),
            ["status"] = e.Status
        }).ToList();

        // Fallback sample roster so the flagship screen is never empty in a demo environment
        // (HR.API uses an in-memory database that is not seeded on its own).
        if (rows.Count == 0)
        {
            rows = Rows(
                new() { ["fullName"] = "Alice Johnson", ["jobTitle"] = "Engineering Manager", ["department"] = "Engineering", ["email"] = "alice.johnson@bau.work", ["phone"] = "(415) 555-0102", ["hireDate"] = "2021-03-15", ["status"] = "Active" },
                new() { ["fullName"] = "Marcus Lee", ["jobTitle"] = "Account Executive", ["department"] = "Sales", ["email"] = "marcus.lee@bau.work", ["phone"] = "(415) 555-0148", ["hireDate"] = "2022-06-01", ["status"] = "Active" },
                new() { ["fullName"] = "Priya Nair", ["jobTitle"] = "HR Business Partner", ["department"] = "Human Resources", ["email"] = "priya.nair@bau.work", ["phone"] = "(415) 555-0177", ["hireDate"] = "2020-11-09", ["status"] = "Active" },
                new() { ["fullName"] = "Dana White", ["jobTitle"] = "Marketing Lead", ["department"] = "Marketing", ["email"] = "dana.white@bau.work", ["phone"] = "(415) 555-0190", ["hireDate"] = "2023-01-23", ["status"] = "Active" },
                new() { ["fullName"] = "Nina Patel", ["jobTitle"] = "Software Engineer", ["department"] = "Engineering", ["email"] = "nina.patel@bau.work", ["phone"] = "(415) 555-0133", ["hireDate"] = "2026-07-08", ["status"] = "Onboarding" },
                new() { ["fullName"] = "Jordan Reyes", ["jobTitle"] = "Sales Development Rep", ["department"] = "Sales", ["email"] = "jordan.reyes@bau.work", ["phone"] = "(415) 555-0125", ["hireDate"] = "2026-07-15", ["status"] = "Onboarding" }
            );
        }

        return rows;
    }

    private static List<Dictionary<string, string>> Rows(params Dictionary<string, string>[] rows) => rows.ToList();

    private static List<Dictionary<string, string>> GetDepartmentRows() => Rows(
        new() { ["name"] = "Engineering", ["manager"] = "Alice Johnson", ["location"] = "San Francisco", ["headcount"] = "42", ["openRoles"] = "5" },
        new() { ["name"] = "Sales", ["manager"] = "Marcus Lee", ["location"] = "New York", ["headcount"] = "28", ["openRoles"] = "3" },
        new() { ["name"] = "Human Resources", ["manager"] = "Priya Nair", ["location"] = "San Francisco", ["headcount"] = "9", ["openRoles"] = "1" },
        new() { ["name"] = "Marketing", ["manager"] = "Dana White", ["location"] = "Remote", ["headcount"] = "15", ["openRoles"] = "2" }
    );

    private static List<Dictionary<string, string>> GetOnboardingRows() => Rows(
        new() { ["employeeName"] = "Jordan Reyes", ["jobTitle"] = "Sales Development Rep", ["startDate"] = "2026-07-15", ["buddy"] = "Marcus Lee", ["progress"] = "60%", ["status"] = "In Progress" },
        new() { ["employeeName"] = "Sam Carter", ["jobTitle"] = "Support Specialist", ["startDate"] = "2026-07-21", ["buddy"] = "Priya Nair", ["progress"] = "20%", ["status"] = "In Progress" },
        new() { ["employeeName"] = "Nina Patel", ["jobTitle"] = "Software Engineer", ["startDate"] = "2026-07-08", ["buddy"] = "Alice Johnson", ["progress"] = "100%", ["status"] = "Complete" }
    );

    private static List<Dictionary<string, string>> GetBenefitsRows() => Rows(
        new() { ["planName"] = "Premium Health PPO", ["category"] = "Medical", ["provider"] = "BlueCross", ["cost"] = "$420", ["enrolled"] = "128", ["status"] = "Active" },
        new() { ["planName"] = "Vision Plus", ["category"] = "Vision", ["provider"] = "VSP", ["cost"] = "$18", ["enrolled"] = "96", ["status"] = "Active" },
        new() { ["planName"] = "401(k) Match", ["category"] = "Retirement", ["provider"] = "Fidelity", ["cost"] = "—", ["enrolled"] = "141", ["status"] = "Active" }
    );

    private static List<Dictionary<string, string>> GetApplicantRows() => Rows(
        new() { ["name"] = "Taylor Brooks", ["position"] = "Senior Engineer", ["source"] = "Referral", ["appliedDate"] = "2026-07-01", ["stage"] = "Interview", ["rating"] = "4.5", ["status"] = "Active" },
        new() { ["name"] = "Chris Nguyen", ["position"] = "Account Executive", ["source"] = "LinkedIn", ["appliedDate"] = "2026-07-03", ["stage"] = "Screening", ["rating"] = "4.0", ["status"] = "Active" },
        new() { ["name"] = "Morgan Ellis", ["position"] = "UX Designer", ["source"] = "Careers Page", ["appliedDate"] = "2026-06-28", ["stage"] = "Offer", ["rating"] = "4.8", ["status"] = "Active" }
    );

    private static List<Dictionary<string, string>> GetInterviewRows() => Rows(
        new() { ["candidate"] = "Taylor Brooks", ["interviewer"] = "Alice Johnson", ["date"] = "2026-07-14", ["time"] = "10:00 AM", ["type"] = "Technical", ["status"] = "Scheduled" },
        new() { ["candidate"] = "Morgan Ellis", ["interviewer"] = "Dana White", ["date"] = "2026-07-16", ["time"] = "2:30 PM", ["type"] = "Portfolio", ["status"] = "Scheduled" }
    );

    private static List<Dictionary<string, string>> GetReviewRows() => Rows(
        new() { ["employeeName"] = "Alice Johnson", ["reviewer"] = "CTO", ["period"] = "H1 2026", ["rating"] = "Exceeds", ["dueDate"] = "2026-07-01", ["status"] = "Complete" },
        new() { ["employeeName"] = "Marcus Lee", ["reviewer"] = "VP Sales", ["period"] = "H1 2026", ["rating"] = "Meets", ["dueDate"] = "2026-06-15", ["status"] = "Overdue" },
        new() { ["employeeName"] = "Priya Nair", ["reviewer"] = "CEO", ["period"] = "H1 2026", ["rating"] = "—", ["dueDate"] = "2026-07-20", ["status"] = "Pending" }
    );

    private static List<Dictionary<string, string>> GetGoalRows() => Rows(
        new() { ["employeeName"] = "Alice Johnson", ["goal"] = "Ship mobile parity", ["category"] = "Delivery", ["progress"] = "80%", ["dueDate"] = "2026-08-01", ["status"] = "On Track" },
        new() { ["employeeName"] = "Marcus Lee", ["goal"] = "Grow pipeline 20%", ["category"] = "Revenue", ["progress"] = "55%", ["dueDate"] = "2026-09-30", ["status"] = "At Risk" }
    );

    private static List<Dictionary<string, string>> GetCourseRows() => Rows(
        new() { ["title"] = "Security Awareness", ["category"] = "Compliance", ["format"] = "Online", ["duration"] = "1h", ["enrolled"] = "150", ["status"] = "Active" },
        new() { ["title"] = "Leadership 101", ["category"] = "Development", ["format"] = "Workshop", ["duration"] = "4h", ["enrolled"] = "36", ["status"] = "Active" },
        new() { ["title"] = "Kotlin for Teams", ["category"] = "Technical", ["format"] = "Self-paced", ["duration"] = "6h", ["enrolled"] = "22", ["status"] = "Active" }
    );

    private static List<Dictionary<string, string>> GetCertificationRows() => Rows(
        new() { ["employeeName"] = "Alice Johnson", ["certification"] = "AWS Solutions Architect", ["issuer"] = "Amazon", ["issued"] = "2024-03-01", ["expires"] = "2027-03-01", ["status"] = "Valid" },
        new() { ["employeeName"] = "Sam Carter", ["certification"] = "PMP", ["issuer"] = "PMI", ["issued"] = "2023-11-15", ["expires"] = "2026-11-15", ["status"] = "Expiring" }
    );

    private static List<Dictionary<string, string>> GetTimesheetRows() => Rows(
        new() { ["employeeName"] = "Marcus Lee", ["week"] = "Jul 7–13", ["regularHours"] = "40", ["overtimeHours"] = "0", ["hours"] = "40", ["status"] = "Pending" },
        new() { ["employeeName"] = "Dana White", ["week"] = "Jul 7–13", ["regularHours"] = "38", ["overtimeHours"] = "0", ["hours"] = "38", ["status"] = "Approved" }
    );

    private static List<Dictionary<string, string>> GetTimeOffRows() => Rows(
        new() { ["employeeName"] = "Nina Patel", ["type"] = "Vacation", ["dates"] = "Jul 20–24", ["days"] = "5", ["approver"] = "Alice Johnson", ["status"] = "Approved" },
        new() { ["employeeName"] = "Jordan Reyes", ["type"] = "Sick", ["dates"] = "Jul 11", ["days"] = "1", ["approver"] = "Marcus Lee", ["status"] = "Pending" }
    );

    private static List<Dictionary<string, string>> GetCompensationRows() => Rows(
        new() { ["employeeName"] = "Alice Johnson", ["grade"] = "L6", ["base"] = "$165,000", ["bonus"] = "$25,000", ["lastReview"] = "2026-01-15", ["nextReview"] = "2026-07-15" },
        new() { ["employeeName"] = "Marcus Lee", ["grade"] = "L5", ["base"] = "$132,000", ["bonus"] = "$18,000", ["lastReview"] = "2026-02-01", ["nextReview"] = "2026-08-01" }
    );

    private static List<Dictionary<string, string>> GetReportRows() => Rows(
        new() { ["name"] = "Headcount Report", ["category"] = "Workforce", ["owner"] = "Priya Nair", ["updated"] = "2026-07-10", ["status"] = "Ready" },
        new() { ["name"] = "Turnover Analysis", ["category"] = "Retention", ["owner"] = "Priya Nair", ["updated"] = "2026-07-09", ["status"] = "Ready" },
        new() { ["name"] = "Diversity Snapshot", ["category"] = "DEI", ["owner"] = "Dana White", ["updated"] = "2026-07-05", ["status"] = "Draft" }
    );

    private HRNavigationMap GetNavigationMap()
    {
        return new HRNavigationMap
        {
            ModuleId = "hr",
            ModuleName = "Human Resources",
            Icon = "person",
            Items = new List<NavigationItem>
            {
                new NavigationItem { Id = "employees", Label = "Employees", Icon = "people", Screen = "employee-list", Route = "/hr/employees" },
                new NavigationItem { Id = "departments", Label = "Departments", Icon = "corporate_fare", Screen = "department-list", Route = "/hr/departments" },
                new NavigationItem { Id = "applicants", Label = "Recruiting", Icon = "person_search", Screen = "applicant-list", Route = "/hr/applicants" },
                new NavigationItem { Id = "interviews", Label = "Interviews", Icon = "event", Screen = "interview-list", Route = "/hr/interviews" },
                new NavigationItem { Id = "reviews", Label = "Reviews", Icon = "assessment", Screen = "review-list", Route = "/hr/reviews" },
                new NavigationItem { Id = "goals", Label = "Goals", Icon = "flag", Screen = "goal-list", Route = "/hr/goals" },
                new NavigationItem { Id = "courses", Label = "Training", Icon = "school", Screen = "course-list", Route = "/hr/courses" },
                new NavigationItem { Id = "certifications", Label = "Certifications", Icon = "workspace_premium", Screen = "certification-list", Route = "/hr/certifications" },
                new NavigationItem { Id = "timesheets", Label = "Timesheets", Icon = "schedule", Screen = "timesheet-list", Route = "/hr/timesheets" },
                new NavigationItem { Id = "timeoff", Label = "Time Off", Icon = "beach_access", Screen = "timeoff-list", Route = "/hr/timeoff" },
                new NavigationItem { Id = "onboarding", Label = "Onboarding", Icon = "person_add", Screen = "onboarding-list", Route = "/hr/onboarding" },
                new NavigationItem { Id = "benefits", Label = "Benefits", Icon = "health_and_safety", Screen = "benefits-list", Route = "/hr/benefits" },
                new NavigationItem { Id = "compensation", Label = "Compensation", Icon = "payments", Screen = "compensation-list", Route = "/hr/compensation" },
                new NavigationItem { Id = "reports", Label = "Reports", Icon = "bar_chart", Screen = "report-list", Route = "/hr/reports" },
                new NavigationItem { Id = "analytics", Label = "Analytics", Icon = "insights", Screen = "report-dashboard", Route = "/hr/analytics" }
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
                new ColumnDefinition { Name = "fullName", Label = "Name", Type = "text", Sortable = true, Width = 200 },
                new ColumnDefinition { Name = "jobTitle", Label = "Title", Type = "text", Sortable = true, Width = 180 },
                new ColumnDefinition { Name = "department", Label = "Department", Type = "text", Sortable = true, Width = 160 },
                new ColumnDefinition { Name = "email", Label = "Email", Type = "text", Width = 220 },
                new ColumnDefinition { Name = "phone", Label = "Phone", Type = "text", Width = 150 },
                new ColumnDefinition { Name = "hireDate", Label = "Hire Date", Type = "date", Sortable = true, Width = 130 },
                new ColumnDefinition { Name = "status", Label = "Status", Type = "badge", Width = 120 }
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
                },
                new ActionButton
                {
                    Id = "delete",
                    Label = "Delete",
                    Icon = "delete",
                    Action = "api-call",
                    ApiEndpoint = "/api/hr/employees/{id}",
                    RequiresConfirmation = true,
                    ConfirmationMessage = "Delete this employee? This action cannot be undone."
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

    // ---- Web-parity list screens (shared helper keeps specs concise) ----
    private static EmployeeListSpec ListSpec(string title, string searchPlaceholder, string addLabel, string addRoute, string emptyMessage, ActionButton[] rowActions, params ColumnDefinition[] columns)
    {
        var actions = new List<ActionButton>
        {
            new ActionButton { Id = "add", Label = addLabel, Icon = "add", Action = "navigate", NavigateTo = addRoute }
        };
        actions.AddRange(rowActions);

        return new EmployeeListSpec
        {
            Title = title,
            SearchPlaceholder = searchPlaceholder,
            EnableSearch = true,
            EnableFilter = false,
            Columns = columns.ToList(),
            Actions = actions,
            EmptyStateMessage = emptyMessage
        };
    }

    private static ColumnDefinition Col(string name, string label, string type = "text", int width = 150, bool sortable = false)
        => new ColumnDefinition { Name = name, Label = label, Type = type, Width = width, Sortable = sortable };

    // ---- Per-row action factories (rendered in the Android overflow menu) ----
    // Convention: any action whose Id != "add" is treated as a per-row action.
    private static ActionButton RowView(string baseRoute) => new ActionButton
    { Id = "view", Label = "View Details", Icon = "visibility", Action = "navigate", NavigateTo = $"{baseRoute}/{{id}}" };

    private static ActionButton RowEdit(string baseRoute) => new ActionButton
    { Id = "edit", Label = "Edit", Icon = "edit", Action = "navigate", NavigateTo = $"{baseRoute}/{{id}}/edit" };

    private static ActionButton RowDelete(string baseRoute, string noun) => new ActionButton
    {
        Id = "delete", Label = "Delete", Icon = "delete", Action = "api-call",
        ApiEndpoint = $"{baseRoute}/{{id}}", RequiresConfirmation = true,
        ConfirmationMessage = $"Delete this {noun}? This action cannot be undone."
    };

    private static ActionButton RowApprove(string baseRoute) => new ActionButton
    {
        Id = "approve", Label = "Approve", Icon = "check_circle", Action = "api-call",
        ApiEndpoint = $"{baseRoute}/{{id}}/approve", RequiresConfirmation = true,
        ConfirmationMessage = "Approve this request?"
    };

    private static ActionButton RowReject(string baseRoute) => new ActionButton
    {
        Id = "reject", Label = "Reject", Icon = "cancel", Action = "api-call",
        ApiEndpoint = $"{baseRoute}/{{id}}/reject", RequiresConfirmation = true,
        ConfirmationMessage = "Reject this request?"
    };

    /// <summary>Standard View / Edit / Delete row actions for a base route (e.g. "/hr/goals").</summary>
    private static ActionButton[] StdRowActions(string baseRoute, string noun) => new[]
    {
        RowView(baseRoute), RowEdit(baseRoute), RowDelete(baseRoute, noun)
    };

    /// <summary>View / Approve / Reject / Edit / Delete for records that flow through an approval.</summary>
    private static ActionButton[] ApprovableRowActions(string baseRoute, string noun) => new[]
    {
        RowView(baseRoute), RowApprove(baseRoute), RowReject(baseRoute), RowEdit(baseRoute), RowDelete(baseRoute, noun)
    };

    private EmployeeListSpec GetApplicantListSpec() => ListSpec(
        "Recruiting", "Search applicants...", "New Applicant", "/hr/applicants/new", "No active applicants.",
        ApprovableRowActions("/hr/applicants", "applicant"),
        Col("name", "Candidate", width: 200, sortable: true), Col("position", "Position", width: 180),
        Col("source", "Source", width: 130), Col("appliedDate", "Applied", "date", 130),
        Col("stage", "Stage", width: 130), Col("rating", "Rating", width: 100),
        Col("status", "Status", "badge", 110));

    private EmployeeListSpec GetInterviewListSpec() => ListSpec(
        "Interviews", "Search interviews...", "Schedule", "/hr/interviews/new", "No interviews scheduled.",
        ApprovableRowActions("/hr/interviews", "interview"),
        Col("candidate", "Candidate", width: 180, sortable: true), Col("interviewer", "Interviewer", width: 160),
        Col("date", "Date", "date", 130), Col("time", "Time", width: 100),
        Col("type", "Type", width: 130), Col("status", "Status", "badge", 110));

    private EmployeeListSpec GetReviewListSpec() => ListSpec(
        "Performance Reviews", "Search reviews...", "New Review", "/hr/reviews/new", "No reviews found.",
        ApprovableRowActions("/hr/reviews", "review"),
        Col("employeeName", "Employee", width: 200, sortable: true), Col("reviewer", "Reviewer", width: 180),
        Col("period", "Period", width: 120), Col("rating", "Rating", width: 120),
        Col("dueDate", "Due", "date", 130), Col("status", "Status", "badge", 110));

    private EmployeeListSpec GetGoalListSpec() => ListSpec(
        "Goals", "Search goals...", "New Goal", "/hr/goals/new", "No goals set.",
        StdRowActions("/hr/goals", "goal"),
        Col("employeeName", "Employee", width: 180, sortable: true), Col("goal", "Goal", width: 240),
        Col("category", "Category", width: 140), Col("progress", "Progress", width: 110),
        Col("dueDate", "Due", "date", 130), Col("status", "Status", "badge", 110));

    private EmployeeListSpec GetCourseListSpec() => ListSpec(
        "Training Courses", "Search courses...", "Add Course", "/hr/courses/new", "No courses in the catalog.",
        StdRowActions("/hr/courses", "course"),
        Col("title", "Course", width: 220, sortable: true), Col("category", "Category", width: 150),
        Col("format", "Format", width: 120), Col("duration", "Duration", width: 100),
        Col("enrolled", "Enrolled", width: 100), Col("status", "Status", "badge", 110));

    private EmployeeListSpec GetCertificationListSpec() => ListSpec(
        "Certifications", "Search certifications...", "Add Certification", "/hr/certifications/new", "No certifications tracked.",
        StdRowActions("/hr/certifications", "certification"),
        Col("employeeName", "Employee", width: 200, sortable: true), Col("certification", "Certification", width: 220),
        Col("issuer", "Issuer", width: 160), Col("issued", "Issued", "date", 130),
        Col("expires", "Expires", "date", 130), Col("status", "Status", "badge", 110));

    private EmployeeListSpec GetTimesheetListSpec() => ListSpec(
        "Timesheets", "Search timesheets...", "New Entry", "/hr/timesheets/new", "No timesheets to review.",
        ApprovableRowActions("/hr/timesheets", "timesheet"),
        Col("employeeName", "Employee", width: 200, sortable: true), Col("week", "Week", width: 140),
        Col("regularHours", "Regular", width: 100), Col("overtimeHours", "Overtime", width: 100),
        Col("hours", "Total", width: 90), Col("status", "Status", "badge", 110));

    private EmployeeListSpec GetTimeOffListSpec() => ListSpec(
        "Time Off", "Search requests...", "Request Time Off", "/hr/timeoff/new", "No time-off requests.",
        ApprovableRowActions("/hr/timeoff", "request"),
        Col("employeeName", "Employee", width: 200, sortable: true), Col("type", "Type", width: 120),
        Col("dates", "Dates", width: 160), Col("days", "Days", width: 80),
        Col("approver", "Approver", width: 170), Col("status", "Status", "badge", 110));

    private EmployeeListSpec GetCompensationListSpec() => ListSpec(
        "Compensation", "Search compensation...", "Adjust", "/hr/compensation/new", "No compensation records.",
        StdRowActions("/hr/compensation", "record"),
        Col("employeeName", "Employee", width: 200, sortable: true), Col("grade", "Grade", width: 100),
        Col("base", "Base", width: 130), Col("bonus", "Bonus", width: 120),
        Col("lastReview", "Last Review", "date", 140), Col("nextReview", "Next Review", "date", 140));

    private EmployeeListSpec GetReportListSpec() => ListSpec(
        "Reports & Analytics", "Search reports...", "New Report", "/hr/reports/new", "No reports available.",
        StdRowActions("/hr/reports", "report"),
        Col("name", "Report", width: 220, sortable: true), Col("category", "Category", width: 160),
        Col("owner", "Owner", width: 170), Col("updated", "Updated", "date", 140),
        Col("status", "Status", "badge", 110));

    // ---- Analytics dashboard (contract-driven charts) ----
    private ChartScreenSpec GetReportDashboardSpec()
    {
        return new ChartScreenSpec
        {
            Title = "HR Analytics",
            EmptyStateMessage = "No analytics available.",
            Charts = new List<ChartSpec>
            {
                new ChartSpec
                {
                    Id = "headcount-trend",
                    Title = "Headcount Trend",
                    Subtitle = "Total employees over the last 6 months",
                    ChartType = "line",
                    Labels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                    Series = new List<ChartSeries>
                    {
                        new ChartSeries
                        {
                            Name = "Headcount",
                            Color = "#1B5E20",
                            Points = new List<ChartDataPoint>
                            {
                                new() { Value = 182 }, new() { Value = 188 }, new() { Value = 191 },
                                new() { Value = 196 }, new() { Value = 203 }, new() { Value = 210 }
                            }
                        }
                    }
                },
                new ChartSpec
                {
                    Id = "hires-vs-attrition",
                    Title = "Hires vs. Attrition",
                    Subtitle = "Monthly comparison",
                    ChartType = "bar",
                    Labels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                    Series = new List<ChartSeries>
                    {
                        new ChartSeries
                        {
                            Name = "Hires", Color = "#1565C0",
                            Points = new List<ChartDataPoint>
                            {
                                new() { Value = 12 }, new() { Value = 9 }, new() { Value = 7 },
                                new() { Value = 11 }, new() { Value = 14 }, new() { Value = 10 }
                            }
                        },
                        new ChartSeries
                        {
                            Name = "Attrition", Color = "#B3261E",
                            Points = new List<ChartDataPoint>
                            {
                                new() { Value = 4 }, new() { Value = 3 }, new() { Value = 5 },
                                new() { Value = 2 }, new() { Value = 6 }, new() { Value = 3 }
                            }
                        }
                    }
                },
                new ChartSpec
                {
                    Id = "department-distribution",
                    Title = "Department Distribution",
                    Subtitle = "Share of headcount by department",
                    ChartType = "donut",
                    Series = new List<ChartSeries>
                    {
                        new ChartSeries
                        {
                            Name = "Departments",
                            Points = new List<ChartDataPoint>
                            {
                                new() { Label = "Engineering", Value = 84, Color = "#1565C0" },
                                new() { Label = "Sales", Value = 46, Color = "#1B5E20" },
                                new() { Label = "Marketing", Value = 28, Color = "#B26A00" },
                                new() { Label = "Human Resources", Value = 22, Color = "#6A1B9A" },
                                new() { Label = "Operations", Value = 30, Color = "#00838F" }
                            }
                        }
                    }
                },
                new ChartSpec
                {
                    Id = "engagement-score",
                    Title = "Engagement Score",
                    Subtitle = "Weekly pulse survey (0-100)",
                    ChartType = "sparkline",
                    Series = new List<ChartSeries>
                    {
                        new ChartSeries
                        {
                            Name = "Engagement", Color = "#00838F",
                            Points = new List<ChartDataPoint>
                            {
                                new() { Value = 72 }, new() { Value = 74 }, new() { Value = 71 },
                                new() { Value = 76 }, new() { Value = 78 }, new() { Value = 77 },
                                new() { Value = 80 }, new() { Value = 82 }
                            }
                        }
                    }
                }
            }
        };
    }

    private EmployeeListSpec GetOnboardingListSpec()
    {
        return new EmployeeListSpec
        {
            Title = "Onboarding",
            SearchPlaceholder = "Search onboarding tasks...",
            EnableSearch = true,
            EnableFilter = false,
            Columns = new List<ColumnDefinition>
            {
                new ColumnDefinition { Name = "employeeName", Label = "Employee", Type = "text", Sortable = true, Width = 200 },
                new ColumnDefinition { Name = "jobTitle", Label = "Title", Type = "text", Width = 180 },
                new ColumnDefinition { Name = "startDate", Label = "Start Date", Type = "date", Sortable = true, Width = 150 },
                new ColumnDefinition { Name = "buddy", Label = "Buddy", Type = "text", Width = 170 },
                new ColumnDefinition { Name = "progress", Label = "Progress", Type = "text", Width = 120 },
                new ColumnDefinition { Name = "status", Label = "Status", Type = "badge", Width = 120 }
            },
            Actions = new List<ActionButton>
            {
                new ActionButton { Id = "add", Label = "New Onboarding", Icon = "add", Action = "navigate", NavigateTo = "/hr/onboarding/new" },
                RowView("/hr/onboarding"), RowEdit("/hr/onboarding"), RowDelete("/hr/onboarding", "onboarding record")
            },
            EmptyStateMessage = "No onboarding tasks in progress."
        };
    }

    private EmployeeListSpec GetBenefitsListSpec()
    {
        return new EmployeeListSpec
        {
            Title = "Benefits",
            SearchPlaceholder = "Search benefit plans...",
            EnableSearch = true,
            EnableFilter = false,
            Columns = new List<ColumnDefinition>
            {
                new ColumnDefinition { Name = "planName", Label = "Plan", Type = "text", Sortable = true, Width = 200 },
                new ColumnDefinition { Name = "category", Label = "Category", Type = "text", Sortable = true, Width = 150 },
                new ColumnDefinition { Name = "provider", Label = "Provider", Type = "text", Width = 170 },
                new ColumnDefinition { Name = "cost", Label = "Monthly Cost", Type = "text", Width = 130 },
                new ColumnDefinition { Name = "enrolled", Label = "Enrolled", Type = "text", Width = 110 },
                new ColumnDefinition { Name = "status", Label = "Status", Type = "badge", Width = 120 }
            },
            Actions = new List<ActionButton>
            {
                new ActionButton { Id = "add", Label = "Add Plan", Icon = "add", Action = "navigate", NavigateTo = "/hr/benefits/new" },
                RowView("/hr/benefits"), RowEdit("/hr/benefits"), RowDelete("/hr/benefits", "benefit plan")
            },
            EmptyStateMessage = "No benefit plans configured."
        };
    }

    private EmployeeListSpec GetDepartmentListSpec()
    {
        return new EmployeeListSpec
        {
            Title = "Departments",
            SearchPlaceholder = "Search departments...",
            EnableSearch = true,
            EnableFilter = false,
            Columns = new List<ColumnDefinition>
            {
                new ColumnDefinition { Name = "name", Label = "Department", Type = "text", Sortable = true, Width = 200 },
                new ColumnDefinition { Name = "manager", Label = "Manager", Type = "text", Sortable = true, Width = 180 },
                new ColumnDefinition { Name = "location", Label = "Location", Type = "text", Width = 160 },
                new ColumnDefinition { Name = "headcount", Label = "Headcount", Type = "text", Width = 120 },
                new ColumnDefinition { Name = "openRoles", Label = "Open Roles", Type = "text", Width = 120 }
            },
            Actions = new List<ActionButton>
            {
                new ActionButton { Id = "add", Label = "Add Department", Icon = "add", Action = "navigate", NavigateTo = "/hr/departments/new" },
                RowView("/hr/departments"), RowEdit("/hr/departments"), RowDelete("/hr/departments", "department")
            },
            EmptyStateMessage = "No departments found."
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
