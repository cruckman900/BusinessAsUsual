using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BusinessAsUsual.Web.Services;

/// <summary>
/// Resets the demo "tenant" database(s) so that logging in with the admin/password demo
/// credentials always presents a fresh, fully-populated dataset. Clears and reseeds the
/// in-process HR and LMS contexts used by the Web shell, and calls the equivalent
/// dev/demo-only reset endpoints on the out-of-process module APIs (Inventory, Sales,
/// Services) so every module's data is refreshed together.
/// </summary>
public class TenantDatabaseResetService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TenantDatabaseResetService> _logger;

    public TenantDatabaseResetService(
        IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory,
        ILogger<TenantDatabaseResetService> logger)
    {
        _serviceProvider = serviceProvider;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Resets all correlating module databases. Safe to call repeatedly; failures in any
    /// one module are logged but do not prevent the others from resetting.
    /// </summary>
    public Task ResetAllAsync() => ResetAllAsync(companyId: null, tenantDbName: null);

    /// <summary>
    /// Resets all correlating module databases for the given tenant. The selected
    /// <paramref name="companyId"/>/<paramref name="tenantDbName"/> are propagated as headers on the
    /// module reset requests so each module can tag/scope the reseeded demo data accordingly.
    /// </summary>
    public async Task ResetAllAsync(Guid? companyId, string? tenantDbName)
    {
        await ResetHRAsync();
        await ResetLMSAsync();
        await ResetViaHttpAsync("InventoryApi", "api/inventory/tenant-reset", companyId, tenantDbName);
        await ResetViaHttpAsync("SalesApi", "api/sales/tenant-reset", companyId, tenantDbName);
        await ResetViaHttpAsync("ServicesApi", "api/services/tenant-reset", companyId, tenantDbName);
        await ResetViaHttpAsync("HrApi", "api/hr/tenant-reset", companyId, tenantDbName);
    }

    private async Task ResetHRAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<HR.Infrastructure.Persistence.HRDbContext>();

            if (dbContext.Database.IsRelational())
            {
                _logger.LogInformation("HR shell database is relational; skipping in-process reset.");
                return;
            }

            dbContext.TrainingCompletions.RemoveRange(dbContext.TrainingCompletions);
            dbContext.EmployeeDepartments.RemoveRange(dbContext.EmployeeDepartments);
            dbContext.DepartmentManagers.RemoveRange(dbContext.DepartmentManagers);
            dbContext.Employees.RemoveRange(dbContext.Employees);
            dbContext.Departments.RemoveRange(dbContext.Departments);
            await dbContext.SaveChangesAsync();

            await Program.SeedHRDataAsync(_serviceProvider);
            _logger.LogInformation("HR shell database reset and reseeded.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reset HR shell database.");
        }
    }

    private async Task ResetLMSAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LMSDbContext>();

            context.EarnedBadges.RemoveRange(context.EarnedBadges);
            context.LearnerGamifications.RemoveRange(context.LearnerGamifications);
            context.Notifications.RemoveRange(context.Notifications);
            context.Certificates.RemoveRange(context.Certificates);
            context.DetailedLearnerProgress.RemoveRange(context.DetailedLearnerProgress);
            context.CourseAssignments.RemoveRange(context.CourseAssignments);
            context.LearnerProgresses.RemoveRange(context.LearnerProgresses);
            context.CourseCompletions.RemoveRange(context.CourseCompletions);
            context.Assignments.RemoveRange(context.Assignments);
            context.LearningPathEnrollments.RemoveRange(context.LearningPathEnrollments);
            context.LearningPathCourses.RemoveRange(context.LearningPathCourses);
            context.LearningPaths.RemoveRange(context.LearningPaths);
            context.CoursePrerequisites.RemoveRange(context.CoursePrerequisites);
            context.Answers.RemoveRange(context.Answers);
            context.QuizAttempts.RemoveRange(context.QuizAttempts);
            context.QuestionOptions.RemoveRange(context.QuestionOptions);
            context.Questions.RemoveRange(context.Questions);
            context.Quizzes.RemoveRange(context.Quizzes);
            context.ContentBlocks.RemoveRange(context.ContentBlocks);
            context.Lessons.RemoveRange(context.Lessons);
            context.Modules.RemoveRange(context.Modules);
            context.MediaAssets.RemoveRange(context.MediaAssets);
            context.Courses.RemoveRange(context.Courses);
            await context.SaveChangesAsync();

            await Program.SeedLMSDataAsync(_serviceProvider);
            _logger.LogInformation("LMS shell database reset and reseeded.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reset LMS shell database.");
        }
    }

    private async Task ResetViaHttpAsync(string httpClientName, string relativePath, Guid? companyId = null, string? tenantDbName = null)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(httpClientName);
            using var request = new HttpRequestMessage(HttpMethod.Post, relativePath);

            if (companyId.HasValue)
            {
                request.Headers.Add("X-Company-Id", companyId.Value.ToString());
            }
            if (!string.IsNullOrWhiteSpace(tenantDbName))
            {
                request.Headers.Add("X-Tenant-Db", tenantDbName);
            }

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var response = await client.SendAsync(request, cts.Token);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Reset {ClientName} tenant database via {Path}.", httpClientName, relativePath);
            }
            else
            {
                _logger.LogWarning("Reset request to {ClientName} ({Path}) returned {StatusCode}.", httpClientName, relativePath, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not reset {ClientName} tenant database (service may be offline).", httpClientName);
        }
    }
}
