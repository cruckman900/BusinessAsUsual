using FluentAssertions;
using LMS.Application.Common;
using LMS.Application.Features.Courses.Commands;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LMS.Tests.Application;

public class CreateCourseCommandHandlerTests
{
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly Mock<ILogger<CreateCourseCommandHandler>> _mockLogger;
    private readonly CreateCourseCommandHandler _handler;

    public CreateCourseCommandHandlerTests()
    {
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockLogger = new Mock<ILogger<CreateCourseCommandHandler>>();
        _handler = new CreateCourseCommandHandler(_mockCourseRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsSuccessWithCourseId()
    {
        // Arrange
        var command = new CreateCourseCommand
        {
            Title = "Test Course",
            Description = "Test Description",
            Difficulty = CourseDifficulty.Beginner,
            EstimatedDurationMinutes = 60
        };

        var createdCourse = new Course
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Description = command.Description
        };

        _mockCourseRepository
            .Setup(r => r.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdCourse);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(createdCourse.Id);
        _mockCourseRepository.Verify(r => r.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_MissingTitle_ReturnsFailure()
    {
        // Arrange
        var command = new CreateCourseCommand
        {
            Title = "",
            Description = "Test Description"
        };

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("title is required");
        _mockCourseRepository.Verify(r => r.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_MissingDescription_ReturnsFailure()
    {
        // Arrange
        var command = new CreateCourseCommand
        {
            Title = "Test Course",
            Description = ""
        };

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("description is required");
    }

    [Fact]
    public async Task HandleAsync_InvalidPassingScore_ReturnsFailure()
    {
        // Arrange
        var command = new CreateCourseCommand
        {
            Title = "Test Course",
            Description = "Description",
            PassingScore = 150 // Invalid
        };

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Passing score must be between 0 and 100");
    }
}
