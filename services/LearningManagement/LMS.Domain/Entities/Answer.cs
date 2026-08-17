namespace LMS.Domain.Entities;

/// <summary>
/// Represents a learner's answer to a quiz question
/// </summary>
public class Answer : BaseEntity
{
    public Guid QuizAttemptId { get; set; }
    public QuizAttempt QuizAttempt { get; set; } = null!;

    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public Guid? SelectedOptionId { get; set; } // For multiple choice
    public QuestionOption? SelectedOption { get; set; }

    public List<Guid> SelectedOptionIds { get; set; } = new(); // For multiple select
    public string? TextAnswer { get; set; } // For short answer

    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
}
