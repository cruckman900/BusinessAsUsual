using LMS.Domain.DTOs;

namespace LMS.Application.Services;

/// <summary>
/// Service for generating and exporting analytics reports
/// </summary>
public interface IReportGenerationService
{
    /// <summary>
    /// Generate a CSV report from quiz analytics data
    /// </summary>
    Task<byte[]> GenerateQuizAnalyticsReportAsync(QuizAnalyticsDto analytics, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate a CSV report from learner quiz history
    /// </summary>
    Task<byte[]> GenerateLearnerHistoryReportAsync(List<LearnerQuizHistoryDto> history, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate a CSV report from performance summary
    /// </summary>
    Task<byte[]> GeneratePerformanceSummaryReportAsync(QuizPerformanceSummaryDto summary, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate a CSV report for question-level metrics
    /// </summary>
    Task<byte[]> GenerateQuestionMetricsReportAsync(List<QuestionMetricsDto> metrics, string quizTitle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate a PDF report from analytics data
    /// </summary>
    Task<byte[]> GeneratePdfReportAsync(GenerateReportRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate a learning analytics dashboard PDF report
    /// </summary>
    Task<byte[]> GenerateLearningAnalyticsPdfAsync(LearningAnalyticsDashboardDto dashboard, CancellationToken cancellationToken = default);
}
