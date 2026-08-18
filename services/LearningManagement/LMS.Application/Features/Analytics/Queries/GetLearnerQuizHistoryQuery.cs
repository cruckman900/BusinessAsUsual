using LMS.Application.Common;
using LMS.Domain.DTOs;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Analytics.Queries;

/// <summary>
/// Query to get a learner's quiz history and performance trends
/// </summary>
public record GetLearnerQuizHistoryQuery(string EmployeeId, Guid? QuizId = null) : IQuery<Result<List<LearnerQuizHistoryDto>>>;

public class GetLearnerQuizHistoryQueryHandler : IQueryHandler<GetLearnerQuizHistoryQuery, Result<List<LearnerQuizHistoryDto>>>
{
    private readonly IQuizAttemptRepository _attemptRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly ILogger<GetLearnerQuizHistoryQueryHandler> _logger;

    public GetLearnerQuizHistoryQueryHandler(
        IQuizAttemptRepository attemptRepository,
        IQuizRepository quizRepository,
        ILogger<GetLearnerQuizHistoryQueryHandler> logger)
    {
        _attemptRepository = attemptRepository;
        _quizRepository = quizRepository;
        _logger = logger;
    }

    public async Task<Result<List<LearnerQuizHistoryDto>>> HandleAsync(
        GetLearnerQuizHistoryQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var historyList = new List<LearnerQuizHistoryDto>();

            if (query.QuizId.HasValue)
            {
                // Get history for a specific quiz
                var history = await GetQuizHistoryForLearnerAsync(
                    query.EmployeeId,
                    query.QuizId.Value,
                    cancellationToken);

                if (history != null)
                {
                    historyList.Add(history);
                }
            }
            else
            {
                // Get all quizzes the learner has attempted
                var allQuizzes = await _quizRepository.GetAllAsync(cancellationToken);

                foreach (var quiz in allQuizzes)
                {
                    var history = await GetQuizHistoryForLearnerAsync(
                        query.EmployeeId,
                        quiz.Id,
                        cancellationToken);

                    if (history != null)
                    {
                        historyList.Add(history);
                    }
                }
            }

            _logger.LogInformation("Retrieved quiz history for employee {EmployeeId}: {QuizCount} quizzes",
                query.EmployeeId, historyList.Count);

            return Result<List<LearnerQuizHistoryDto>>.Ok(historyList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quiz history for employee {EmployeeId}", query.EmployeeId);
            return Result<List<LearnerQuizHistoryDto>>.Fail($"Failed to retrieve quiz history: {ex.Message}");
        }
    }

    private async Task<LearnerQuizHistoryDto?> GetQuizHistoryForLearnerAsync(
        string employeeId,
        Guid quizId,
        CancellationToken cancellationToken)
    {
        var attempts = (await _attemptRepository.GetByEmployeeAndQuizAsync(employeeId, quizId, cancellationToken))
            .OrderBy(a => a.StartedAt)
            .ToList();

        if (!attempts.Any())
        {
            return null;
        }

        var quiz = await _quizRepository.GetByIdAsync(quizId, cancellationToken);
        if (quiz == null)
        {
            return null;
        }

        var completedAttempts = attempts
            .Where(a => a.Status == LMS.Domain.Entities.QuizAttemptStatus.Completed)
            .ToList();

        var history = new LearnerQuizHistoryDto
        {
            EmployeeId = employeeId,
            EmployeeName = "Unknown", // TODO: Fetch from HR service
            QuizId = quizId,
            QuizTitle = quiz.Title,
            TotalAttempts = attempts.Count,
            BestScore = completedAttempts.Any() ? completedAttempts.Max(a => a.ScorePercentage) : 0,
            LatestScore = completedAttempts.Any() ? completedAttempts.Last().ScorePercentage : 0,
            AverageScore = completedAttempts.Any() 
                ? Math.Round(completedAttempts.Average(a => a.ScorePercentage), 2) 
                : 0,
            HasPassed = completedAttempts.Any(a => a.Passed)
        };

        // Map attempts to summaries
        history.Attempts = attempts.Select(a => new QuizAttemptSummaryDto
        {
            AttemptId = a.Id,
            AttemptNumber = a.AttemptNumber,
            StartedAt = a.StartedAt,
            CompletedAt = a.CompletedAt,
            ScorePercentage = a.ScorePercentage,
            Passed = a.Passed,
            TotalPoints = a.TotalPoints,
            PointsEarned = a.PointsEarned,
            Status = (QuizAttemptStatus)a.Status,
            CompletionTimeMinutes = a.CompletedAt.HasValue
                ? Math.Round((a.CompletedAt.Value - a.StartedAt).TotalMinutes, 2)
                : null
        }).ToList();

        // Identify weak areas (questions answered incorrectly most often)
        if (completedAttempts.Any())
        {
            var incorrectAnswers = completedAttempts
                .SelectMany(a => a.Answers)
                .Where(a => !a.IsCorrect)
                .GroupBy(a => a.QuestionId)
                .OrderByDescending(g => g.Count())
                .Take(3) // Top 3 problematic questions
                .ToList();

            var quizWithQuestions = await _quizRepository.GetWithQuestionsAsync(quizId, cancellationToken);
            if (quizWithQuestions?.Questions != null)
            {
                history.WeakAreas = incorrectAnswers
                    .Select(g =>
                    {
                        var question = quizWithQuestions.Questions.FirstOrDefault(q => q.Id == g.Key);
                        return question != null
                            ? $"{question.QuestionText.Substring(0, Math.Min(50, question.QuestionText.Length))}..."
                            : "Unknown";
                    })
                    .ToList();
            }
        }

        return history;
    }
}
