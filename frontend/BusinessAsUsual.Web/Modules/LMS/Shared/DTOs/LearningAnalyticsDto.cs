namespace BusinessAsUsual.Web.Modules.LMS.Shared.DTOs;

/// <summary>
/// Overall learning analytics dashboard data
/// </summary>
public class LearningAnalyticsDashboardDto
{
    public OverallMetricsDto OverallMetrics { get; set; } = new();
    public List<CourseCompletionTrendDto> CompletionTrends { get; set; } = new();
    public List<PopularCourseDto> PopularCourses { get; set; } = new();
    public List<EngagementMetricDto> EngagementMetrics { get; set; } = new();
    public List<DepartmentPerformanceDto> DepartmentPerformance { get; set; } = new();
}

/// <summary>
/// Overall learning metrics summary
/// </summary>
public class OverallMetricsDto
{
    public int TotalEnrollments { get; set; }
    public int ActiveLearners { get; set; }
    public int CompletedCourses { get; set; }
    public decimal OverallCompletionRate { get; set; }
    public decimal AverageProgressPercentage { get; set; }
    public double AverageTimeSpentHours { get; set; }
    public int CertificatesIssued { get; set; }
}

/// <summary>
/// Course completion trend over time
/// </summary>
public class CourseCompletionTrendDto
{
    public DateTime Date { get; set; }
    public int Enrollments { get; set; }
    public int Completions { get; set; }
    public decimal CompletionRate { get; set; }
    public int ActiveLearners { get; set; }
}

/// <summary>
/// Popular course metrics
/// </summary>
public class PopularCourseDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public int EnrollmentCount { get; set; }
    public int CompletionCount { get; set; }
    public decimal CompletionRate { get; set; }
    public double AverageRating { get; set; }
    public double AverageTimeToCompleteHours { get; set; }
}

/// <summary>
/// Engagement metrics by time period
/// </summary>
public class EngagementMetricDto
{
    public DateTime Period { get; set; }
    public int ActiveUsers { get; set; }
    public int SessionCount { get; set; }
    public double AverageSessionDurationMinutes { get; set; }
    public int DropoffCount { get; set; }
    public decimal DropoffRate { get; set; }
}

/// <summary>
/// Department performance comparison
/// </summary>
public class DepartmentPerformanceDto
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int TotalEmployees { get; set; }
    public int ActiveLearners { get; set; }
    public int CompletedCourses { get; set; }
    public decimal CompletionRate { get; set; }
    public double AverageProgressPercentage { get; set; }
    public int CertificatesEarned { get; set; }
}
