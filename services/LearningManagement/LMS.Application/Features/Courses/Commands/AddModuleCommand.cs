using LMS.Application.Common;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands;

public class AddModuleCommand : ICommand<Result<Guid>>
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
}

public class AddModuleCommandHandler : ICommandHandler<AddModuleCommand, Result<Guid>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<AddModuleCommandHandler> _logger;

    public AddModuleCommandHandler(
        ICourseRepository courseRepository,
        ILogger<AddModuleCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(AddModuleCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var course = await _courseRepository.GetWithModulesAsync(command.CourseId, cancellationToken);
            if (course == null)
                return Result<Guid>.Fail("Course not found");

            if (string.IsNullOrWhiteSpace(command.Title))
                return Result<Guid>.Fail("Module title is required");

            var module = new Module
            {
                CourseId = command.CourseId,
                Title = command.Title,
                Description = command.Description,
                OrderIndex = command.OrderIndex,
                CreatedAt = DateTime.UtcNow
            };

            course.Modules.Add(module);
            await _courseRepository.UpdateAsync(course, cancellationToken);

            _logger.LogInformation("Module added to course {CourseId}: {ModuleId}", command.CourseId, module.Id);

            return Result<Guid>.Ok(module.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding module to course: {CourseId}", command.CourseId);
            return Result<Guid>.Fail($"Error adding module: {ex.Message}");
        }
    }
}
