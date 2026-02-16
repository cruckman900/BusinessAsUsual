using BusinessAsUsual.Admin.Areas.Admin.Models.Monitoring;
using BusinessAsUsual.Admin.Dtos;
using BusinessAsUsual.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Web.Areas.Admin.ViewComponents
{
    /// <summary>
    /// Represents a view component that displays the health status of platform services using metrics and alarms.
    /// </summary>
    /// <remarks>This view component renders a collection of metric cards, each summarizing the health of an
    /// individual service based on platform monitoring data. If no model is provided, the component retrieves the
    /// latest platform health information from the monitoring service. The component is typically used in
    /// administrative dashboards to provide a visual overview of service health, with color-coded indicators reflecting
    /// alarm states.</remarks>
    public class ServiceHealthViewComponent : ViewComponent
    {
        private readonly IMonitoringService _monitoring;

        /// <summary>
        /// Initializes a new instance of the ServiceHealthViewComponent class using the specified monitoring service.
        /// </summary>
        /// <param name="monitoring">The monitoring service used to track and report the health status of the system. This parameter cannot be
        /// null.</param>
        public ServiceHealthViewComponent(IMonitoringService monitoring)
        {
            _monitoring = monitoring;
        }

        /// <summary>
        /// Invokes the view component to render platform health metrics and alarm statuses for various services.
        /// </summary>
        /// <remarks>If the provided model is null, the method synchronously fetches the latest platform
        /// health data before rendering. The resulting view categorizes service alarms and visually indicates their
        /// status using color coding.</remarks>
        /// <param name="model">A model containing the current platform health data. If null, the method retrieves the latest platform
        /// health information.</param>
        /// <returns>A view component result that displays a collection of metric cards, each representing the health status and
        /// alarms for a service.</returns>
        public IViewComponentResult Invoke(PlatformHealthDto model)
        {
            // ---------------------------------------------
            // If model is null (initial page load), fetch it
            // ---------------------------------------------
            if (model == null)
            {
                model = _monitoring.GetPlatformHealthAsync().GetAwaiter().GetResult();
            }

            var cards = new List<MetricCardModel>();

            foreach (var serviceName in model.Metrics.Keys)
            {
                var metrics = model.Metrics[serviceName];
                var alarms = model.Alarms[serviceName];

                var serviceAlarms = new Dictionary<string, string>
                {
                    { "High Error Rate", alarms.HighErrorRate ?? string.Empty },
                    { "No Traffic", alarms.NoTraffic ?? string.Empty }
                };

                string color =
                    serviceAlarms.Values.Contains("ALARM") ? "red" :
                    serviceAlarms.Values.Contains("INSUFFICIENT_DATA") ? "yellow" :
                    "green";

                cards.Add(new MetricCardModel
                {
                    Title = serviceName,
                    Color = color,
                    Metrics = metrics,
                    ServiceAlarms = serviceAlarms
                });
            }

            return View(cards);
        }
    }
}