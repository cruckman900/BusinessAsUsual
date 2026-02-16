using BusinessAsUsual.Admin.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Areas.Admin.ViewComponents
{
    /// <summary>
    /// Represents a view component that renders the infrastructure health status using an optional health model.
    /// </summary>
    /// <remarks>Use this view component to display the current health information of the platform
    /// infrastructure. The component accepts an optional model of type PlatformHealthDto, which contains the health
    /// data to be presented. If no model is provided, the view will be rendered with a null model, and the display
    /// should account for the absence of health data.</remarks>
    public class InfrastructureHealthViewComponent : ViewComponent
    {
        /// <summary>
        /// Invokes the view component to render the associated view, optionally using the specified platform health
        /// data model.
        /// </summary>
        /// <param name="model">An optional model containing platform health data to be displayed in the view. If null, the view is rendered
        /// without model data.</param>
        /// <returns>An <see cref="IViewComponentResult"/> that renders the view for the platform health component.</returns>
        public IViewComponentResult Invoke(PlatformHealthDto model)
        {
            return View(model);
        }
    }
}
