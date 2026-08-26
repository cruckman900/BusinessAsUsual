using BusinessAsUsual.Core.Events;
using HR.Domain.Entities;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HR.Application.EventHandlers;

/// <summary>
/// Handles CourseCompletedEvent from LMS and records training completion in HR system.
/// Creates a TrainingCompletion record linked to the employee, enabling training
/// history tracking and compliance reporting.
/// </summary>
public sealed class CourseCompletedEventHandler : IIntegrationEventHandler<CourseCompletedEvent>
{
    private readonly HRDbContext _context;
    private readonly ILogger<CourseCompletedEventHandler> _logger;

    public CourseCompletedEventHandler(HRDbContext context, ILogger<CourseCompletedEventHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task HandleAsync(CourseCompletedEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check for idempotency - don't process the same event twice
            var existing = await _context.TrainingCompletions
                .FirstOrDefaultAsync(tc => tc.SourceEventId == @event.EventId, cancellationToken);

            if (existing != null)
            {
                _logger.LogInformation(
                    "Skipping duplicate CourseCompletedEvent {EventId} for user {UserId} and course {CourseId}",
                    @event.EventId, @event.UserId, @event.CourseId);
                return;
            }

            // Verify employee exists in HR system
            // Parse the UserId string to Guid
            if (!Guid.TryParse(@event.UserId, out var employeeId))
            {
                _logger.LogWarning(
                    "Invalid UserId format {UserId} for course completion event {EventId}",
                    @event.UserId, @event.EventId);
                return;
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);

            if (employee == null)
            {
                _logger.LogWarning(
                    "Employee {UserId} not found in HR system for course completion event {EventId}",
                    @event.UserId, @event.EventId);
                return;
            }

            // Create training completion record
            var completion = new TrainingCompletion
            {
                EmployeeId = employeeId,
                CourseId = @event.CourseId,
                CourseName = @event.CourseName,
                CompletionDate = @event.CompletionDate,
                Score = @event.Score,
                CertificateNumber = @event.CertificateNumber,
                TimeSpentMinutes = @event.TimeSpentMinutes,
                SourceEventId = @event.EventId,
                RecordedAt = DateTime.UtcNow
            };

            _context.TrainingCompletions.Add(completion);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Recorded training completion for employee {EmployeeName} ({EmployeeId}) - Course: {CourseName}, Score: {Score}%, Certificate: {CertificateNumber}",
                employee.FullName, @event.UserId, @event.CourseName, @event.Score, @event.CertificateNumber ?? "N/A");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error handling CourseCompletedEvent {EventId} for user {UserId} and course {CourseId}",
                @event.EventId, @event.UserId, @event.CourseId);
            throw;
        }
    }
}

/// <summary>
/// CourseCompletedEvent from LMS - copy of the event definition for HR consumption.
/// In a real microservices architecture, this would be in a shared contracts library.
/// </summary>
public class CourseCompletedEvent : IntegrationEvent
{
    public string UserId { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public DateTime CompletionDate { get; set; }
    public decimal Score { get; set; }
    public string? CertificateNumber { get; set; }
    public int TimeSpentMinutes { get; set; }
}
