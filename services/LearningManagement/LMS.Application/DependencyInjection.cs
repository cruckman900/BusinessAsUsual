using Microsoft.Extensions.DependencyInjection;
using LMS.Application.Common;
using LMS.Application.Features.Courses.Commands;
using LMS.Application.Features.Courses.Queries;
using LMS.Application.Features.Learning.Commands;
using LMS.Application.Features.Learning.Queries;
using LMS.Application.Features.Notifications.Commands;
using LMS.Application.Features.Notifications.Queries;
using LMS.Application.Features.Media.Commands;
using LMS.Application.Features.Analytics.Queries;
using LMS.Application.Features.LearningPaths.Commands;
using LMS.Application.Features.LearningPaths.Queries;
using LMS.Application.Features.Gamification.Queries;
using LMS.Domain.Entities;
using LMS.Domain.DTOs;

namespace LMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddLMSApplication(this IServiceCollection services)
    {
        // Register Course command handlers
        services.AddScoped<CreateCourseCommandHandler>();
        services.AddScoped<UpdateCourseCommandHandler>();
        services.AddScoped<PublishCourseCommandHandler>();
        services.AddScoped<DeleteCourseCommandHandler>();
        services.AddScoped<AddModuleCommandHandler>();
        services.AddScoped<AddLessonCommandHandler>();
        services.AddScoped<AddContentBlockCommandHandler>();

        // Register Course query handlers
        services.AddScoped<GetCourseQueryHandler>();
        services.AddScoped<GetCourseBuilderQueryHandler>();
        services.AddScoped<ListCoursesQueryHandler>();

        // Register Learning command handlers
        services.AddScoped<EnrollInCourseCommandHandler>();
        services.AddScoped<StartCourseCommandHandler>();
        services.AddScoped<UpdateProgressCommandHandler>();
        services.AddScoped<CompleteCourseCommandHandler>();
        services.AddScoped<AssignCourseCommandHandler>();
        services.AddScoped<SubmitQuizAttemptCommandHandler>();
        services.AddScoped<IssueCertificateCommandHandler>();
        services.AddScoped<GenerateCertificatePdfCommandHandler>();

        // Register Notification handlers
        services.AddScoped<SendNotificationCommandHandler>();
        services.AddScoped<GetMyNotificationsQueryHandler>();

        // Register Media handlers
        services.AddScoped<UploadMediaCommandHandler>();

        // Register Analytics query handlers
        services.AddScoped<GetQuizAnalyticsQueryHandler>();
        services.AddScoped<GetLearnerQuizHistoryQueryHandler>();
        services.AddScoped<GetQuizPerformanceSummaryQueryHandler>();
        services.AddScoped<GetLearningAnalyticsDashboardQueryHandler>();

        // Register Learning Path handlers
        services.AddScoped<CreateLearningPathCommandHandler>();
        services.AddScoped<GetLearningPathsQueryHandler>();

        // Register Gamification handlers
        services.AddScoped<GetGamificationStatsQueryHandler>();

        // Register command handlers as ICommandHandler interfaces for DI
        services.AddScoped<ICommandHandler<AssignCourseCommand, Result<List<Guid>>>>(
            sp => sp.GetRequiredService<AssignCourseCommandHandler>());
        services.AddScoped<ICommandHandler<SubmitQuizAttemptCommand, Result<QuizAttemptResult>>>(
            sp => sp.GetRequiredService<SubmitQuizAttemptCommandHandler>());
        services.AddScoped<ICommandHandler<IssueCertificateCommand, Result<Certificate>>>(
            sp => sp.GetRequiredService<IssueCertificateCommandHandler>());
        services.AddScoped<ICommandHandler<SendNotificationCommand, Result<Notification>>>(
            sp => sp.GetRequiredService<SendNotificationCommandHandler>());

        // Register Learning query handlers
        services.AddScoped<GetMyCoursesQueryHandler>();
        services.AddScoped<GetProgressQueryHandler>();
        services.AddScoped<GetQuizAttemptsQueryHandler>();
        services.AddScoped<GetMyCertificatesQueryHandler>();

        // Register query handlers as IQueryHandler interfaces for DI
        services.AddScoped<IQueryHandler<GetMyCertificatesQuery, List<Certificate>>>(
            sp => sp.GetRequiredService<GetMyCertificatesQueryHandler>());
        services.AddScoped<IQueryHandler<GetMyNotificationsQuery, List<Notification>>>(
            sp => sp.GetRequiredService<GetMyNotificationsQueryHandler>());

        // Register Analytics query handlers as IQueryHandler interfaces
        services.AddScoped<IQueryHandler<GetQuizAnalyticsQuery, Result<QuizAnalyticsDto>>>(
            sp => sp.GetRequiredService<GetQuizAnalyticsQueryHandler>());
        services.AddScoped<IQueryHandler<GetLearnerQuizHistoryQuery, Result<List<LearnerQuizHistoryDto>>>>(
            sp => sp.GetRequiredService<GetLearnerQuizHistoryQueryHandler>());
        services.AddScoped<IQueryHandler<GetQuizPerformanceSummaryQuery, Result<QuizPerformanceSummaryDto>>>(
            sp => sp.GetRequiredService<GetQuizPerformanceSummaryQueryHandler>());
        services.AddScoped<IQueryHandler<GetLearningAnalyticsDashboardQuery, Result<LearningAnalyticsDashboardDto>>>(
            sp => sp.GetRequiredService<GetLearningAnalyticsDashboardQueryHandler>());

        // Register Learning Path handlers as interfaces
        services.AddScoped<ICommandHandler<CreateLearningPathCommand, Result<Guid>>>(
            sp => sp.GetRequiredService<CreateLearningPathCommandHandler>());
        services.AddScoped<IQueryHandler<GetLearningPathsQuery, Result<List<LearningPathDto>>>>(
            sp => sp.GetRequiredService<GetLearningPathsQueryHandler>());

        // Register Gamification handlers as interfaces
        services.AddScoped<IQueryHandler<GetGamificationStatsQuery, Result<GamificationStatsDto>>>(
            sp => sp.GetRequiredService<GetGamificationStatsQueryHandler>());

        return services;
    }
}
