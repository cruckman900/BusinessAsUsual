using LMS.Application.Common;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Learning.Commands;

public class CompleteCourseCommand : ICommand<Result<Guid>>
{
    public string EmployeeId { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public Guid? FinalAssessmentAttemptId { get; set; }
    public decimal FinalScore { get; set; }
    public bool Passed { get; set; }
}

public class CompleteCourseCommandHandler : ICommandHandler<CompleteCourseCommand, Result<Guid>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseCompletionRepository _completionRepository;
    private readonly ILearnerProgressRepository _progressRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ICommandHandler<IssueCertificateCommand, Result<Certificate>> _issueCertificateHandler;
    private readonly IEventBus _eventBus;
    private readonly ILogger<CompleteCourseCommandHandler> _logger;

    public CompleteCourseCommandHandler(
        ICourseRepository courseRepository,
        ICourseCompletionRepository completionRepository,
        ILearnerProgressRepository progressRepository,
        IAssignmentRepository assignmentRepository,
        ICommandHandler<IssueCertificateCommand, Result<Certificate>> issueCertificateHandler,
        IEventBus eventBus,
        ILogger<CompleteCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _completionRepository = completionRepository;
        _progressRepository = progressRepository;
        _assignmentRepository = assignmentRepository;
        _issueCertificateHandler = issueCertificateHandler;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(CompleteCourseCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(command.CourseId, cancellationToken);
            if (course == null)
                return Result<Guid>.Fail("Course not found");

            var progress = await _progressRepository.GetByEmployeeAndCourseAsync(command.EmployeeId, command.CourseId, cancellationToken);
            if (progress == null)
                return Result<Guid>.Fail("Progress not found");

            // Check if already completed
            var existing = await _completionRepository.GetByEmployeeAndCourseAsync(command.EmployeeId, command.CourseId, cancellationToken);
            if (existing != null)
                return Result<Guid>.Fail("Course already completed");

            // Create completion record
            var completion = new CourseCompletion
            {
                CourseId = command.CourseId,
                EmployeeId = command.EmployeeId,
                StartedAt = progress.CreatedAt,
                CompletedAt = DateTime.UtcNow,
                FinalAssessmentAttemptId = command.FinalAssessmentAttemptId,
                FinalScore = command.FinalScore,
                Passed = command.Passed,
                CertificateIssued = false, // Will be issued separately if needed
                ProgressData = System.Text.Json.JsonSerializer.Serialize(new
                {
                    completedModules = progress.CompletedModules,
                    completedLessons = progress.CompletedLessons,
                    completedQuizzes = progress.CompletedQuizzes
                }),
                CreatedAt = DateTime.UtcNow
            };

            var created = await _completionRepository.AddAsync(completion, cancellationToken);

            // Update any assignments
            var assignments = await _assignmentRepository.GetByEmployeeIdAsync(command.EmployeeId, cancellationToken);
            var courseAssignment = assignments.FirstOrDefault(a => a.CourseId == command.CourseId && a.Status != AssignmentStatus.Completed);
            if (courseAssignment != null)
            {
                courseAssignment.Status = AssignmentStatus.Completed;
                courseAssignment.CompletedAt = DateTime.UtcNow;
                courseAssignment.CompletionId = created.Id;
                await _assignmentRepository.UpdateAsync(courseAssignment, cancellationToken);
            }

            // Publish TrainingCompletedIntegrationEvent to HR
            var integrationEvent = new TrainingCompletedIntegrationEvent
            {
                CompletionId = created.Id,
                CourseId = course.Id,
                CourseTitle = course.Title,
                EmployeeId = command.EmployeeId,
                CompletedDate = completion.CompletedAt,
                FinalScore = command.FinalScore,
                Passed = command.Passed,
                DurationMinutes = (int)(completion.CompletedAt - completion.StartedAt).TotalMinutes
            };
            await _eventBus.PublishAsync(integrationEvent, cancellationToken);

            // Auto-issue certificate if passed and course offers one
            if (command.Passed && course.IssuesCertificate)
            {
                var certCommand = new IssueCertificateCommand
                {
                    CourseId = command.CourseId,
                    EmployeeId = command.EmployeeId,
                    FinalScore = command.FinalScore,
                    ExpiryDate = course.CertificateValidityDays.HasValue 
                        ? DateTime.UtcNow.AddDays(course.CertificateValidityDays.Value) 
                        : null
                };

                var certResult = await _issueCertificateHandler.HandleAsync(certCommand, cancellationToken);
                if (certResult.Success)
                {
                    completion.CertificateIssued = true;
                    await _completionRepository.UpdateAsync(completion, cancellationToken);
                    _logger.LogInformation("Certificate issued for employee {EmployeeId} completing course {CourseId}", 
                        command.EmployeeId, command.CourseId);
                }
            }

            _logger.LogInformation("Employee {EmployeeId} completed course {CourseId} with score {Score} (Passed: {Passed})", 
                command.EmployeeId, command.CourseId, command.FinalScore, command.Passed);

            return Result<Guid>.Ok(created.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing course: {CourseId}", command.CourseId);
            return Result<Guid>.Fail($"Error completing course: {ex.Message}");
        }
    }
}
