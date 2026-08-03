using BusinessAsUsual.Admin.Models;
using BusinessAsUsual.Admin.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace BusinessAsUsual.Admin.Tests.Unit;

public class SystemSettingsServiceTests
{
    private SystemSettingsService NewService(string tempPath)
    {
        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(e => e.ContentRootPath).Returns(tempPath);
        return new SystemSettingsService(mockEnv.Object);
    }

    [Fact]
    public void Load_ReturnsDefaultSettings_WhenFileDoesNotExist()
    {
        // Arrange
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var service = NewService(tempPath);

        try
        {
            // Act
            var settings = service.Load();

            // Assert
            settings.Should().NotBeNull();
            settings.Should().BeOfType<SystemSettings>();
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
        }
    }

    [Fact]
    public void Save_PersistsSettingsToFile()
    {
        // Arrange
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var service = NewService(tempPath);

        var testSettings = new SystemSettings
        {
            EnableCpu = true
        };

        try
        {
            // Act
            service.Save(testSettings);

            // Assert
            var settingsPath = Path.Combine(tempPath, "App_Data", "systemsettings.json");
            File.Exists(settingsPath).Should().BeTrue();
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
        }
    }

    [Fact]
    public void Load_ReturnsPersistedSettings_AfterSave()
    {
        // Arrange
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var service = NewService(tempPath);

        var testSettings = new SystemSettings
        {
            MetricsPollingIntervalSeconds = 10,
            EnableCpu = true
        };

        try
        {
            // Act
            service.Save(testSettings);
            var loadedSettings = service.Load();

            // Assert
            loadedSettings.Should().NotBeNull();
            loadedSettings.MetricsPollingIntervalSeconds.Should().Be(10);
            loadedSettings.EnableCpu.Should().BeTrue();
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
        }
    }

    [Fact]
    public void Save_OverwritesExistingSettings()
    {
        // Arrange
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var service = NewService(tempPath);

        var initialSettings = new SystemSettings { EnableCpu = false };
        var updatedSettings = new SystemSettings { EnableCpu = true };

        try
        {
            // Act
            service.Save(initialSettings);
            service.Save(updatedSettings);
            var loadedSettings = service.Load();

            // Assert
            loadedSettings.EnableCpu.Should().BeTrue();
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
        }
    }

    [Fact]
    public void Constructor_CreatesAppDataDirectory()
    {
        // Arrange
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var expectedPath = Path.Combine(tempPath, "App_Data");

        try
        {
            // Act
            var service = NewService(tempPath);

            // Assert
            Directory.Exists(expectedPath).Should().BeTrue();
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
        }
    }
}
