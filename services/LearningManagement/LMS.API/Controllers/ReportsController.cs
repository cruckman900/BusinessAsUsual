using LMS.Application.Services;
using LMS.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/lms/reports")]
// [Authorize] // Temporarily disabled for integrated Web app
public class ReportsController : ControllerBase
{
    private readonly IReportGenerationService _reportService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(
        IReportGenerationService reportService,
        ILogger<ReportsController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    /// <summary>
    /// Generate a custom report based on the request
    /// </summary>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(FileResult), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GenerateReport(
        [FromBody] GenerateReportRequest request,
        [FromQuery] string format = "csv",
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating {Format} report of type {ReportType}", format, request.ReportType);

        try
        {
            byte[] reportBytes;
            string contentType;
            string fileExtension;

            if (format.ToLower() == "pdf")
            {
                reportBytes = await _reportService.GeneratePdfReportAsync(request, cancellationToken);
                contentType = "application/pdf";
                fileExtension = "pdf";
            }
            else
            {
                // Default to CSV - implement based on report type
                // For now, return a placeholder
                var message = $"Report Type: {request.ReportType}\nFormat: CSV\nDate Range: {request.StartDate} to {request.EndDate}";
                reportBytes = System.Text.Encoding.UTF8.GetBytes(message);
                contentType = "text/csv";
                fileExtension = "csv";
            }

            var fileName = $"report-{request.ReportType}-{DateTime.UtcNow:yyyyMMdd}.{fileExtension}";
            return File(reportBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report");
            return BadRequest(new { error = $"Failed to generate report: {ex.Message}" });
        }
    }

    /// <summary>
    /// Export learning analytics dashboard as PDF
    /// </summary>
    [HttpGet("learning-analytics/pdf")]
    [ProducesResponseType(typeof(FileResult), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ExportLearningAnalyticsPdf(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Exporting learning analytics dashboard as PDF");

        try
        {
            // This would fetch the dashboard data and generate PDF
            // For now, return a placeholder
            var dashboardPlaceholder = new LearningAnalyticsDashboardDto
            {
                OverallMetrics = new OverallMetricsDto
                {
                    TotalEnrollments = 0,
                    ActiveLearners = 0,
                    OverallCompletionRate = 0
                }
            };

            var pdfBytes = await _reportService.GenerateLearningAnalyticsPdfAsync(dashboardPlaceholder, cancellationToken);
            var fileName = $"learning-analytics-{DateTime.UtcNow:yyyyMMdd}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting learning analytics PDF");
            return BadRequest(new { error = $"Failed to export PDF: {ex.Message}" });
        }
    }
}
