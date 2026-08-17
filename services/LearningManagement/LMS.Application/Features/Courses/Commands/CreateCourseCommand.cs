using LMS.Application.Common;
using LMS.Domain.Entities;

namespace LMS.Application.Features.Courses.Commands;

public class CreateCourseCommand : ICommand<Result<Guid>>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = new();
    public CourseDifficulty Difficulty { get; set; } = CourseDifficulty.Beginner;
    public int EstimatedDurationMinutes { get; set; }
    public bool RequiresAssessment { get; set; } = true;
    public int PassingScore { get; set; } = 70;
    public int MaxAttempts { get; set; } = 3;
    public bool IssuesCertificate { get; set; } = true;
    public int? CertificateValidityDays { get; set; }
    public string? CreatedBy { get; set; }
}
