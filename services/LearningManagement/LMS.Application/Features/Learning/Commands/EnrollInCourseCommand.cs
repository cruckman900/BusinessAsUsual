using LMS.Application.Common;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Learning.Commands;

public class EnrollInCourseCommand : ICommand<Result<Guid>>
{
    public Guid CourseId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
}

public class EnrollInCourseCommandHandler : ICommandHandler<EnrollInCourseCommand, Result<Guid>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILearnerProgressRepository _progressRepository;
    private readonly ILogger<EnrollInCourseCommandHandler> _logger;

    public EnrollInCourseCommandHandler(
        ICourseRepository courseRepository,
        ILearnerProgressRepository progressRepository,
        ILogger<EnrollInCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _progressRepository = progressRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(EnrollInCourseCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(command.CourseId, cancellationToken);
            if (course == null)
                return Result<Guid>.Fail("Course not found");

            if (course.Status != CourseStatus.Published)
                return Result<Guid>.Fail("Cannot enroll in unpublished course");

            // Check if already enrolled
            var existing = await _progressRepository.GetByEmployeeAndCourseAsync(command.EmployeeId, command.CourseId, cancellationToken);
            if (existing != null)
                return Result<Guid>.Fail("Already enrolled in this course");

            var progress = new LearnerProgress
            {
                EmployeeId = command.EmployeeId,
                CourseId = command.CourseId,
                ProgressPercentage = 0,
                LastAccessedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _progressRepository.AddAsync(progress, cancellationToken);

            _logger.LogInformation("Employee {EmployeeId} enrolled in course {CourseId}", command.EmployeeId, command.CourseId);

            return Result<Guid>.Ok(created.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enrolling in course: {CourseId}", command.CourseId);
            return Result<Guid>.Fail($"Error enrolling: {ex.Message}");
        }
    }
}
