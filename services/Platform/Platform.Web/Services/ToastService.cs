using MudBlazor;

namespace Platform.Web.Services;

public class ToastService
{
    private readonly ISnackbar _snackbar;

    public ToastService(ISnackbar snackbar)
    {
        _snackbar = snackbar;
    }

    public void Success(string message, Action? action = null, string actionLabel = "Undo")
    {
        if (action != null)
        {
            _snackbar.Add(message, Severity.Success, config =>
            {
                config.Action = actionLabel;
                config.ActionColor = Color.Inherit;
                config.OnClick = _ =>
                {
                    action();
                    return Task.CompletedTask;
                };
            });
        }
        else
        {
            _snackbar.Add(message, Severity.Success);
        }
    }

    public void Info(string message, Action? action = null, string actionLabel = "View")
    {
        if (action != null)
        {
            _snackbar.Add(message, Severity.Info, config =>
            {
                config.Action = actionLabel;
                config.ActionColor = Color.Inherit;
                config.OnClick = _ =>
                {
                    action();
                    return Task.CompletedTask;
                };
            });
        }
        else
        {
            _snackbar.Add(message, Severity.Info);
        }
    }

    public void Warning(string message, Action? action = null, string actionLabel = "Details")
    {
        if (action != null)
        {
            _snackbar.Add(message, Severity.Warning, config =>
            {
                config.Action = actionLabel;
                config.ActionColor = Color.Inherit;
                config.OnClick = _ =>
                {
                    action();
                    return Task.CompletedTask;
                };
            });
        }
        else
        {
            _snackbar.Add(message, Severity.Warning);
        }
    }

    public void Error(string message, Action? action = null, string actionLabel = "Retry")
    {
        if (action != null)
        {
            _snackbar.Add(message, Severity.Error, config =>
            {
                config.Action = actionLabel;
                config.ActionColor = Color.Inherit;
                config.OnClick = _ =>
                {
                    action();
                    return Task.CompletedTask;
                };
                config.VisibleStateDuration = 5000; // Errors stay longer
            });
        }
        else
        {
            _snackbar.Add(message, Severity.Error);
        }
    }

    public void Deleted(string itemName, Action undoAction)
    {
        _snackbar.Add($"{itemName} deleted", Severity.Info, config =>
        {
            config.Action = "Undo";
            config.ActionColor = Color.Warning;
            config.OnClick = _ =>
            {
                undoAction();
                return Task.CompletedTask;
            };
            config.VisibleStateDuration = 5000; // Give time to undo
        });
    }

    public void Saved(string itemName)
    {
        _snackbar.Add($"✓ {itemName} saved successfully", Severity.Success, config =>
        {
            config.VisibleStateDuration = 2000; // Quick success message
        });
    }

    public void Created(string itemName, Action? viewAction = null)
    {
        if (viewAction != null)
        {
            _snackbar.Add($"✓ {itemName} created successfully", Severity.Success, config =>
            {
                config.Action = "View";
                config.ActionColor = Color.Inherit;
                config.OnClick = _ =>
                {
                    viewAction();
                    return Task.CompletedTask;
                };
            });
        }
        else
        {
            _snackbar.Add($"✓ {itemName} created successfully", Severity.Success);
        }
    }
}
