namespace LMS.Domain.Entities;

/// <summary>
/// Represents an answer option for multiple choice questions
/// </summary>
public class QuestionOption : BaseEntity
{
    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int OrderIndex { get; set; }
}
