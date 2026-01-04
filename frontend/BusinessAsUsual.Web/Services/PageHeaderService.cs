using Microsoft.AspNetCore.Components;

/// <summary>
/// Provides a service for managing and updating the page header content in a Blazor application.
/// </summary>
/// <remarks>This service allows components to set or update the page header and notifies subscribers when the
/// header changes. It is typically used to coordinate dynamic page header content across different
/// components.</remarks>
public class PageHeaderService
{
    /// <summary>
    /// Gets or sets the content to render in the header section.
    /// </summary>
    /// <remarks>The header content can include markup, components, or plain text. If not set, no header will
    /// be rendered.</remarks>
    public RenderFragment? Header { get; set; }

    /// <summary>
    /// Occurs when the state of the object changes.
    /// </summary>
    /// <remarks>Subscribe to this event to be notified when a change occurs. Handlers attached to this event
    /// are invoked with no arguments when the change is detected.</remarks>
    public event Action? OnChange;

    /// <summary>
    /// Exposed function to set header content.
    /// </summary>
    /// <param name="header"></param>
    public void SetHeader(RenderFragment? header)
    {
        Header = header;
        OnChange?.Invoke();
    }
}