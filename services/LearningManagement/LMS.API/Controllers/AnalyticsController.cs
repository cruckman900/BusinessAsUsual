using LMS.Application.Common;
using LMS.Application.Features.Analytics.Queries;
using LMS.Application.Services;
using LMS.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/lms/analytics")]
// [Authorize] // Temporarily disabled for integrated Web app
public class AnalyticsController : ControllerBase
{
    private readonly IQueryHandler<GetQuizAnalyticsQuery, Result<QuizAnalyticsDto>> _quizAnalyticsHandler;
    private readonly IQueryHandler<GetLearnerQuizHistoryQuery, Result<List<LearnerQuizHistoryDto>>> _learnerHistoryHandler;
    private readonly IQueryHandler<GetQuizPerformanceSummaryQuery, Result<QuizPerformanceSummaryDto>> _performanceSummaryHandler;
    private readonly IQueryHandler<GetLearningAnalyticsDashboardQuery, Result<LearningAnalyticsDashboardDto>> _dashboardHandler;
    private readonly IReportGenerationService _reportService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(
        IQueryHandler<GetQuizAnalyticsQuery, Result<QuizAnalyticsDto>> quizAnalyticsHandler,
        IQueryHandler<GetLearnerQuizHistoryQuery, Result<List<LearnerQuizHistoryDto>>> learnerHistoryHandler,
        IQueryHandler<GetQuizPerformanceSummaryQuery, Result<QuizPerformanceSummaryDto>> performanceSummaryHandler,
        IQueryHandler<GetLearningAnalyticsDashboardQuery, Result<LearningAnalyticsDashboardDto>> dashboardHandler,
        IReportGenerationService reportService,
        ILogger<AnalyticsController> logger)
    {
        _quizAnalyticsHandler = quizAnalyticsHandler;
        _learnerHistoryHandler = learnerHistoryHandler;
        _performanceSummaryHandler = performanceSummaryHandler;
        _dashboardHandler = dashboardHandler;
        _reportService = reportService;
        _logger = logger;
    }

