namespace LMS.Contracts.DTOs;

public class CourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public int EstimatedDurationMinutes { get; set; }
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = new();
    public DateTime? PublishedDate { get; set; }
    public int ModuleCount { get; set; }
    public int LessonCount { get; set; }
    public bool RequiresAssessment { get; set; }
    public int PassingScore { get; set; }
    public bool IssuesCertificate { get; set; }
}
