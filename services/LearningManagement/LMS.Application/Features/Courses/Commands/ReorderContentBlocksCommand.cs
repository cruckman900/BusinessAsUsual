using LMS.Application.Common;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands;

public class ReorderContentBlocksCommand : ICommand<Result>
{
    public Guid LessonId { get; set; }
    public Dictionary<Guid, int> BlockOrders { get; set; } = new(); // BlockId -> OrderIndex
}

public class ReorderContentBlocksCommandHandler : ICommandHandler<ReorderContentBlocksCommand, Result>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<ReorderContentBlocksCommandHandler> _logger;

    public ReorderContentBlocksCommandHandler(
        ICourseRepository courseRepository,
        ILogger<ReorderContentBlocksCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(ReorderContentBlocksCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var courses = await _courseRepository.GetAllAsync(cancellationToken);
            var course = courses.FirstOrDefault(c => 
                c.Modules.Any(m => m.Lessons.Any(l => l.Id == command.LessonId)));

            if (course == null)
                return Result.Fail("Lesson not found");

            var lesson = course.Modules
                .SelectMany(m => m.Lessons)
                .First(l => l.Id == command.LessonId);

            foreach (var block in lesson.ContentBlocks)
            {
                if (command.BlockOrders.TryGetValue(block.Id, out int newOrder))
                {
                    block.OrderIndex = newOrder;
                }
            }

            await _courseRepository.UpdateAsync(course, cancellationToken);

            _logger.LogInformation("Reordered content blocks for lesson {LessonId}", command.LessonId);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering content blocks: {LessonId}", command.LessonId);
            return Result.Fail($"Error reordering blocks: {ex.Message}");
        }
    }
}