    /// <summary>
    /// Get comprehensive analytics for a specific quiz
    /// </summary>
    [HttpGet("quiz/{quizId:guid}")]
    [ProducesResponseType(typeof(QuizAnalyticsDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetQuizAnalytics(Guid quizId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting analytics for quiz {QuizId}", quizId);

        var result = await _quizAnalyticsHandler.HandleAsync(
            new GetQuizAnalyticsQuery(quizId),
            cancellationToken);

        if (!result.Success)
        {
            _logger.LogWarning("Failed to get quiz analytics for {QuizId}: {Error}", quizId, result.ErrorMessage);
            return NotFound(new { error = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Get question-level metrics for a quiz
    /// </summary>
    [HttpGet("quiz/{quizId:guid}/questions")]
    [ProducesResponseType(typeof(List<QuestionMetricsDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetQuestionMetrics(Guid quizId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting question metrics for quiz {QuizId}", quizId);

        var result = await _quizAnalyticsHandler.HandleAsync(
            new GetQuizAnalyticsQuery(quizId),
            cancellationToken);

        if (!result.Success)
        {
            return NotFound(new { error = result.ErrorMessage });
        }

        return Ok(result.Data!.QuestionMetrics);
    }

    /// <summary>
    /// Export quiz analytics as CSV
    /// </summary>
    [HttpGet("quiz/{quizId:guid}/export")]
    [ProducesResponseType(typeof(FileResult), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ExportQuizAnalytics(Guid quizId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Exporting quiz analytics for {QuizId}", quizId);

        var result = await _quizAnalyticsHandler.HandleAsync(
            new GetQuizAnalyticsQuery(quizId),
            cancellationToken);

        if (!result.Success)
        {
            return NotFound(new { error = result.ErrorMessage });
        }

        var csvBytes = await _reportService.GenerateQuizAnalyticsReportAsync(
            result.Data!,
            cancellationToken);

        var fileName = $"quiz-analytics-{result.Data.QuizTitle.Replace(" ", "-")}-{DateTime.UtcNow:yyyyMMdd}.csv";
        return File(csvBytes, "text/csv", fileName);
    }

    /// <summary>
    /// Export question metrics as CSV
    /// </summary>
    [HttpGet("quiz/{quizId:guid}/questions/export")]
    [ProducesResponseType(typeof(FileResult), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ExportQuestionMetrics(Guid quizId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Exporting question metrics for quiz {QuizId}", quizId);

        var result = await _quizAnalyticsHandler.HandleAsync(
            new GetQuizAnalyticsQuery(quizId),
            cancellationToken);

        if (!result.Success)
        {
            return NotFound(new { error = result.ErrorMessage });
        }

        var csvBytes = await _reportService.GenerateQuestionMetricsReportAsync(
            result.Data!.QuestionMetrics,
            result.Data.QuizTitle,
            cancellationToken);

        var fileName = $"question-metrics-{result.Data.QuizTitle.Replace(" ", "-")}-{DateTime.UtcNow:yyyyMMdd}.csv";
        return File(csvBytes, "text/csv", fileName);
    }

    /// <summary>
    /// Get quiz history for a learner (all quizzes or specific quiz)
    /// </summary>
    [HttpGet("learner/{employeeId}/quiz-history")]
    [ProducesResponseType(typeof(List<LearnerQuizHistoryDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetLearnerQuizHistory(
        string employeeId,
        [FromQuery] Guid? quizId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting quiz history for employee {EmployeeId}", employeeId);

        var result = await _learnerHistoryHandler.HandleAsync(
            new GetLearnerQuizHistoryQuery(employeeId, quizId),
            cancellationToken);

        if (!result.Success)
        {
            return NotFound(new { error = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Export learner quiz history as CSV
    /// </summary>
    [HttpGet("learner/{employeeId}/quiz-history/export")]
    [ProducesResponseType(typeof(FileResult), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ExportLearnerQuizHistory(
        string employeeId,
        [FromQuery] Guid? quizId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Exporting quiz history for employee {EmployeeId}", employeeId);

        var result = await _learnerHistoryHandler.HandleAsync(
            new GetLearnerQuizHistoryQuery(employeeId, quizId),
            cancellationToken);

        if (!result.Success)
        {
            return NotFound(new { error = result.ErrorMessage });
        }

        var csvBytes = await _reportService.GenerateLearnerHistoryReportAsync(
            result.Data!,
            cancellationToken);

        var fileName = $"learner-history-{employeeId}-{DateTime.UtcNow:yyyyMMdd}.csv";
        return File(csvBytes, "text/csv", fileName);
    }

    /// <summary>
    /// Get overall quiz performance summary
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(QuizPerformanceSummaryDto), 200)]
    public async Task<IActionResult> GetPerformanceSummary(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting overall quiz performance summary");

        var result = await _performanceSummaryHandler.HandleAsync(
            new GetQuizPerformanceSummaryQuery(),
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Export performance summary as CSV
    /// </summary>
    [HttpGet("summary/export")]
    [ProducesResponseType(typeof(FileResult), 200)]
    public async Task<IActionResult> ExportPerformanceSummary(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Exporting quiz performance summary");

        var result = await _performanceSummaryHandler.HandleAsync(
            new GetQuizPerformanceSummaryQuery(),
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        var csvBytes = await _reportService.GeneratePerformanceSummaryReportAsync(
            result.Data!,
            cancellationToken);

        var fileName = $"performance-summary-{DateTime.UtcNow:yyyyMMdd}.csv";
        return File(csvBytes, "text/csv", fileName);
    }

    /// <summary>
    /// Get overall learning analytics dashboard
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(LearningAnalyticsDashboardDto), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetLearningAnalyticsDashboard(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting learning analytics dashboard");

        var result = await _dashboardHandler.HandleAsync(
            new GetLearningAnalyticsDashboardQuery
            {
                StartDate = startDate,
                EndDate = endDate
            },
            cancellationToken);

        if (!result.Success)
        {
            _logger.LogWarning("Failed to get learning analytics dashboard: {Error}", result.ErrorMessage);
            return BadRequest(new { error = result.ErrorMessage });
        }

        return Ok(result.Data);
    }
}
