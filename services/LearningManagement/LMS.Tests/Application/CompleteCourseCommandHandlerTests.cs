using FluentAssertions;
using LMS.Application.Common;
using LMS.Application.Features.Learning.Commands;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LMS.Tests.Application;

public class CompleteCourseCommandHandlerTests
{
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly Mock<ICourseCompletionRepository> _mockCompletionRepository;
    private readonly Mock<ILearnerProgressRepository> _mockProgressRepository;
    private readonly Mock<IAssignmentRepository> _mockAssignmentRepository;
    private readonly Mock<ICommandHandler<IssueCertificateCommand, Result<Certificate>>> _mockIssueCertificateHandler;
    private readonly Mock<IEventBus> _mockEventBus;
    private readonly Mock<ILogger<CompleteCourseCommandHandler>> _mockLogger;
    private readonly CompleteCourseCommandHandler _handler;

    public CompleteCourseCommandHandlerTests()
    {
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockCompletionRepository = new Mock<ICourseCompletionRepository>();
        _mockProgressRepository = new Mock<ILearnerProgressRepository>();
        _mockAssignmentRepository = new Mock<IAssignmentRepository>();
        _mockIssueCertificateHandler = new Mock<ICommandHandler<IssueCertificateCommand, Result<Certificate>>>();
        _mockEventBus = new Mock<IEventBus>();
        _mockLogger = new Mock<ILogger<CompleteCourseCommandHandler>>();

        _handler = new CompleteCourseCommandHandler(
            _mockCourseRepository.Object,
            _mockCompletionRepository.Object,
            _mockProgressRepository.Object,
            _mockAssignmentRepository.Object,
            _mockIssueCertificateHandler.Object,
            _mockEventBus.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidCompletion_ReturnsSuccessAndPublishesEvent()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var employeeId = "EMP123";

        var command = new CompleteCourseCommand
        {
            CourseId = courseId,
            EmployeeId = employeeId,
            FinalScore = 85,
            Passed = true
        };

        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            Status = CourseStatus.Published
        };

        var progress = new LearnerProgress
        {
            EmployeeId = employeeId,
            CourseId = courseId,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CompletedModules = new List<Guid>(),
            CompletedLessons = new List<Guid>(),
            CompletedQuizzes = new List<Guid>()
        };

        var assignments = new List<Assignment>
        {
            new Assignment
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                EmployeeId = employeeId,
                Status = AssignmentStatus.InProgress
            }
        };

        _mockCourseRepository.Setup(r => r.GetByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _mockProgressRepository.Setup(r => r.GetByEmployeeAndCourseAsync(employeeId, courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(progress);
        _mockCompletionRepository.Setup(r => r.GetByEmployeeAndCourseAsync(employeeId, courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CourseCompletion?)null);
        _mockCompletionRepository.Setup(r => r.AddAsync(It.IsAny<CourseCompletion>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CourseCompletion c, CancellationToken ct) => { c.Id = Guid.NewGuid(); return c; });
        _mockAssignmentRepository.Setup(r => r.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignments);
        _mockAssignmentRepository.Setup(r => r.UpdateAsync(It.IsAny<Assignment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockEventBus.Setup(e => e.PublishAsync(It.IsAny<TrainingCompletedIntegrationEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        _mockCompletionRepository.Verify(r => r.AddAsync(It.IsAny<CourseCompletion>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockAssignmentRepository.Verify(r => r.UpdateAsync(It.Is<Assignment>(a => a.Status == AssignmentStatus.Completed), It.IsAny<CancellationToken>()), Times.Once);
        _mockEventBus.Verify(e => e.PublishAsync(It.Is<TrainingCompletedIntegrationEvent>(evt =>
            evt.CourseId == courseId &&
            evt.EmployeeId == employeeId &&
            evt.FinalScore == 85 &&
            evt.Passed == true), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_CourseNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new CompleteCourseCommand
        {
            CourseId = Guid.NewGuid(),
            EmployeeId = "EMP123"
        };

        _mockCourseRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Course?)null);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Course not found");
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<TrainingCompletedIntegrationEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_FailingScore_PublishesEventWithPassedFalse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var employeeId = "EMP123";

        var command = new CompleteCourseCommand
        {
            CourseId = courseId,
            EmployeeId = employeeId,
            FinalScore = 50,
            Passed = false // Below default passing score of 70
        };

        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            Status = CourseStatus.Published,
            PassingScore = 70
        };

        var progress = new LearnerProgress
        {
            EmployeeId = employeeId,
            CourseId = courseId,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CompletedModules = new List<Guid>(),
            CompletedLessons = new List<Guid>(),
            CompletedQuizzes = new List<Guid>()
        };

        _mockCourseRepository.Setup(r => r.GetByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _mockProgressRepository.Setup(r => r.GetByEmployeeAndCourseAsync(employeeId, courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(progress);
        _mockCompletionRepository.Setup(r => r.GetByEmployeeAndCourseAsync(employeeId, courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CourseCompletion?)null);
        _mockCompletionRepository.Setup(r => r.AddAsync(It.IsAny<CourseCompletion>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CourseCompletion c, CancellationToken ct) => { c.Id = Guid.NewGuid(); return c; });
        _mockAssignmentRepository.Setup(r => r.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Assignment>());

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Success.Should().BeTrue();
        _mockEventBus.Verify(e => e.PublishAsync(It.Is<TrainingCompletedIntegrationEvent>(evt =>
            evt.Passed == false &&
            evt.FinalScore == 50), It.IsAny<CancellationToken>()), Times.Once);
    }
}
