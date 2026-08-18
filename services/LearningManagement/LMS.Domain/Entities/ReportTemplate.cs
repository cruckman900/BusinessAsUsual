namespace LMS.Domain.Entities;

/// <summary>
/// Represents a custom report template
/// </summary>
public class ReportTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ReportType Type { get; set; }
    public string QueryDefinition { get; set; } = "{}"; // JSON
    public string Columns { get; set; } = "[]"; // JSON array
    public string Filters { get; set; } = "[]"; // JSON array
    public bool IsActive { get; set; } = true;
    public string CreatedBy { get; set; } = string.Empty;
}

public enum ReportType
{
    CourseCompletions,
    QuizPerformance,
    LearnerProgress,
    CertificateReport,
    EngagementReport,
    Custom
}
