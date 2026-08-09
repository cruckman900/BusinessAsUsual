using FluentAssertions;
using Platform.Web.Services;

namespace Platform.Web.Tests.Services;

public class SmartDefaultsServiceTests
{
    [Fact]
    public void RememberValue_ShouldStoreValue()
    {
        // Arrange
        var service = new SmartDefaultsService();
        const string key = "test.key";
        const string value = "test value";

        // Act
        service.RememberValue(key, value);

        // Assert
        service.HasValue(key).Should().BeTrue();
        service.GetValue<string>(key).Should().Be(value);
    }

    [Fact]
    public void GetValue_ShouldReturnStoredValue_WhenValueExists()
    {
        // Arrange
        var service = new SmartDefaultsService();
        const string key = "test.number";
        const int value = 42;
        service.RememberValue(key, value);

        // Act
        var result = service.GetValue<int>(key);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void GetValue_ShouldReturnFallback_WhenValueDoesNotExist()
    {
        // Arrange
        var service = new SmartDefaultsService();
        const string key = "nonexistent.key";
        const string fallback = "default value";

        // Act
        var result = service.GetValue(key, fallback);

        // Assert
        result.Should().Be(fallback);
    }

    [Fact]
    public void GetValue_ShouldReturnFallback_WhenValueIsWrongType()
    {
        // Arrange
        var service = new SmartDefaultsService();
        const string key = "test.key";
        service.RememberValue(key, "string value");

        // Act
        var result = service.GetValue<int>(key, 99);

        // Assert
        result.Should().Be(99);
    }

    [Fact]
    public void ClearValue_ShouldRemoveSpecificValue()
    {
        // Arrange
        var service = new SmartDefaultsService();
        service.RememberValue("key1", "value1");
        service.RememberValue("key2", "value2");

        // Act
        service.ClearValue("key1");

        // Assert
        service.HasValue("key1").Should().BeFalse();
        service.HasValue("key2").Should().BeTrue();
    }

    [Fact]
    public void ClearAll_ShouldRemoveAllValues()
    {
        // Arrange
        var service = new SmartDefaultsService();
        service.RememberValue("key1", "value1");
        service.RememberValue("key2", "value2");
        service.RememberValue("key3", "value3");

        // Act
        service.ClearAll();

        // Assert
        service.HasValue("key1").Should().BeFalse();
        service.HasValue("key2").Should().BeFalse();
        service.HasValue("key3").Should().BeFalse();
    }

    [Fact]
    public void HasValue_ShouldReturnTrue_WhenValueExists()
    {
        // Arrange
        var service = new SmartDefaultsService();
        const string key = "existing.key";
        service.RememberValue(key, "value");

        // Act
        var result = service.HasValue(key);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasValue_ShouldReturnFalse_WhenValueDoesNotExist()
    {
        // Arrange
        var service = new SmartDefaultsService();

        // Act
        var result = service.HasValue("nonexistent.key");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetUserDefaults_ShouldReturnDefaultValues_WhenNoValuesRemembered()
    {
        // Arrange
        var service = new SmartDefaultsService();

        // Act
        var defaults = service.GetUserDefaults();

        // Assert
        defaults.DefaultRole.Should().Be("User");
        defaults.DefaultActive.Should().BeTrue();
        defaults.DefaultDepartment.Should().BeNull();
        defaults.DefaultTimezone.Should().Be(TimeZoneInfo.Local.Id);
    }

    [Fact]
    public void GetUserDefaults_ShouldReturnRememberedValues_WhenValuesExist()
    {
        // Arrange
        var service = new SmartDefaultsService();
        service.RememberValue("user.lastRole", "Admin");
        service.RememberValue("user.defaultActive", false);
        service.RememberValue("user.lastDepartment", "Engineering");
        service.RememberValue("user.timezone", "America/New_York");

        // Act
        var defaults = service.GetUserDefaults();

        // Assert
        defaults.DefaultRole.Should().Be("Admin");
        defaults.DefaultActive.Should().BeFalse();
        defaults.DefaultDepartment.Should().Be("Engineering");
        defaults.DefaultTimezone.Should().Be("America/New_York");
    }

    [Fact]
    public void GetFormDefaults_ShouldReturnDefaultValues_WhenNoValuesRemembered()
    {
        // Arrange
        var service = new SmartDefaultsService();

        // Act
        var defaults = service.GetFormDefaults();

        // Assert
        defaults.DefaultDate.Should().Be(DateTime.Today);
        defaults.DefaultCurrency.Should().Be("USD");
        defaults.DefaultLanguage.Should().Be("en-US");
    }

    [Fact]
    public void GetFormDefaults_ShouldReturnRememberedValues_WhenValuesExist()
    {
        // Arrange
        var service = new SmartDefaultsService();
        var customDate = new DateTime(2025, 6, 15);
        service.RememberValue("form.lastDate", customDate);
        service.RememberValue("form.currency", "EUR");
        service.RememberValue("form.language", "fr-FR");

        // Act
        var defaults = service.GetFormDefaults();

        // Assert
        defaults.DefaultDate.Should().Be(customDate);
        defaults.DefaultCurrency.Should().Be("EUR");
        defaults.DefaultLanguage.Should().Be("fr-FR");
    }

    [Fact]
    public void RememberUserFormData_ShouldStoreAllUserValues()
    {
        // Arrange
        var service = new SmartDefaultsService();

        // Act
        service.RememberUserFormData("Manager", "Sales", false);

        // Assert
        service.GetValue<string>("user.lastRole").Should().Be("Manager");
        service.GetValue<string>("user.lastDepartment").Should().Be("Sales");
        service.GetValue<bool>("user.defaultActive").Should().BeFalse();
    }

    [Fact]
    public void RememberUserFormData_ShouldNotStoreDepartment_WhenNull()
    {
        // Arrange
        var service = new SmartDefaultsService();

        // Act
        service.RememberUserFormData("User", null, true);

        // Assert
        service.HasValue("user.lastDepartment").Should().BeFalse();
    }

    [Fact]
    public void RememberUserFormData_ShouldNotStoreDepartment_WhenEmpty()
    {
        // Arrange
        var service = new SmartDefaultsService();

        // Act
        service.RememberUserFormData("User", "", true);

        // Assert
        service.HasValue("user.lastDepartment").Should().BeFalse();
    }

    [Fact]
    public void RememberFormData_ShouldStoreAllFormValues()
    {
        // Arrange
        var service = new SmartDefaultsService();
        var date = new DateTime(2025, 12, 25);

        // Act
        service.RememberFormData(date, "GBP", "en-GB");

        // Assert
        service.GetValue<DateTime>("form.lastDate").Should().Be(date);
        service.GetValue<string>("form.currency").Should().Be("GBP");
        service.GetValue<string>("form.language").Should().Be("en-GB");
    }

    [Fact]
    public void RememberValue_ShouldOverwriteExistingValue()
    {
        // Arrange
        var service = new SmartDefaultsService();
        const string key = "test.key";
        service.RememberValue(key, "old value");

        // Act
        service.RememberValue(key, "new value");

        // Assert
        service.GetValue<string>(key).Should().Be("new value");
    }
}
