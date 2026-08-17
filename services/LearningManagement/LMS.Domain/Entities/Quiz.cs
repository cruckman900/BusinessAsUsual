namespace LMS.Domain.Entities;

/// <summary>
/// Represents a quiz (can be inline in a lesson or end-of-course assessment)
/// </summary>
public class Quiz : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public QuizType QuizType { get; set; } = QuizType.Practice;
    public int TimeLimitMinutes { get; set; } = 0; // 0 = no time limit
    public int PassingScore { get; set; } = 70; // Percentage
    public bool ShuffleQuestions { get; set; } = false;
    public bool ShowResultsImmediately { get; set; } = true;
    public int MaxAttempts { get; set; } = 0; // 0 = unlimited
    public bool AllowReview { get; set; } = true; // Show correct answers after completion

    // For end-of-course assessment
    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }

    // For inline lesson quiz
    public Guid? LessonId { get; set; }
    public Lesson? Lesson { get; set; }

    // Navigation properties
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
}

public enum QuizType
{
    Practice, // Inline quiz in lesson (doesn't affect completion)
    Assessment // End-of-course assessment (required for completion)
}
