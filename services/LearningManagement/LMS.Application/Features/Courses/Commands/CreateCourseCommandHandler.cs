using LMS.Application.Common;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Courses.Commands;

public class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand, Result<Guid>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<CreateCourseCommandHandler> _logger;

    public CreateCourseCommandHandler(
        ICourseRepository courseRepository,
        ILogger<CreateCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(CreateCourseCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate
            if (string.IsNullOrWhiteSpace(command.Title))
                return Result<Guid>.Fail("Course title is required");

            if (string.IsNullOrWhiteSpace(command.Description))
                return Result<Guid>.Fail("Course description is required");

            if (command.PassingScore < 0 || command.PassingScore > 100)
                return Result<Guid>.Fail("Passing score must be between 0 and 100");

            // Create course
            var course = new Course
            {
                Title = command.Title,
                Description = command.Description,
                Category = command.Category,
                Tags = command.Tags,
                Difficulty = command.Difficulty,
                EstimatedDurationMinutes = command.EstimatedDurationMinutes,
                Status = CourseStatus.Draft,
                RequiresAssessment = command.RequiresAssessment,
                PassingScore = command.PassingScore,
                MaxAttempts = command.MaxAttempts,
                IssuesCertificate = command.IssuesCertificate,
                CertificateValidityDays = command.CertificateValidityDays,
                CreatedBy = command.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _courseRepository.AddAsync(course, cancellationToken);

            _logger.LogInformation("Course created: {CourseId} - {Title}", created.Id, created.Title);

            return Result<Guid>.Ok(created.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course: {Title}", command.Title);
            return Result<Guid>.Fail($"Error creating course: {ex.Message}");
        }
    }
}
