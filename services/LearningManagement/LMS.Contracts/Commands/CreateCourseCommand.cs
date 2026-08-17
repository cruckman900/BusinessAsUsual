namespace LMS.Contracts.Commands;

public class CreateCourseCommand
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = new();
    public string Difficulty { get; set; } = "Beginner";
    public int EstimatedDurationMinutes { get; set; }
    public bool RequiresAssessment { get; set; } = true;
    public int PassingScore { get; set; } = 70;
    public bool IssuesCertificate { get; set; } = true;
}
