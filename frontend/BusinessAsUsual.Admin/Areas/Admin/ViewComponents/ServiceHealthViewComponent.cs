using BusinessAsUsual.Admin.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Areas.Admin.ViewComponents
{
    /// <summary>
    /// Represents a view component that displays the health status of the platform services.
    /// </summary>
    /// <remarks>This view component accepts an optional model of type PlatformHealthDto, which contains the
    /// health data to be rendered. If no model is provided, the component will render an empty view. Use this component
    /// to provide users with a visual summary of platform service health within the application's UI.</remarks>
    public class ServiceHealthViewComponent : ViewComponent
    {
        /// <summary>
        /// Invokes the view component to render the associated view using the specified platform health data model.
        /// </summary>
        /// <param name="model">An optional model containing platform health data to be displayed in the view. If null, the view is rendered
        /// without model data.</param>
        /// <returns>An <see cref="IViewComponentResult"/> that represents the rendered view component result.</returns>
        public IViewComponentResult Invoke(PlatformHealthDto model)
        {
            return View(model);
        }
    }
}
