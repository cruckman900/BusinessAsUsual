using BusinessAsUsual.Admin.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Areas.Admin.ViewComponents
{
    /// <summary>
    /// Represents a view component that displays the current health status of the platform.
    /// </summary>
    /// <remarks>This view component accepts an optional model of type PlatformHealthDto, which contains the
    /// health status data to be rendered. If no model is provided, the view will be rendered without health status
    /// information. Use this component to provide users with a visual summary of platform health in the UI.</remarks>
    public class PlatformStatusViewComponent : ViewComponent
    {
        /// <summary>
        /// Invokes the view component to render the platform status view using the specified model.
        /// </summary>
        /// <param name="model">An optional model containing platform health data to be displayed in the view. If null, the view is rendered
        /// without model data.</param>
        /// <returns>An <see cref="IViewComponentResult"/> that renders the platform status view with the provided model.</returns>
        public IViewComponentResult Invoke(PlatformHealthDto? model = null)
        {
            return View(model);
        }
    }
}
