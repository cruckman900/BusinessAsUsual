using LMS.Application.Common;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands;

public class UpdateCourseCommandHandler : ICommandHandler<UpdateCourseCommand, Result>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<UpdateCourseCommandHandler> _logger;

    public UpdateCourseCommandHandler(
        ICourseRepository courseRepository,
        ILogger<UpdateCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateCourseCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(command.CourseId, cancellationToken);
            if (course == null)
                return Result.Fail("Course not found");

            // Validate
            if (string.IsNullOrWhiteSpace(command.Title))
                return Result.Fail("Course title is required");

            if (string.IsNullOrWhiteSpace(command.Description))
                return Result.Fail("Course description is required");

            // Update
            course.Title = command.Title;
            course.Description = command.Description;
            course.ThumbnailUrl = command.ThumbnailUrl;
            course.Category = command.Category;
            course.Tags = command.Tags;
            course.Difficulty = command.Difficulty;
            course.EstimatedDurationMinutes = command.EstimatedDurationMinutes;
            course.RequiresAssessment = command.RequiresAssessment;
            course.PassingScore = command.PassingScore;
            course.MaxAttempts = command.MaxAttempts;
            course.IssuesCertificate = command.IssuesCertificate;
            course.CertificateValidityDays = command.CertificateValidityDays;
            course.UpdatedBy = command.UpdatedBy;
            course.UpdatedAt = DateTime.UtcNow;

            await _courseRepository.UpdateAsync(course, cancellationToken);

            _logger.LogInformation("Course updated: {CourseId}", command.CourseId);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating course: {CourseId}", command.CourseId);
            return Result.Fail($"Error updating course: {ex.Message}");
        }
    }
}
