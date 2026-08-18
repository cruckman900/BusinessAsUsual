using LMS.Infrastructure.Persistence;
using LMS.Infrastructure.Storage;
using LMS.Infrastructure.Repositories;
using LMS.Infrastructure.Data;
using LMS.Infrastructure.Services;
using LMS.Application.Services;
using LMS.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLMSInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        var connectionString = configuration.GetConnectionString("LMSDatabase") ?? "Data Source=lms.db";
        services.AddDbContext<LMSDbContext>(options =>
            options.UseSqlite(connectionString));

        // Add Blob Storage
        services.AddScoped<IBlobStorageService, LocalFileStorageService>();

        // Add Services
        services.AddScoped<ICertificateGenerator, CertificateGenerator>();
        services.AddScoped<ICertificatePdfService, CertificatePdfService>();
        services.AddScoped<IMediaStorageService, MediaStorageService>();

        // Add Repositories
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        services.AddScoped<ICourseCompletionRepository, CourseCompletionRepository>();
        services.AddScoped<ILearnerProgressRepository, LearnerProgressRepository>();
        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();
        services.AddScoped<ICertificateRepository, CertificateRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IMediaAssetRepository, MediaAssetRepository>();

        // Add Seed Data
        services.AddScoped<LMSSeedData>();

        return services;
    }
}

