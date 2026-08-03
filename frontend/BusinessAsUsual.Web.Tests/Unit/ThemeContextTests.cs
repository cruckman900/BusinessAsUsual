using FluentAssertions;

namespace BusinessAsUsual.Web.Tests.Unit;

public class ThemeContextTests
{
    private ThemeContext NewThemeContext() => new();

    [Fact]
    public void Constructor_InitializesWithDefaultTheme()
    {
        // Arrange & Act
        var context = NewThemeContext();

        // Assert
        context.CurrentTheme.Should().NotBeNull();
        context.ThemeName.Should().Be("light");
        context.IsDarkMode.Should().BeFalse();
    }

    [Fact]
    public void SetTheme_UpdatesThemeName()
    {
        // Arrange
        var context = NewThemeContext();

        // Act
        context.SetTheme("dark", true);

        // Assert
        context.ThemeName.Should().Be("dark");
        context.IsDarkMode.Should().BeTrue();
    }

    [Fact]
    public void SetTheme_RaisesOnThemeChanged_Event()
    {
        // Arrange
        var context = NewThemeContext();
        var eventRaised = false;
        context.OnThemeChanged += () => eventRaised = true;

        // Act
        context.SetTheme("corporate", false);

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    public void CurrentTheme_ReturnsMudTheme()
    {
        // Arrange
        var context = NewThemeContext();

        // Act
        var theme = context.CurrentTheme;

        // Assert
        theme.Should().NotBeNull();
        theme.Should().BeOfType<MudBlazor.MudTheme>();
    }

    [Fact]
    public void SetTheme_UpdatesDarkModeState()
    {
        // Arrange
        var context = NewThemeContext();
        context.SetTheme("light", false);

        // Act
        context.SetTheme("light", true);

        // Assert
        context.IsDarkMode.Should().BeTrue();
        context.ThemeName.Should().Be("light");
    }
}
