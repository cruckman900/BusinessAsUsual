using BusinessAsUsual.Admin.Dtos;
using BusinessAsUsual.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Web.Areas.Admin.ViewComponents
{
    /// <summary>
    /// Represents a view component that displays the current health status of the platform.
    /// </summary>
    /// <remarks>This view component retrieves platform health information using the provided monitoring
    /// service. If no health data model is supplied, it automatically fetches the latest platform health status. Ensure
    /// that the monitoring service is properly configured to provide accurate health information.</remarks>
    public class PlatformStatusViewComponent : ViewComponent
    {
        private readonly IMonitoringService _monitoring;

        /// <summary>
        /// Initializes a new instance of the PlatformStatusViewComponent class using the specified monitoring service.
        /// </summary>
        /// <remarks>The provided IMonitoringService instance must be properly configured before being
        /// passed to this constructor. Passing a null value will result in a runtime exception.</remarks>
        /// <param name="monitoring">The monitoring service that provides platform status information. This parameter must not be null.</param>
        public PlatformStatusViewComponent(IMonitoringService monitoring)
        {
            _monitoring = monitoring;
        }

        /// <summary>
        /// Invokes the view component to display platform health information using the specified model or retrieves the
        /// current platform health data if the model is null.
        /// </summary>
        /// <remarks>If the supplied model is null, this method synchronously fetches the latest platform
        /// health data to ensure the view is always rendered with up-to-date information.</remarks>
        /// <param name="model">The platform health data to be displayed in the view. If null, the method retrieves the latest platform
        /// health information from the monitoring service.</param>
        /// <returns>A view component result that renders the platform health view with the provided or retrieved model.</returns>
        public IViewComponentResult Invoke(PlatformHealthDto model)
        {
            // ---------------------------------------------
            // If model is null (initial page load), fetch it
            // ---------------------------------------------
            if (model == null)
            {
                model = _monitoring.GetPlatformHealthAsync().GetAwaiter().GetResult();
            }

            return View(model);
        }
    }
}