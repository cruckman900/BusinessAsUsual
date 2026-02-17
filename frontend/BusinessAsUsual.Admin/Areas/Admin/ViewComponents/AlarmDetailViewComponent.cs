using BusinessAsUsual.Admin.Areas.Admin.Models.Monitoring;
using BusinessAsUsual.Admin.Dtos;
using BusinessAsUsual.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Admin.Areas.Admin.ViewComponents
{
    /// <summary>
    /// Represents a view component that generates a detailed view of alarms and metrics for a specified service title.
    /// </summary>
    /// <remarks>This component retrieves platform health information and organizes alarm details, metrics,
    /// and descriptions for display. It supports multiple service titles, such as 'RDS' and 'EC2', each with specific
    /// alarm criteria and descriptions. Use this component to present up-to-date monitoring information in the admin
    /// area.</remarks>
    public class AlarmDetailViewComponent : ViewComponent
    {
        private readonly IMonitoringService _monitoring;

        /// <summary>
        /// Initializes a new instance of the AlarmDetailViewComponent class using the specified monitoring service.
        /// </summary>
        /// <param name="monitoring">The monitoring service used to retrieve alarm details and provide status updates.</param>
        public AlarmDetailViewComponent(IMonitoringService monitoring)
        {
            _monitoring = monitoring;
        }

        /// <summary>
        /// Generates a view component result that displays platform health details, including alarms and recent
        /// metrics, for the specified service or platform.
        /// </summary>
        /// <remarks>The method retrieves the latest health metrics and alarms for the given platform or
        /// service. Different titles, such as 'RDS' and 'EC2', provide specific alarm types and metric descriptions.
        /// Ensure that the title matches a supported service to obtain accurate health details.</remarks>
        /// <param name="title">The name of the platform or service for which health metrics and alarm details are retrieved. Must
        /// correspond to a valid service such as 'RDS' or 'EC2'.</param>
        /// <returns>A view component result containing structured health information, including alarms, metrics, and
        /// descriptions for the specified platform or service.</returns>
        public IViewComponentResult Invoke(string title)
        {
            // Fetch the latest platform health
            var model = _monitoring.GetPlatformHealthAsync().GetAwaiter().GetResult();

            var detail = new AlarmDetailModel
            {
                Title = title,
                Alarms = new Dictionary<string, string>(),
                RecentMetrics = new List<ServiceMetricsDto>(),
                CloudWatchLinks = new Dictionary<string, string>(),
                Descriptions = new Dictionary<string, string>()
            };

            // -----------------------------------------
            // SERVICE DETAIL
            // -----------------------------------------
            if (model.Metrics.ContainsKey(title))
            {
                var metrics = model.Metrics[title];
                var alarms = model.Alarms[title];

                detail.Alarms.Add("High Error Rate", alarms.HighErrorRate ?? string.Empty);
                detail.Alarms.Add("No Traffic", alarms.NoTraffic ?? string.Empty);

                detail.RecentMetrics.Add(metrics);

                detail.Descriptions.Add("High Error Rate", "Triggered when error rate >= 1.");
                detail.Descriptions.Add("No Traffic", "Triggered when no requests are received.");
            }

            // -----------------------------------------
            // INFRASTRUCTURE DETAIL
            // -----------------------------------------
            if (title == "RDS")
            {
                detail.Alarms.Add("Free Storage Low", model.Infrastructure.RdsFreeStorageLow ?? string.Empty);
                detail.Alarms.Add("CPU High", model.Infrastructure.RdsCpuHigh ?? string.Empty);
                detail.Alarms.Add("Freeable Memory Low", model.Infrastructure.RdsFreeableMemoryLow ?? string.Empty);
                detail.Alarms.Add("Connections High", model.Infrastructure.RdsConnectionsHigh ?? string.Empty);

                detail.Descriptions.Add("Free Storage Low", "Triggered when free storage < 1GB.");
                detail.Descriptions.Add("CPU High", "Triggered when CPU usage exceeds threshold.");
                detail.Descriptions.Add("Freeable Memory Low", "Triggered when freeable memory drops too low.");
                detail.Descriptions.Add("Connections High", "Triggered when DB connections exceed threshold.");
            }

            if (title == "EC2")
            {
                detail.Alarms.Add("CPU High", model.Infrastructure.Ec2CpuHigh ?? string.Empty);
                detail.Alarms.Add("Status Check Failed", model.Infrastructure.Ec2StatusCheckFailed ?? string.Empty);

                detail.Descriptions.Add("CPU High", "Triggered when EC2 CPU usage exceeds threshold.");
                detail.Descriptions.Add("Status Check Failed", "Triggered when EC2 fails system or instance checks.");
            }

            return View(detail);
        }
    }
}
