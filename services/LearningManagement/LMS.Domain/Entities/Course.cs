namespace LMS.Domain.Entities;

/// <summary>
/// Represents a training course
/// </summary>
public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public CourseDifficulty Difficulty { get; set; } = CourseDifficulty.Beginner;
    public int EstimatedDurationMinutes { get; set; }
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = new();
    public DateTime? PublishedDate { get; set; }
    public string? PublishedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? LastModifiedBy { get; set; }

    // Assessment settings
    public bool RequiresAssessment { get; set; } = true;
    public int PassingScore { get; set; } = 70; // Percentage
    public int MaxAttempts { get; set; } = 3;

    // Certificate settings
    public bool IssuesCertificate { get; set; } = true;
    public int? CertificateValidityDays { get; set; } // null = no expiry

    // Navigation properties
    public ICollection<Module> Modules { get; set; } = new List<Module>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public ICollection<CourseCompletion> Completions { get; set; } = new List<CourseCompletion>();
}

public enum CourseStatus
{
    Draft,
    Published,
    Archived
}

public enum CourseDifficulty
{
    Beginner,
    Intermediate,
    Advanced,
    Expert
}
