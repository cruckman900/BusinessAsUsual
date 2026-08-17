using LMS.Application.Common;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands;

public class AddContentBlockCommand : ICommand<Result<Guid>>
{
    public Guid LessonId { get; set; }
    public ContentBlockType BlockType { get; set; }
    public int OrderIndex { get; set; }
    public string JsonContent { get; set; } = "{}";
    public Guid? QuizId { get; set; }
}

public class AddContentBlockCommandHandler : ICommandHandler<AddContentBlockCommand, Result<Guid>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<AddContentBlockCommandHandler> _logger;

    public AddContentBlockCommandHandler(
        ICourseRepository courseRepository,
        ILogger<AddContentBlockCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(AddContentBlockCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Find the lesson
            var courses = await _courseRepository.GetAllAsync(cancellationToken);
            var course = courses.FirstOrDefault(c => 
                c.Modules.Any(m => m.Lessons.Any(l => l.Id == command.LessonId)));

            if (course == null)
                return Result<Guid>.Fail("Lesson not found");

            var lesson = course.Modules
                .SelectMany(m => m.Lessons)
                .First(l => l.Id == command.LessonId);

            var contentBlock = new ContentBlock
            {
                LessonId = command.LessonId,
                BlockType = command.BlockType,
                OrderIndex = command.OrderIndex,
                JsonContent = command.JsonContent,
                QuizId = command.QuizId,
                CreatedAt = DateTime.UtcNow
            };

            lesson.ContentBlocks.Add(contentBlock);
            await _courseRepository.UpdateAsync(course, cancellationToken);

            _logger.LogInformation("Content block added to lesson {LessonId}: {BlockId} ({BlockType})", 
                command.LessonId, contentBlock.Id, command.BlockType);

            return Result<Guid>.Ok(contentBlock.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding content block to lesson: {LessonId}", command.LessonId);
            return Result<Guid>.Fail($"Error adding content block: {ex.Message}");
        }
    }
}
