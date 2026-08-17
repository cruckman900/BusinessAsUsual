using LMS.Application.Common;
using LMS.Domain.Entities;

namespace LMS.Application.Features.Learning.Commands;

public class SubmitQuizAttemptCommand : ICommand<Result<QuizAttemptResult>>
{
    public Guid QuizId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public Dictionary<Guid, QuestionAnswer> Answers { get; set; } = new();
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class QuestionAnswer
{
    public Guid QuestionId { get; set; }
    public Guid? SelectedOptionId { get; set; } // For multiple choice / true-false
    public List<Guid> SelectedOptionIds { get; set; } = new(); // For multiple select
    public string? TextAnswer { get; set; } // For short answer
}

public class QuizAttemptResult
{
    public Guid AttemptId { get; set; }
    public int AttemptNumber { get; set; }
    public int TotalPoints { get; set; }
    public int PointsEarned { get; set; }
    public decimal ScorePercentage { get; set; }
    public bool Passed { get; set; }
    public QuizAttempt Attempt { get; set; } = null!;
}
