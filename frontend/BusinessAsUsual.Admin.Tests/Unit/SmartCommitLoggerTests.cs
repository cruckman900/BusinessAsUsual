using BusinessAsUsual.Admin.Services;
using FluentAssertions;

namespace BusinessAsUsual.Admin.Tests.Unit;

public class SmartCommitLoggerTests
{
    private SmartCommitLogger NewLogger() => new();

    [Fact]
    public void Log_CreatesLogsDirectory()
    {
        // Arrange
        var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);
        var originalDir = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(testDir);

        try
        {
            var logger = NewLogger();

            // Act
            logger.Log("Test message");

            // Assert
            Directory.Exists(Path.Combine(testDir, "Logs")).Should().BeTrue();
        }
        finally
        {
            // Cleanup
            Directory.SetCurrentDirectory(originalDir);
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, true);
        }
    }

    [Fact]
    public void Log_AppendsMessageToFile()
    {
        // Arrange
        var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);
        var originalDir = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(testDir);

        try
        {
            var logger = NewLogger();
            var testMessage = "Test commit message";

            // Act
            logger.Log(testMessage);

            // Assert
            var logPath = Path.Combine(testDir, "Logs", "smart-commits.txt");
            File.Exists(logPath).Should().BeTrue();
            var content = File.ReadAllText(logPath);
            content.Should().Contain(testMessage);
        }
        finally
        {
            // Cleanup
            Directory.SetCurrentDirectory(originalDir);
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, true);
        }
    }

    [Fact]
    public void Log_AppendsMultipleMessages()
    {
        // Arrange
        var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);
        var originalDir = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(testDir);

        try
        {
            var logger = NewLogger();
            var message1 = "First commit";
            var message2 = "Second commit";

            // Act
            logger.Log(message1);
            logger.Log(message2);

            // Assert
            var logPath = Path.Combine(testDir, "Logs", "smart-commits.txt");
            var content = File.ReadAllText(logPath);
            content.Should().Contain(message1);
            content.Should().Contain(message2);
        }
        finally
        {
            // Cleanup
            Directory.SetCurrentDirectory(originalDir);
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, true);
        }
    }

    [Fact]
    public void Log_AddsNewlineAfterMessage()
    {
        // Arrange
        var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);
        var originalDir = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(testDir);

        try
        {
            var logger = NewLogger();

            // Act
            logger.Log("Test");

            // Assert
            var logPath = Path.Combine(testDir, "Logs", "smart-commits.txt");
            var content = File.ReadAllText(logPath);
            content.Should().EndWith(Environment.NewLine);
        }
        finally
        {
            // Cleanup
            Directory.SetCurrentDirectory(originalDir);
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, true);
        }
    }
}
