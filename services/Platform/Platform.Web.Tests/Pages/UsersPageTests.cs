using FluentAssertions;
using Xunit;

namespace Platform.Web.Tests.Pages;

public class UsersPageTests
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Department { get; set; }
        public string Role { get; set; } = "";
        public bool IsActive { get; set; }
        public DateTime? LastLogin { get; set; }
    }

    [Fact]
    public void QuickFilter_ShouldReturnTrue_WhenSearchStringIsEmpty()
    {
        // Arrange
        var user = new UserModel 
        { 
            FullName = "John Doe", 
            Email = "john@test.com", 
            Department = "IT" 
        };
        var searchString = "";

        // Act
        var result = ApplyQuickFilter(user, searchString);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void QuickFilter_ShouldReturnTrue_WhenSearchStringIsWhitespace()
    {
        // Arrange
        var user = new UserModel 
        { 
            FullName = "John Doe", 
            Email = "john@test.com" 
        };
        var searchString = "   ";

        // Act
        var result = ApplyQuickFilter(user, searchString);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void QuickFilter_ShouldMatchFullName_CaseInsensitive()
    {
        // Arrange
        var user = new UserModel 
        { 
            FullName = "John Doe", 
            Email = "john@test.com" 
        };
        var searchString = "john";

        // Act
        var result = ApplyQuickFilter(user, searchString);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void QuickFilter_ShouldMatchFullName_UpperCase()
    {
        // Arrange
        var user = new UserModel 
        { 
            FullName = "John Doe", 
            Email = "john@test.com" 
        };
        var searchString = "JOHN";

        // Act
        var result = ApplyQuickFilter(user, searchString);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void QuickFilter_ShouldMatchEmail()
    {
        // Arrange
        var user = new UserModel 
        { 
            FullName = "John Doe", 
            Email = "john.doe@businessasusual.com" 
        };
        var searchString = "doe@business";

        // Act
        var result = ApplyQuickFilter(user, searchString);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void QuickFilter_ShouldMatchDepartment()
    {
        // Arrange
        var user = new UserModel 
        { 
            FullName = "John Doe", 
            Email = "john@test.com",
            Department = "Engineering"
        };
        var searchString = "engineer";

        // Act
        var result = ApplyQuickFilter(user, searchString);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void QuickFilter_ShouldReturnTrue_WhenDepartmentIsNull()
    {
        // Arrange
        var user = new UserModel 
        { 
            FullName = "John Doe", 
            Email = "john@test.com",
            Department = null
        };
        var searchString = "";

        // Act
        var result = ApplyQuickFilter(user, searchString);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void QuickFilter_ShouldReturnFalse_WhenNoFieldsMatch()
    {
        // Arrange
        var user = new UserModel 
        { 
            FullName = "John Doe", 
            Email = "john@test.com",
            Department = "IT"
        };
        var searchString = "sales";

        // Act
        var result = ApplyQuickFilter(user, searchString);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void QuickFilter_ShouldReturnFalse_WhenDepartmentIsNullAndSearching()
    {
        // Arrange
        var user = new UserModel 
        { 
            FullName = "John Doe", 
            Email = "john@test.com",
            Department = null
        };
        var searchString = "engineering";

        // Act
        var result = ApplyQuickFilter(user, searchString);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void QuickFilter_ShouldMatchPartialFullName()
    {
        // Arrange
        var user = new UserModel 
        { 
            FullName = "Alice Williams", 
            Email = "alice@test.com"
        };
        var searchString = "will";

        // Act
        var result = ApplyQuickFilter(user, searchString);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void QuickFilter_ShouldMatchPartialEmail()
    {
        // Arrange
        var user = new UserModel 
        { 
            FullName = "Bob Johnson", 
            Email = "bob.johnson@example.com"
        };
        var searchString = "example";

        // Act
        var result = ApplyQuickFilter(user, searchString);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void QuickFilter_ShouldHandleSpecialCharactersInSearch()
    {
        // Arrange
        var user = new UserModel 
        { 
            FullName = "John Doe", 
            Email = "john.doe@test.com"
        };
        var searchString = "john.doe";

        // Act
        var result = ApplyQuickFilter(user, searchString);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void QuickFilter_MultipleUsers_ShouldFilterCorrectly()
    {
        // Arrange
        var users = new List<UserModel>
        {
            new() { FullName = "John Doe", Email = "john@test.com", Department = "IT" },
            new() { FullName = "Jane Smith", Email = "jane@test.com", Department = "Finance" },
            new() { FullName = "Bob Johnson", Email = "bob@test.com", Department = "Sales" }
        };
        var searchString = "jane";

        // Act
        var filtered = users.Where(u => ApplyQuickFilter(u, searchString)).ToList();

        // Assert
        filtered.Should().HaveCount(1);
        filtered[0].FullName.Should().Be("Jane Smith");
    }

    [Fact]
    public void QuickFilter_MultipleUsers_ShouldReturnAll_WhenSearchIsEmpty()
    {
        // Arrange
        var users = new List<UserModel>
        {
            new() { FullName = "John Doe", Email = "john@test.com" },
            new() { FullName = "Jane Smith", Email = "jane@test.com" },
            new() { FullName = "Bob Johnson", Email = "bob@test.com" }
        };
        var searchString = "";

        // Act
        var filtered = users.Where(u => ApplyQuickFilter(u, searchString)).ToList();

        // Assert
        filtered.Should().HaveCount(3);
    }

    [Fact]
    public void QuickFilter_MultipleUsers_ShouldReturnNone_WhenNoMatch()
    {
        // Arrange
        var users = new List<UserModel>
        {
            new() { FullName = "John Doe", Email = "john@test.com", Department = "IT" },
            new() { FullName = "Jane Smith", Email = "jane@test.com", Department = "Finance" }
        };
        var searchString = "sales";

        // Act
        var filtered = users.Where(u => ApplyQuickFilter(u, searchString)).ToList();

        // Assert
        filtered.Should().BeEmpty();
    }

    // Helper method that replicates the Users page quick filter logic
    private bool ApplyQuickFilter(UserModel user, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;

        if (user.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (user.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (user.Department?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true)
            return true;

        return false;
    }
}
