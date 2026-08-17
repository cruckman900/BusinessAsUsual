using LMS.Application.Common;
using LMS.Domain.Entities;

namespace LMS.Application.Features.Courses.Commands;

public class UpdateCourseCommand : ICommand<Result>
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = new();
    public CourseDifficulty Difficulty { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public bool RequiresAssessment { get; set; }
    public int PassingScore { get; set; }
    public int MaxAttempts { get; set; }
    public bool IssuesCertificate { get; set; }
    public int? CertificateValidityDays { get; set; }
    public string? UpdatedBy { get; set; }
}
