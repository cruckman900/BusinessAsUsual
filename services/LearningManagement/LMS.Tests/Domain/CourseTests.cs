using FluentAssertions;
using LMS.Domain.Entities;
using Xunit;

namespace LMS.Tests.Domain;

public class CourseTests
{
    [Fact]
    public void Course_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.Id.Should().NotBeEmpty();
        course.Status.Should().Be(CourseStatus.Draft);
        course.Difficulty.Should().Be(CourseDifficulty.Beginner);
        course.RequiresAssessment.Should().BeTrue();
        course.PassingScore.Should().Be(70);
        course.MaxAttempts.Should().Be(3);
        course.IssuesCertificate.Should().BeTrue();
        course.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Course_CanAddModules()
    {
        // Arrange
        var course = new Course { Title = "Test Course" };
        var module = new Module { Title = "Module 1", OrderIndex = 0 };

        // Act
        course.Modules.Add(module);

        // Assert
        course.Modules.Should().HaveCount(1);
        course.Modules.First().Title.Should().Be("Module 1");
    }

    [Fact]
    public void Module_CanAddLessons()
    {
        // Arrange
        var module = new Module { Title = "Test Module" };
        var lesson = new Lesson { Title = "Lesson 1", OrderIndex = 0 };

        // Act
        module.Lessons.Add(lesson);

        // Assert
        module.Lessons.Should().HaveCount(1);
        module.Lessons.First().Title.Should().Be("Lesson 1");
    }

    [Fact]
    public void ContentBlock_ShouldHaveJsonContent()
    {
        // Arrange & Act
        var block = new ContentBlock
        {
            BlockType = ContentBlockType.Text,
            JsonContent = "{\"content\":\"Hello World\"}",
            OrderIndex = 0
        };

        // Assert
        block.BlockType.Should().Be(ContentBlockType.Text);
        block.JsonContent.Should().Contain("Hello World");
    }
}
