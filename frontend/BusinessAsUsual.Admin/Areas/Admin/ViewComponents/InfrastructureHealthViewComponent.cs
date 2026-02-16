using BusinessAsUsual.Admin.Areas.Admin.Models.Monitoring;
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
        /// Generates a view component result that displays infrastructure health metrics for RDS and EC2 based on the
        /// provided platform health data.
        /// </summary>
        /// <remarks>The method evaluates alarm statuses for RDS and EC2 and determines their display
        /// colors according to the current health state. The resulting view provides a visual summary of infrastructure
        /// health for monitoring purposes.</remarks>
        /// <param name="model">The platform health data containing infrastructure alarm statuses for RDS and EC2 components. Cannot be
        /// null.</param>
        /// <returns>A view component result containing the rendered view with health metric cards for RDS and EC2.</returns>
        public IViewComponentResult Invoke(PlatformHealthDto model)
        {
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

            string rdsColor = rdsAlarms.Values.Contains("ALARM")
                ? "red"
                : rdsAlarms.Values.Contains("INSUFFICIENT_DATA")
                    ? "yellow"
                    : "green";

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

            string ec2Color = ec2Alarms.Values.Contains("ALARM")
                ? "red"
                : ec2Alarms.Values.Contains("INSUFFICIENT_DATA")
                    ? "yellow"
                    : "green";

            var ec2Card = new MetricCardModel
            {
                Title = "EC2",
                Color = ec2Color,
                InfraAlarms = ec2Alarms
            };

            // Pass both cards to the view
            return View(new[] { rdsCard, ec2Card });
        }
    }
}
