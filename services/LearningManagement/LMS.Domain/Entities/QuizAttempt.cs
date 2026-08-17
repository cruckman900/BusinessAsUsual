namespace LMS.Domain.Entities;

/// <summary>
/// Represents a learner's attempt at a quiz
/// </summary>
public class QuizAttempt : BaseEntity
{
    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public string EmployeeId { get; set; } = string.Empty; // Reference to HR employee
    public int AttemptNumber { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public QuizAttemptStatus Status { get; set; } = QuizAttemptStatus.InProgress;

    public int TotalPoints { get; set; }
    public int PointsEarned { get; set; }
    public decimal ScorePercentage { get; set; }
    public bool Passed { get; set; }

    // Navigation properties
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}

public enum QuizAttemptStatus
{
    InProgress,
    Completed,
    Abandoned
}
