using FluentAssertions;
using LMS.Application.Features.Learning.Commands;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using BusinessAsUsual.Core.Events;
using BusinessAsUsual.Core.Events.Integration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LMS.Tests.Application;

public class AssignCourseCommandHandlerTests
{
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly Mock<IAssignmentRepository> _mockAssignmentRepository;
    private readonly Mock<IEventBus> _mockEventBus;
    private readonly Mock<ILogger<AssignCourseCommandHandler>> _mockLogger;
    private readonly AssignCourseCommandHandler _handler;

    public AssignCourseCommandHandlerTests()
    {
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockAssignmentRepository = new Mock<IAssignmentRepository>();
        _mockEventBus = new Mock<IEventBus>();
        _mockLogger = new Mock<ILogger<AssignCourseCommandHandler>>();

        _handler = new AssignCourseCommandHandler(
            _mockCourseRepository.Object,
            _mockAssignmentRepository.Object,
            _mockEventBus.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidAssignment_AssignsToAllEmployeesAndPublishesEvents()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var employeeIds = new List<string> { "EMP001", "EMP002", "EMP003" };

        var command = new AssignCourseCommand
        {
            CourseId = courseId,
            EmployeeIds = employeeIds,
            DueDate = DateTime.UtcNow.AddDays(30),
            AssignedBy = "ADMIN001",
            IsMandatory = true
        };

        var course = new Course
        {
            Id = courseId,
            Title = "Safety Training",
            Status = CourseStatus.Published
        };

        _mockCourseRepository.Setup(r => r.GetByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _mockAssignmentRepository.Setup(r => r.AddAsync(It.IsAny<Assignment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Assignment a, CancellationToken ct) => { a.Id = Guid.NewGuid(); return a; });

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(3);
        _mockAssignmentRepository.Verify(r => r.AddAsync(It.IsAny<Assignment>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _mockEventBus.Verify(e => e.PublishAsync(It.IsAny<TrainingAssignedIntegrationEvent>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact]
    public async Task HandleAsync_CourseNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new AssignCourseCommand
        {
            CourseId = Guid.NewGuid(),
            EmployeeIds = new List<string> { "EMP001" }
        };

        _mockCourseRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Course?)null);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Course not found");
        _mockAssignmentRepository.Verify(r => r.AddAsync(It.IsAny<Assignment>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_UnpublishedCourse_ReturnsFailure()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var command = new AssignCourseCommand
        {
            CourseId = courseId,
            EmployeeIds = new List<string> { "EMP001" }
        };

        var course = new Course
        {
            Id = courseId,
            Title = "Draft Course",
            Status = CourseStatus.Draft
        };

        _mockCourseRepository.Setup(r => r.GetByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Cannot assign unpublished course");
        _mockAssignmentRepository.Verify(r => r.AddAsync(It.IsAny<Assignment>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
