using LMS.Application.Common;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands;

public class AddLessonCommand : ICommand<Result<Guid>>
{
    public Guid ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public int EstimatedDurationMinutes { get; set; }
}

public class AddLessonCommandHandler : ICommandHandler<AddLessonCommand, Result<Guid>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<AddLessonCommandHandler> _logger;

    public AddLessonCommandHandler(
        ICourseRepository courseRepository,
        ILogger<AddLessonCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(AddLessonCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Find the module (we'll need to get course with full structure)
            var courses = await _courseRepository.GetAllAsync(cancellationToken);
            var course = courses.FirstOrDefault(c => c.Modules.Any(m => m.Id == command.ModuleId));

            if (course == null)
                return Result<Guid>.Fail("Module not found");

            var module = course.Modules.First(m => m.Id == command.ModuleId);

            if (string.IsNullOrWhiteSpace(command.Title))
                return Result<Guid>.Fail("Lesson title is required");

            var lesson = new Lesson
            {
                ModuleId = command.ModuleId,
                Title = command.Title,
                Description = command.Description,
                OrderIndex = command.OrderIndex,
                EstimatedDurationMinutes = command.EstimatedDurationMinutes,
                CreatedAt = DateTime.UtcNow
            };

            module.Lessons.Add(lesson);
            await _courseRepository.UpdateAsync(course, cancellationToken);

            _logger.LogInformation("Lesson added to module {ModuleId}: {LessonId}", command.ModuleId, lesson.Id);

            return Result<Guid>.Ok(lesson.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding lesson to module: {ModuleId}", command.ModuleId);
            return Result<Guid>.Fail($"Error adding lesson: {ex.Message}");
        }
    }
}
