using FluentAssertions;
using Moq;
using MudBlazor;
using Platform.Web.Services;

namespace Platform.Web.Tests.Services;

public class ToastServiceTests
{
    private readonly Mock<ISnackbar> _snackbarMock;
    private readonly ToastService _service;

    public ToastServiceTests()
    {
        _snackbarMock = new Mock<ISnackbar>();
        _service = new ToastService(_snackbarMock.Object);
    }

    [Fact]
    public void Success_WithoutAction_ShouldAddSuccessMessage()
    {
        // Arrange
        const string message = "Operation successful";

        // Act
        _service.Success(message);

        // Assert
        _snackbarMock.Verify(
            x => x.Add(message, Severity.Success, null, null),
            Times.Once);
    }

    [Fact]
    public void Success_WithAction_ShouldAddSuccessMessageWithUndoButton()
    {
        // Arrange
        const string message = "Item created";
        var actionCalled = false;
        Action action = () => actionCalled = true;

        // Act
        _service.Success(message, action);

        // Assert
        _snackbarMock.Verify(
            x => x.Add(message, Severity.Success, It.IsAny<Action<SnackbarOptions>>(), null),
            Times.Once);
    }

    [Fact]
    public void Success_WithCustomActionLabel_ShouldUseCustomLabel()
    {
        // Arrange
        const string message = "Item created";
        const string actionLabel = "Custom Action";
        Action action = () => { };

        // Act
        _service.Success(message, action, actionLabel);

        // Assert
        _snackbarMock.Verify(
            x => x.Add(message, Severity.Success, It.IsAny<Action<SnackbarOptions>>(), null),
            Times.Once);
    }

    [Fact]
    public void Info_WithoutAction_ShouldAddInfoMessage()
    {
        // Arrange
        const string message = "Information message";

        // Act
        _service.Info(message);

        // Assert
        _snackbarMock.Verify(
            x => x.Add(message, Severity.Info, null, null),
            Times.Once);
    }

    [Fact]
    public void Info_WithAction_ShouldAddInfoMessageWithViewButton()
    {
        // Arrange
        const string message = "New notification";
        Action action = () => { };

        // Act
        _service.Info(message, action);

        // Assert
        _snackbarMock.Verify(
            x => x.Add(message, Severity.Info, It.IsAny<Action<SnackbarOptions>>(), null),
            Times.Once);
    }

    [Fact]
    public void Warning_WithoutAction_ShouldAddWarningMessage()
    {
        // Arrange
        const string message = "Warning message";

        // Act
        _service.Warning(message);

        // Assert
        _snackbarMock.Verify(
            x => x.Add(message, Severity.Warning, null, null),
            Times.Once);
    }

    [Fact]
    public void Warning_WithAction_ShouldAddWarningMessageWithDetailsButton()
    {
        // Arrange
        const string message = "Potential issue detected";
        Action action = () => { };

        // Act
        _service.Warning(message, action);

        // Assert
        _snackbarMock.Verify(
            x => x.Add(message, Severity.Warning, It.IsAny<Action<SnackbarOptions>>(), null),
            Times.Once);
    }

    [Fact]
    public void Error_WithoutAction_ShouldAddErrorMessage()
    {
        // Arrange
        const string message = "Error occurred";

        // Act
        _service.Error(message);

        // Assert
        _snackbarMock.Verify(
            x => x.Add(message, Severity.Error, null, null),
            Times.Once);
    }

    [Fact]
    public void Error_WithAction_ShouldAddErrorMessageWithRetryButton()
    {
        // Arrange
        const string message = "Failed to save";
        Action action = () => { };

        // Act
        _service.Error(message, action);

        // Assert
        _snackbarMock.Verify(
            x => x.Add(message, Severity.Error, It.IsAny<Action<SnackbarOptions>>(), null),
            Times.Once);
    }

    [Fact]
    public void Deleted_ShouldAddInfoMessageWithUndo()
    {
        // Arrange
        const string itemName = "User";
        var undoCalled = false;
        Action undoAction = () => undoCalled = true;

        // Act
        _service.Deleted(itemName, undoAction);

        // Assert
        _snackbarMock.Verify(
            x => x.Add($"{itemName} deleted", Severity.Info, It.IsAny<Action<SnackbarOptions>>(), null),
            Times.Once);
    }

    [Fact]
    public void Saved_ShouldAddSuccessMessageWithCheckmark()
    {
        // Arrange
        const string itemName = "Document";

        // Act
        _service.Saved(itemName);

        // Assert
        _snackbarMock.Verify(
            x => x.Add($"✓ {itemName} saved successfully", Severity.Success, It.IsAny<Action<SnackbarOptions>>(), null),
            Times.Once);
    }

    [Fact]
    public void Created_WithoutViewAction_ShouldAddSuccessMessage()
    {
        // Arrange
        const string itemName = "New User";

        // Act
        _service.Created(itemName);

        // Assert
        _snackbarMock.Verify(
            x => x.Add($"✓ {itemName} created successfully", Severity.Success, null, null),
            Times.Once);
    }

    [Fact]
    public void Created_WithViewAction_ShouldAddSuccessMessageWithViewButton()
    {
        // Arrange
        const string itemName = "New Document";
        var viewCalled = false;
        Action viewAction = () => viewCalled = true;

        // Act
        _service.Created(itemName, viewAction);

        // Assert
        _snackbarMock.Verify(
            x => x.Add($"✓ {itemName} created successfully", Severity.Success, It.IsAny<Action<SnackbarOptions>>(), null),
            Times.Once);
    }

    [Fact]
    public void Success_MultipleCallsWithActions_ShouldInvokeActionsIndependently()
    {
        // Arrange
        var action1Called = false;
        var action2Called = false;
        Action action1 = () => action1Called = true;
        Action action2 = () => action2Called = true;

        // Act
        _service.Success("Message 1", action1);
        _service.Success("Message 2", action2);

        // Assert
        _snackbarMock.Verify(
            x => x.Add(It.IsAny<string>(), Severity.Success, It.IsAny<Action<SnackbarOptions>>(), null),
            Times.Exactly(2));
    }

    [Fact]
    public void Info_WithNullAction_ShouldAddSimpleInfoMessage()
    {
        // Arrange
        const string message = "Simple info";

        // Act
        _service.Info(message, null);

        // Assert
        _snackbarMock.Verify(
            x => x.Add(message, Severity.Info, null, null),
            Times.Once);
    }

    [Fact]
    public void Warning_WithNullAction_ShouldAddSimpleWarningMessage()
    {
        // Arrange
        const string message = "Simple warning";

        // Act
        _service.Warning(message, null);

        // Assert
        _snackbarMock.Verify(
            x => x.Add(message, Severity.Warning, null, null),
            Times.Once);
    }

    [Fact]
    public void Error_WithNullAction_ShouldAddSimpleErrorMessage()
    {
        // Arrange
        const string message = "Simple error";

        // Act
        _service.Error(message, null);

        // Assert
        _snackbarMock.Verify(
            x => x.Add(message, Severity.Error, null, null),
            Times.Once);
    }

    [Fact]
    public void Created_WithNullAction_ShouldAddSimpleSuccessMessage()
    {
        // Arrange
        const string itemName = "Item";

        // Act
        _service.Created(itemName, null);

        // Assert
        _snackbarMock.Verify(
            x => x.Add($"✓ {itemName} created successfully", Severity.Success, null, null),
            Times.Once);
    }
}
