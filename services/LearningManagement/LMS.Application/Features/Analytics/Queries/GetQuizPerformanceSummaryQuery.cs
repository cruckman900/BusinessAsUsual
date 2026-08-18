using LMS.Application.Common;
using LMS.Domain.DTOs;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Analytics.Queries;

/// <summary>
/// Query to get overall quiz performance summary across all quizzes
/// </summary>
public record GetQuizPerformanceSummaryQuery : IQuery<Result<QuizPerformanceSummaryDto>>;

public class GetQuizPerformanceSummaryQueryHandler : IQueryHandler<GetQuizPerformanceSummaryQuery, Result<QuizPerformanceSummaryDto>>
{
    private readonly IQuizRepository _quizRepository;
    private readonly ILogger<GetQuizPerformanceSummaryQueryHandler> _logger;

    public GetQuizPerformanceSummaryQueryHandler(
        IQuizRepository quizRepository,
        ILogger<GetQuizPerformanceSummaryQueryHandler> logger)
    {
        _quizRepository = quizRepository;
        _logger = logger;
    }

    public async Task<Result<QuizPerformanceSummaryDto>> HandleAsync(
        GetQuizPerformanceSummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var allQuizzes = await _quizRepository.GetAllWithAttemptsAsync(cancellationToken);

            if (!allQuizzes.Any())
            {
                return Result<QuizPerformanceSummaryDto>.Ok(new QuizPerformanceSummaryDto());
            }

            var allAttempts = allQuizzes.SelectMany(q => q.Attempts).ToList();
            var completedAttempts = allAttempts
                .Where(a => a.Status == LMS.Domain.Entities.QuizAttemptStatus.Completed)
                .ToList();

            var summary = new QuizPerformanceSummaryDto
            {
                TotalQuizzes = allQuizzes.Count(),
                TotalAttempts = allAttempts.Count,
                TotalCompletions = completedAttempts.Count,
                OverallAverageScore = completedAttempts.Any() 
                    ? Math.Round(completedAttempts.Average(a => a.ScorePercentage), 2) 
                    : 0,
                OverallPassRate = completedAttempts.Any()
                    ? Math.Round((decimal)completedAttempts.Count(a => a.Passed) / completedAttempts.Count * 100, 2)
                    : 0,
                ActiveLearners = allAttempts.Select(a => a.EmployeeId).Distinct().Count()
            };

            // Get quiz summaries for each quiz
            var quizSummaries = allQuizzes.Select(quiz =>
            {
                var quizAttempts = quiz.Attempts.ToList();
                var quizCompletedAttempts = quizAttempts
                    .Where(a => a.Status == LMS.Domain.Entities.QuizAttemptStatus.Completed)
                    .ToList();

                return new QuizSummaryDto
                {
                    QuizId = quiz.Id,
                    Title = quiz.Title,
                    AttemptCount = quizAttempts.Count,
                    AverageScore = quizCompletedAttempts.Any()
                        ? Math.Round(quizCompletedAttempts.Average(a => a.ScorePercentage), 2)
                        : 0,
                    PassRate = quizCompletedAttempts.Any()
                        ? Math.Round((decimal)quizCompletedAttempts.Count(a => a.Passed) / quizCompletedAttempts.Count * 100, 2)
                        : 0
                };
            }).ToList();

            // Top performing quizzes (highest average score)
            summary.TopPerformingQuizzes = quizSummaries
                .Where(q => q.AttemptCount > 0)
                .OrderByDescending(q => q.AverageScore)
                .Take(5)
                .ToList();

            // Lowest performing quizzes (lowest pass rate)
            summary.LowestPerformingQuizzes = quizSummaries
                .Where(q => q.AttemptCount > 0)
                .OrderBy(q => q.PassRate)
                .Take(5)
                .ToList();

            // Most attempted quizzes
            summary.MostAttemptedQuizzes = quizSummaries
                .OrderByDescending(q => q.AttemptCount)
                .Take(5)
                .ToList();

            _logger.LogInformation("Generated performance summary: {TotalQuizzes} quizzes, {TotalAttempts} attempts",
                summary.TotalQuizzes, summary.TotalAttempts);

            return Result<QuizPerformanceSummaryDto>.Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating quiz performance summary");
            return Result<QuizPerformanceSummaryDto>.Fail($"Failed to generate performance summary: {ex.Message}");
        }
    }
}
