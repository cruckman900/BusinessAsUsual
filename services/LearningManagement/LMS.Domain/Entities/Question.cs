namespace LMS.Domain.Entities;

/// <summary>
/// Represents a question within a quiz
/// </summary>
public class Question : BaseEntity
{
    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public string QuestionText { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public int OrderIndex { get; set; }
    public int Points { get; set; } = 1;
    public string? Explanation { get; set; } // Shown after answering
    public string? HintText { get; set; }

    // Navigation properties
    public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}

public enum QuestionType
{
    MultipleChoice,
    TrueFalse,
    ShortAnswer,
    MultipleSelect // Multiple correct answers
}
