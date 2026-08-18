using LMS.Application.Common;
using LMS.Domain.DTOs;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Analytics.Queries;

/// <summary>
/// Query to get comprehensive analytics for a specific quiz
/// </summary>
public record GetQuizAnalyticsQuery(Guid QuizId) : IQuery<Result<QuizAnalyticsDto>>;

public class GetQuizAnalyticsQueryHandler : IQueryHandler<GetQuizAnalyticsQuery, Result<QuizAnalyticsDto>>
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizAttemptRepository _attemptRepository;
    private readonly ILogger<GetQuizAnalyticsQueryHandler> _logger;

    public GetQuizAnalyticsQueryHandler(
        IQuizRepository quizRepository,
        IQuizAttemptRepository attemptRepository,
        ILogger<GetQuizAnalyticsQueryHandler> logger)
    {
        _quizRepository = quizRepository;
        _attemptRepository = attemptRepository;
        _logger = logger;
    }

    public async Task<Result<QuizAnalyticsDto>> HandleAsync(
        GetQuizAnalyticsQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get quiz with all attempts and questions
            var quiz = await _quizRepository.GetWithAttemptsAsync(query.QuizId, cancellationToken);
            if (quiz == null)
            {
                return Result<QuizAnalyticsDto>.Fail($"Quiz with ID {query.QuizId} not found");
            }

            var attempts = quiz.Attempts.ToList();
            var completedAttempts = attempts.Where(a => a.Status == LMS.Domain.Entities.QuizAttemptStatus.Completed).ToList();

            // Calculate overall statistics
            var analytics = new QuizAnalyticsDto
            {
                QuizId = quiz.Id,
                QuizTitle = quiz.Title,
                QuizType = (QuizType)quiz.QuizType,

                // Attempt counts
                TotalAttempts = attempts.Count,
                UniqueLearnersAttempted = attempts.Select(a => a.EmployeeId).Distinct().Count(),
                CompletedAttempts = completedAttempts.Count,
                InProgressAttempts = attempts.Count(a => a.Status == LMS.Domain.Entities.QuizAttemptStatus.InProgress),
                AbandonedAttempts = attempts.Count(a => a.Status == LMS.Domain.Entities.QuizAttemptStatus.Abandoned)
            };

            if (completedAttempts.Any())
            {
                // Performance metrics
                var scores = completedAttempts.Select(a => a.ScorePercentage).OrderBy(s => s).ToList();
                analytics.AverageScore = Math.Round(scores.Average(), 2);
                analytics.MedianScore = CalculateMedian(scores);
                analytics.HighestScore = scores.Max();
                analytics.LowestScore = scores.Min();
                analytics.PassCount = completedAttempts.Count(a => a.Passed);
                analytics.FailCount = completedAttempts.Count(a => !a.Passed);
                analytics.PassRate = Math.Round((decimal)analytics.PassCount / completedAttempts.Count * 100, 2);

                // Timing metrics
                var completionTimes = completedAttempts
                    .Where(a => a.CompletedAt.HasValue)
                    .Select(a => (a.CompletedAt!.Value - a.StartedAt).TotalMinutes)
                    .OrderBy(t => t)
                    .ToList();

                if (completionTimes.Any())
                {
                    analytics.AverageCompletionTimeMinutes = Math.Round(completionTimes.Average(), 2);
                    analytics.MedianCompletionTimeMinutes = Math.Round(CalculateMedian(completionTimes), 2);
                    analytics.FastestCompletionTimeMinutes = Math.Round(completionTimes.Min(), 2);
                    analytics.SlowestCompletionTimeMinutes = Math.Round(completionTimes.Max(), 2);
                }

                // First attempt pass rate
                var firstAttempts = completedAttempts.Where(a => a.AttemptNumber == 1).ToList();
                analytics.FirstAttemptPassCount = firstAttempts.Count(a => a.Passed);
                analytics.FirstAttemptPassRate = firstAttempts.Any()
                    ? Math.Round((decimal)analytics.FirstAttemptPassCount / firstAttempts.Count * 100, 2)
                    : 0;

                // Attempt distribution
                analytics.AttemptDistribution = completedAttempts
                    .GroupBy(a => a.AttemptNumber)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Daily metrics for trending
                analytics.DailyMetrics = completedAttempts
                    .Where(a => a.CompletedAt.HasValue)
                    .GroupBy(a => a.CompletedAt!.Value.Date)
                    .Select(g => new DailyQuizMetricsDto
                    {
                        Date = g.Key,
                        AttemptCount = g.Count(),
                        CompletedCount = g.Count(),
                        AverageScore = Math.Round(g.Average(a => a.ScorePercentage), 2),
                        PassRate = Math.Round((decimal)g.Count(a => a.Passed) / g.Count() * 100, 2)
                    })
                    .OrderBy(d => d.Date)
                    .ToList();
            }

            // Question-level metrics (will be calculated separately for performance)
            analytics.QuestionMetrics = await CalculateQuestionMetricsAsync(quiz.Id, attempts, cancellationToken);

            _logger.LogInformation("Generated analytics for quiz {QuizId} with {AttemptCount} attempts",
                quiz.Id, attempts.Count);

            return Result<QuizAnalyticsDto>.Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating analytics for quiz {QuizId}", query.QuizId);
            return Result<QuizAnalyticsDto>.Fail($"Failed to generate quiz analytics: {ex.Message}");
        }
    }

    private async Task<List<QuestionMetricsDto>> CalculateQuestionMetricsAsync(
        Guid quizId,
        List<LMS.Domain.Entities.QuizAttempt> attempts,
        CancellationToken cancellationToken)
    {
        // Get all answers for all attempts
        var allAnswers = attempts.SelectMany(a => a.Answers).ToList();

        // Get quiz with questions to get question details
        var quiz = await _quizRepository.GetWithQuestionsAsync(quizId, cancellationToken);
        if (quiz?.Questions == null || !quiz.Questions.Any())
        {
            return new List<QuestionMetricsDto>();
        }

        var metrics = new List<QuestionMetricsDto>();

        foreach (var question in quiz.Questions.OrderBy(q => q.OrderIndex))
        {
            var questionAnswers = allAnswers.Where(a => a.QuestionId == question.Id).ToList();

            if (!questionAnswers.Any())
            {
                continue;
            }

            var correctCount = questionAnswers.Count(a => a.IsCorrect);
            var incorrectCount = questionAnswers.Count(a => !a.IsCorrect);
            var successRate = Math.Round((decimal)correctCount / questionAnswers.Count * 100, 2);

            var metric = new QuestionMetricsDto
            {
                QuestionId = question.Id,
                QuestionText = question.QuestionText,
                QuestionType = (QuestionType)question.Type,
                OrderIndex = question.OrderIndex,
                MaxPoints = question.Points,
                TotalAnswers = questionAnswers.Count,
                CorrectAnswers = correctCount,
                IncorrectAnswers = incorrectCount,
                SuccessRate = successRate,
                AveragePointsEarned = Math.Round((decimal)questionAnswers.Average(a => a.PointsEarned), 2),
                PerceivedDifficulty = DetermineDifficulty(successRate)
            };

            // Track option selection counts for multiple choice questions
            if (question.Type == LMS.Domain.Entities.QuestionType.MultipleChoice)
            {
                metric.OptionSelectionCount = questionAnswers
                    .Where(a => a.SelectedOptionId.HasValue)
                    .GroupBy(a => a.SelectedOptionId!.Value)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Find common wrong answers
                var incorrectAnswers = questionAnswers.Where(a => !a.IsCorrect && a.SelectedOptionId.HasValue).ToList();
                if (incorrectAnswers.Any())
                {
                    metric.CommonWrongAnswers = incorrectAnswers
                        .GroupBy(a => a.SelectedOptionId!.Value)
                        .Select(g => new CommonWrongAnswerDto
                        {
                            OptionId = g.Key,
                            OptionText = question.Options.FirstOrDefault(o => o.Id == g.Key)?.OptionText ?? "Unknown",
                            SelectionCount = g.Count(),
                            SelectionRate = Math.Round((decimal)g.Count() / incorrectAnswers.Count * 100, 2)
                        })
                        .OrderByDescending(w => w.SelectionCount)
                        .Take(3) // Top 3 wrong answers
                        .ToList();
                }
            }

            metrics.Add(metric);
        }

        return metrics;
    }

    private decimal CalculateMedian(List<decimal> values)
    {
        if (!values.Any()) return 0;

        var sorted = values.OrderBy(v => v).ToList();
        int count = sorted.Count;

        if (count % 2 == 0)
        {
            return Math.Round((sorted[count / 2 - 1] + sorted[count / 2]) / 2, 2);
        }
        else
        {
            return Math.Round(sorted[count / 2], 2);
        }
    }

    private double CalculateMedian(List<double> values)
    {
        if (!values.Any()) return 0;

        var sorted = values.OrderBy(v => v).ToList();
        int count = sorted.Count;

        if (count % 2 == 0)
        {
            return (sorted[count / 2 - 1] + sorted[count / 2]) / 2;
        }
        else
        {
            return sorted[count / 2];
        }
    }

    private DifficultyLevel DetermineDifficulty(decimal successRate)
    {
        return successRate switch
        {
            > 90 => DifficultyLevel.VeryEasy,
            > 75 => DifficultyLevel.Easy,
            > 50 => DifficultyLevel.Moderate,
            > 25 => DifficultyLevel.Hard,
            _ => DifficultyLevel.VeryHard
        };
    }
}
