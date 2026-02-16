using BusinessAsUsual.Admin.Areas.Admin.Models.Monitoring;
using BusinessAsUsual.Admin.Dtos;
using BusinessAsUsual.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusinessAsUsual.Web.Areas.Admin.ViewComponents
{
    /// <summary>
    /// Represents a view component that displays the health status of infrastructure components, such as RDS and EC2.
    /// </summary>
    /// <remarks>This component retrieves and displays health metrics for RDS and EC2 instances. If the
    /// provided model is null, it fetches the current platform health asynchronously. The displayed metrics include
    /// alarms for storage, CPU, memory, and connection statuses, with color coding based on alarm states.</remarks>
    public class InfrastructureHealthViewComponent : ViewComponent
    {
        private readonly IMonitoringService _monitoring;

        /// <summary>
        /// Initializes a new instance of the InfrastructureHealthViewComponent class using the specified monitoring
        /// service.
        /// </summary>
        /// <remarks>Ensure that the provided IMonitoringService instance is properly configured before
        /// passing it to this constructor. An invalid or null service may result in runtime errors when attempting to
        /// access infrastructure health information.</remarks>
        /// <param name="monitoring">The monitoring service used to retrieve infrastructure health data. This parameter cannot be null.</param>
        public InfrastructureHealthViewComponent(IMonitoringService monitoring)
        {
            _monitoring = monitoring;
        }

        /// <summary>
        /// Invokes the view component to render platform health metrics, including the current status of RDS and EC2
        /// alarms.
        /// </summary>
        /// <remarks>If the provided model is null, the method synchronously fetches the latest platform
        /// health data to ensure the view displays up-to-date metrics.</remarks>
        /// <param name="model">A model containing platform health data. If null, the method retrieves the latest platform health metrics
        /// before rendering.</param>
        /// <returns>An <see cref="IViewComponentResult"/> that renders the view displaying health metrics for RDS and EC2
        /// resources.</returns>
        public IViewComponentResult Invoke(PlatformHealthDto model)
        {
            // ---------------------------------------------
            // If model is null (initial page load), fetch it
            // ---------------------------------------------
            if (model == null)
            {
                model = _monitoring.GetPlatformHealthAsync().GetAwaiter().GetResult();
            }

            var infra = model.Infrastructure;

            // -----------------------------
            // RDS CARD MODEL
            // -----------------------------
            var rdsAlarms = new Dictionary<string, string>
            {
                { "Free Storage Low", infra.RdsFreeStorageLow ?? string.Empty },
                { "CPU High", infra.RdsCpuHigh ?? string.Empty },
                { "Freeable Memory Low", infra.RdsFreeableMemoryLow ?? string.Empty },
                { "Connections High", infra.RdsConnectionsHigh ?? string.Empty }
            };

            string rdsColor =
                rdsAlarms.Values.Contains("ALARM") ? "red" :
                rdsAlarms.Values.Contains("INSUFFICIENT_DATA") ? "yellow" :
                "green";

            var rdsCard = new MetricCardModel
            {
                Title = "RDS",
                Color = rdsColor,
                InfraAlarms = rdsAlarms
            };

            // -----------------------------
            // EC2 CARD MODEL
            // -----------------------------
            var ec2Alarms = new Dictionary<string, string>
            {
                { "CPU High", infra.Ec2CpuHigh ?? string.Empty },
                { "Status Check Failed", infra.Ec2StatusCheckFailed ?? string.Empty }
            };

            string ec2Color =
                ec2Alarms.Values.Contains("ALARM") ? "red" :
                ec2Alarms.Values.Contains("INSUFFICIENT_DATA") ? "yellow" :
                "green";

            var ec2Card = new MetricCardModel
            {
                Title = "EC2",
                Color = ec2Color,
                InfraAlarms = ec2Alarms
            };

            return View(new[] { rdsCard, ec2Card });
        }
    }
}