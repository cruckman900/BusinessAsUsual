namespace LMS.Domain.DTOs;

/// <summary>
/// Represents comprehensive analytics for a quiz
/// </summary>
public class QuizAnalyticsDto
{
    public Guid QuizId { get; set; }
    public string QuizTitle { get; set; } = string.Empty;
    public QuizType QuizType { get; set; }

    // Overall Statistics
    public int TotalAttempts { get; set; }
    public int UniqueLearnersAttempted { get; set; }
    public int CompletedAttempts { get; set; }
    public int InProgressAttempts { get; set; }
    public int AbandonedAttempts { get; set; }

    // Performance Metrics
    public decimal AverageScore { get; set; }
    public decimal MedianScore { get; set; }
    public decimal HighestScore { get; set; }
    public decimal LowestScore { get; set; }
    public int PassCount { get; set; }
    public int FailCount { get; set; }
    public decimal PassRate { get; set; } // Percentage

    // Timing Metrics
    public double AverageCompletionTimeMinutes { get; set; }
    public double MedianCompletionTimeMinutes { get; set; }
    public double FastestCompletionTimeMinutes { get; set; }
    public double SlowestCompletionTimeMinutes { get; set; }

    // Attempt Distribution
    public int FirstAttemptPassCount { get; set; }
    public decimal FirstAttemptPassRate { get; set; }
    public Dictionary<int, int> AttemptDistribution { get; set; } = new(); // AttemptNumber -> Count

    // Question-Level Metrics
    public List<QuestionMetricsDto> QuestionMetrics { get; set; } = new();

    // Time-Series Data (for trending)
    public List<DailyQuizMetricsDto> DailyMetrics { get; set; } = new();
}

/// <summary>
/// Represents analytics for a specific question within a quiz
/// </summary>
public class QuestionMetricsDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public int OrderIndex { get; set; }
    public int MaxPoints { get; set; }

    // Performance Metrics
    public int TotalAnswers { get; set; }
    public int CorrectAnswers { get; set; }
    public int IncorrectAnswers { get; set; }
    public decimal SuccessRate { get; set; } // Percentage
    public decimal AveragePointsEarned { get; set; }

    // Difficulty Indicator
    public DifficultyLevel PerceivedDifficulty { get; set; }

    // For Multiple Choice - Track incorrect option selections
    public Dictionary<Guid, int> OptionSelectionCount { get; set; } = new(); // OptionId -> Count
    public List<CommonWrongAnswerDto> CommonWrongAnswers { get; set; } = new();
}

/// <summary>
/// Represents daily aggregated metrics for trending analysis
/// </summary>
public class DailyQuizMetricsDto
{
    public DateTime Date { get; set; }
    public int AttemptCount { get; set; }
    public int CompletedCount { get; set; }
    public decimal AverageScore { get; set; }
    public decimal PassRate { get; set; }
}

/// <summary>
/// Represents a commonly selected wrong answer for a question
/// </summary>
public class CommonWrongAnswerDto
{
    public Guid OptionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public int SelectionCount { get; set; }
    public decimal SelectionRate { get; set; } // Percentage of incorrect answers
}

/// <summary>
/// Represents a learner's quiz history and performance trends
/// </summary>
public class LearnerQuizHistoryDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public Guid QuizId { get; set; }
    public string QuizTitle { get; set; } = string.Empty;

    public int TotalAttempts { get; set; }
    public decimal BestScore { get; set; }
    public decimal LatestScore { get; set; }
    public decimal AverageScore { get; set; }
    public bool HasPassed { get; set; }

    public List<QuizAttemptSummaryDto> Attempts { get; set; } = new();
    public List<string> WeakAreas { get; set; } = new(); // Question topics or types where learner struggles
}

/// <summary>
/// Summary of a single quiz attempt
/// </summary>
public class QuizAttemptSummaryDto
{
    public Guid AttemptId { get; set; }
    public int AttemptNumber { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal ScorePercentage { get; set; }
    public bool Passed { get; set; }
    public int TotalPoints { get; set; }
    public int PointsEarned { get; set; }
    public QuizAttemptStatus Status { get; set; }
    public double? CompletionTimeMinutes { get; set; }
}

/// <summary>
/// Overall quiz performance summary across all quizzes
/// </summary>
public class QuizPerformanceSummaryDto
{
    public int TotalQuizzes { get; set; }
    public int TotalAttempts { get; set; }
    public int TotalCompletions { get; set; }
    public decimal OverallAverageScore { get; set; }
    public decimal OverallPassRate { get; set; }
    public int ActiveLearners { get; set; } // Learners who have attempted at least one quiz

    public List<QuizSummaryDto> TopPerformingQuizzes { get; set; } = new();
    public List<QuizSummaryDto> LowestPerformingQuizzes { get; set; } = new();
    public List<QuizSummaryDto> MostAttemptedQuizzes { get; set; } = new();
}

/// <summary>
/// Brief summary of a quiz for listings
/// </summary>
public class QuizSummaryDto
{
    public Guid QuizId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int AttemptCount { get; set; }
    public decimal AverageScore { get; set; }
    public decimal PassRate { get; set; }
}

public enum DifficultyLevel
{
    VeryEasy,    // > 90% success rate
    Easy,        // 75-90%
    Moderate,    // 50-75%
    Hard,        // 25-50%
    VeryHard     // < 25%
}

// Import enums from domain entities
public enum QuizType
{
    Practice,
    Assessment
}

public enum QuestionType
{
    MultipleChoice,
    TrueFalse,
    ShortAnswer,
    MultipleSelect
}

public enum QuizAttemptStatus
{
    InProgress,
    Completed,
    Abandoned
}
