using BusinessAsUsual.Web.Services;
using FluentAssertions;

namespace BusinessAsUsual.Web.Tests.Unit;

public class AuthenticationServiceTests
{
    private AuthenticationService NewService()
    {
        var service = new AuthenticationService();
        // Clear the default test user created in constructor
        service.Logout();
        return service;
    }

    [Fact]
    public void IsAuthenticated_InitiallyFalse_AfterLogout()
    {
        // Arrange & Act
        var service = NewService();

        // Assert
        service.IsAuthenticated.Should().BeFalse();
        service.CurrentUser.Should().BeNull();
    }

    [Fact]
    public void Login_SetsAuthenticatedState()
    {
        // Arrange
        var service = NewService();
        var username = "testuser";
        var email = "test@example.com";
        var fullName = "Test User";

        // Act
        service.Login(username, email, fullName);

        // Assert
        service.IsAuthenticated.Should().BeTrue();
        service.CurrentUser.Should().NotBeNull();
        service.CurrentUser!.Username.Should().Be(username);
        service.CurrentUser.Email.Should().Be(email);
        service.CurrentUser.FullName.Should().Be(fullName);
    }

    [Fact]
    public void Login_WithRole_SetsRole()
    {
        // Arrange
        var service = NewService();

        // Act
        service.Login("admin", "admin@example.com", "Admin User", "Administrator");

        // Assert
        service.CurrentUser.Should().NotBeNull();
        service.CurrentUser!.Role.Should().Be("Administrator");
    }

    [Fact]
    public void Logout_ClearsAuthenticationState()
    {
        // Arrange
        var service = NewService();
        service.Login("user", "user@example.com", "Test User");
        service.IsAuthenticated.Should().BeTrue();

        // Act
        service.Logout();

        // Assert
        service.IsAuthenticated.Should().BeFalse();
        service.CurrentUser.Should().BeNull();
    }

    [Fact]
    public void OnAuthStateChanged_RaisesEvent_OnLogin()
    {
        // Arrange
        var service = NewService();
        var eventRaised = false;
        service.OnAuthStateChanged += () => eventRaised = true;

        // Act
        service.Login("user", "user@example.com", "Test User");

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    public void OnAuthStateChanged_RaisesEvent_OnLogout()
    {
        // Arrange
        var service = NewService();
        service.Login("user", "user@example.com", "Test User");
        var eventRaised = false;
        service.OnAuthStateChanged += () => eventRaised = true;

        // Act
        service.Logout();

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    public void UpdateProfile_UpdatesUserInformation()
    {
        // Arrange
        var service = NewService();
        service.Login("user", "old@example.com", "Old Name");

        // Act
        service.UpdateProfile("New Name", "new@example.com", "555-1234", "Developer", "IT");

        // Assert
        service.CurrentUser.Should().NotBeNull();
        service.CurrentUser!.FullName.Should().Be("New Name");
        service.CurrentUser.Email.Should().Be("new@example.com");
        service.CurrentUser.PhoneNumber.Should().Be("555-1234");
        service.CurrentUser.JobTitle.Should().Be("Developer");
        service.CurrentUser.Department.Should().Be("IT");
    }

    [Fact]
    public void UpdateProfile_RaisesEvent()
    {
        // Arrange
        var service = NewService();
        service.Login("user", "user@example.com", "Test User");
        var eventRaised = false;
        service.OnAuthStateChanged += () => eventRaised = true;

        // Act
        service.UpdateProfile("Updated Name", "user@example.com", null, null, null);

        // Assert
        eventRaised.Should().BeTrue();
    }
}
