using LMS.Application.Common;
using LMS.Domain.DTOs;
using LMS.Domain.Repositories;

namespace LMS.Application.Features.Analytics.Queries;

/// <summary>
/// Query to get learning analytics dashboard data
/// </summary>
public class GetLearningAnalyticsDashboardQuery : IQuery<Result<LearningAnalyticsDashboardDto>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// Handler for retrieving learning analytics dashboard
/// </summary>
public class GetLearningAnalyticsDashboardQueryHandler : IQueryHandler<GetLearningAnalyticsDashboardQuery, Result<LearningAnalyticsDashboardDto>>
{
    private readonly ILearnerProgressRepository _learnerProgressRepository;
    private readonly ICourseCompletionRepository _courseCompletionRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICertificateRepository _certificateRepository;

    public GetLearningAnalyticsDashboardQueryHandler(
        ILearnerProgressRepository learnerProgressRepository,
        ICourseCompletionRepository courseCompletionRepository,
        ICourseRepository courseRepository,
        ICertificateRepository certificateRepository)
    {
        _learnerProgressRepository = learnerProgressRepository;
        _courseCompletionRepository = courseCompletionRepository;
        _courseRepository = courseRepository;
        _certificateRepository = certificateRepository;
    }

    public async Task<Result<LearningAnalyticsDashboardDto>> HandleAsync(GetLearningAnalyticsDashboardQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var startDate = query.StartDate ?? DateTime.UtcNow.AddMonths(-6);
            var endDate = query.EndDate ?? DateTime.UtcNow;

            // Get all progress and completions
            var progressRecords = (await _learnerProgressRepository.GetAllAsync(cancellationToken)).ToList();
            var completions = (await _courseCompletionRepository.GetAllAsync(cancellationToken)).ToList();
            var courses = (await _courseRepository.GetAllAsync()).ToList();
            var certificates = (await _certificateRepository.GetAllAsync()).ToList();

            // Filter completions by date range
            var filteredCompletions = completions
                .Where(c => c.StartedAt >= startDate && c.StartedAt <= endDate)
                .ToList();

            // Calculate overall metrics
            var overallMetrics = new OverallMetricsDto
            {
                TotalEnrollments = progressRecords.Count,
                ActiveLearners = progressRecords.Select(p => p.EmployeeId).Distinct().Count(),
                CompletedCourses = filteredCompletions.Count,
                OverallCompletionRate = progressRecords.Any()
                    ? (decimal)filteredCompletions.Count / progressRecords.Count * 100
                    : 0,
                AverageProgressPercentage = progressRecords.Any()
                    ? (decimal)progressRecords.Average(p => p.ProgressPercentage)
                    : 0,
                AverageTimeSpentHours = 0, // Placeholder - would need session tracking
                CertificatesIssued = certificates.Count(c => c.IssuedDate >= startDate && c.IssuedDate <= endDate)
            };

            // Calculate completion trends (grouped by week)
            var completionTrends = filteredCompletions
                .GroupBy(c => new DateTime(c.StartedAt.Year, c.StartedAt.Month, c.StartedAt.Day).AddDays(-(int)c.StartedAt.DayOfWeek))
                .Select(g => new CourseCompletionTrendDto
                {
                    Date = g.Key,
                    Enrollments = g.Count(),
                    Completions = g.Count(),
                    CompletionRate = 100, // All in this group are completions
                    ActiveLearners = g.Select(c => c.EmployeeId).Distinct().Count()
                })
                .OrderBy(t => t.Date)
                .ToList();

            // Calculate popular courses
            var popularCourses = courses
                .Select(c => new
                {
                    Course = c,
                    Progress = progressRecords.Where(p => p.CourseId == c.Id).ToList(),
                    Completions = filteredCompletions.Where(comp => comp.CourseId == c.Id).ToList()
                })
                .Where(x => x.Progress.Any())
                .Select(x => new PopularCourseDto
                {
                    CourseId = x.Course.Id,
                    CourseTitle = x.Course.Title,
                    EnrollmentCount = x.Progress.Count,
                    CompletionCount = x.Completions.Count,
                    CompletionRate = x.Progress.Any()
                        ? (decimal)x.Completions.Count / x.Progress.Count * 100
                        : 0,
                    AverageRating = 0, // Placeholder - would need rating system
                    AverageTimeToCompleteHours = x.Completions
                        .Select(c => (c.CompletedAt - c.StartedAt).TotalHours)
                        .DefaultIfEmpty(0)
                        .Average()
                })
                .OrderByDescending(c => c.EnrollmentCount)
                .Take(10)
                .ToList();

            // Calculate engagement metrics (grouped by week)
            var engagementMetrics = progressRecords
                .GroupBy(p => new DateTime(p.LastAccessedAt.Year, p.LastAccessedAt.Month, p.LastAccessedAt.Day).AddDays(-(int)p.LastAccessedAt.DayOfWeek))
                .Select(g => new EngagementMetricDto
                {
                    Period = g.Key,
                    ActiveUsers = g.Select(p => p.EmployeeId).Distinct().Count(),
                    SessionCount = g.Count(), // Simplified - actual sessions would need tracking
                    AverageSessionDurationMinutes = 0, // Placeholder - would need session tracking
                    DropoffCount = g.Count(p => p.ProgressPercentage < 50),
                    DropoffRate = g.Any()
                        ? (decimal)g.Count(p => p.ProgressPercentage < 50) / g.Count() * 100
                        : 0
                })
                .OrderBy(m => m.Period)
                .Take(12) // Last 12 weeks
                .ToList();

            // Department performance - placeholder (would need HR department data)
            var departmentPerformance = new List<DepartmentPerformanceDto>();

            var dashboard = new LearningAnalyticsDashboardDto
            {
                OverallMetrics = overallMetrics,
                CompletionTrends = completionTrends,
                PopularCourses = popularCourses,
                EngagementMetrics = engagementMetrics,
                DepartmentPerformance = departmentPerformance
            };

            return Result<LearningAnalyticsDashboardDto>.Ok(dashboard);
        }
        catch (Exception ex)
        {
            return Result<LearningAnalyticsDashboardDto>.Fail($"Failed to retrieve learning analytics: {ex.Message}");
        }
    }
}
