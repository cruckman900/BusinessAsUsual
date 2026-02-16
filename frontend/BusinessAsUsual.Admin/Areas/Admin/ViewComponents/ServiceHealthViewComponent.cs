using BusinessAsUsual.Admin.Areas.Admin.Models.Monitoring;
using BusinessAsUsual.Admin.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Areas.Admin.ViewComponents
{
    /// <summary>
    /// Represents a view component that renders health metrics and alarm statuses for multiple platform services.
    /// </summary>
    /// <remarks>This view component processes a model containing service health metrics and associated
    /// alarms, and generates a collection of metric cards for display. Each card visually indicates the health status
    /// of a service using color coding based on alarm severity. Use this component to provide an at-a-glance overview
    /// of service health within the platform's administrative interface.</remarks>
    public class ServiceHealthViewComponent : ViewComponent
    {
        /// <summary>
        /// Generates a view component result that displays metric cards representing the health status of each
        /// monitored service.
        /// </summary>
        /// <remarks>Each metric card visually indicates the health of a service based on its alarm
        /// states. The card color reflects the most severe alarm present: red for active alarms, yellow for
        /// insufficient data, and green for healthy states.</remarks>
        /// <param name="model">A model containing health metrics and alarm states for multiple services. Cannot be null.</param>
        /// <returns>A view component result that renders a collection of metric cards, each summarizing the metrics and alarm
        /// status for a service.</returns>
        public IViewComponentResult Invoke(PlatformHealthDto model)
        {
            var cards = new List<MetricCardModel>();

            foreach (var serviceName in model.Metrics.Keys)
            {
                var metrics = model.Metrics[serviceName];
                var alarms = model.Alarms[serviceName];

                // Shape service alarms into a dictionary
                var serviceAlarms = new Dictionary<string, string>
                {
                    { "High Error Rate", alarms.HighErrorRate ?? string.Empty },
                    { "No Traffic", alarms.NoTraffic ?? string.Empty }
                };

                // Determine card color
                string color =
                    serviceAlarms.Values.Contains("ALARM") ? "red" :
                    serviceAlarms.Values.Contains("INSUFFICIENT_DATA") ? "yellow" :
                    "green";

                // Build the card model
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