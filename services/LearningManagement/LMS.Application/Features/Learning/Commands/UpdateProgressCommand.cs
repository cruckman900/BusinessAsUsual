using LMS.Application.Common;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Learning.Commands;

public class UpdateProgressCommand : ICommand<Result>
{
    public string EmployeeId { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public Guid? CurrentModuleId { get; set; }
    public Guid? CurrentLessonId { get; set; }
    public Guid? CompletedLessonId { get; set; }
}

public class UpdateProgressCommandHandler : ICommandHandler<UpdateProgressCommand, Result>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILearnerProgressRepository _progressRepository;
    private readonly ILogger<UpdateProgressCommandHandler> _logger;

    public UpdateProgressCommandHandler(
        ICourseRepository courseRepository,
        ILearnerProgressRepository progressRepository,
        ILogger<UpdateProgressCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _progressRepository = progressRepository;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(UpdateProgressCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var progress = await _progressRepository.GetByEmployeeAndCourseAsync(command.EmployeeId, command.CourseId, cancellationToken);
            if (progress == null)
                return Result.Fail("Progress not found");

            var course = await _courseRepository.GetWithFullStructureAsync(command.CourseId, cancellationToken);
            if (course == null)
                return Result.Fail("Course not found");

            // Update current position
            if (command.CurrentModuleId.HasValue)
                progress.CurrentModuleId = command.CurrentModuleId;

            if (command.CurrentLessonId.HasValue)
                progress.CurrentLessonId = command.CurrentLessonId;

            // Mark lesson as completed
            if (command.CompletedLessonId.HasValue && !progress.CompletedLessons.Contains(command.CompletedLessonId.Value))
            {
                progress.CompletedLessons.Add(command.CompletedLessonId.Value);
            }

            // Calculate progress percentage
            var totalLessons = course.Modules.SelectMany(m => m.Lessons).Count();
            if (totalLessons > 0)
            {
                progress.ProgressPercentage = (int)((progress.CompletedLessons.Count / (double)totalLessons) * 100);
            }

            progress.LastAccessedAt = DateTime.UtcNow;
            await _progressRepository.UpdateAsync(progress, cancellationToken);

            _logger.LogInformation("Updated progress for employee {EmployeeId} in course {CourseId}: {Progress}%", 
                command.EmployeeId, command.CourseId, progress.ProgressPercentage);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating progress: {CourseId}", command.CourseId);
            return Result.Fail($"Error updating progress: {ex.Message}");
        }
    }
}
