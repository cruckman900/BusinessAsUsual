using Microsoft.AspNetCore.Components;

namespace BusinessAsUsual.Web.Modules._Shared
{
    /// <summary>
    /// Serves as a base class for Blazor components that require no page header to be displayed.
    /// </summary>
    /// <remarks>Inheriting from this class ensures that the page header is cleared when the component is
    /// first rendered. Use this base class for pages where a header should not be shown, such as login or error
    /// pages.</remarks>
    public abstract class NoHeaderPageBase : ComponentBase
    {
        /// <summary>
        /// Gets or sets the service used to manage the page header state and actions.
        /// </summary>
        [Inject] public PageHeaderService HeaderService { get; set; } = default!;

        /// <summary>
        /// Invoked after the component has rendered. Allows for post-render logic to be executed.
        /// </summary>
        /// <remarks>Override this method to perform actions after the component has rendered. Use the
        /// <paramref name="firstRender"/> parameter to ensure that initialization logic runs only once.</remarks>
        /// <param name="firstRender">Indicates whether this is the first time the component has rendered. Set to <see langword="true"/> if this
        /// is the initial render; otherwise, <see langword="false"/>.</param>
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                HeaderService.SetHeader(null);
            }
        }
    }
}
