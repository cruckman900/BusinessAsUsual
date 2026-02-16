using BusinessAsUsual.Admin.Dtos;

namespace BusinessAsUsual.Admin.Areas.Admin.Models.Monitoring
{
    /// <summary>
    /// Represents a data model for displaying service metrics and alarm information on a metric card within the
    /// administration interface.
    /// </summary>
    /// <remarks>The MetricCardModel aggregates key monitoring data, including service-level metrics,
    /// service-level alarms, and infrastructure-level alarms, to provide a comprehensive overview of a service's health
    /// and status. This model is typically used to present real-time operational insights for various services such as
    /// Backend, Admin, Web, RDS, and EC2. Each property supplies essential information for monitoring and alerting
    /// scenarios.</remarks>
    public class MetricCardModel
    {
        // Title of the card (Backend, Admin, Web, RDS, EC2, etc.)
        /// <summary>
        /// Gets or sets the title of the card, which identifies the environment or system represented, such as Backend,
        /// Admin, Web, RDS, or EC2.
        /// </summary>
        public string? Title { get; set; }

        // Status color: "green", "yellow", "red"
        /// <summary>
        /// Gets or sets the status color used to visually represent the current state.
        /// </summary>
        /// <remarks>The value should be one of the predefined options: "green", "yellow", or "red". Using
        /// a value outside these options may result in incorrect or undefined visual representation.</remarks>
        public string? Color { get; set; }

        // Service-level metrics (Latency, RPM, ErrorRate)
        /// <summary>
        /// Gets or sets the service-level metrics, including latency, requests per minute (RPM), and error rate.
        /// </summary>
        /// <remarks>This property provides insights into the performance and reliability of the service,
        /// allowing for monitoring and optimization based on the metrics it exposes.</remarks>
        public ServiceMetricsDto? Metrics { get; set; }

        // Service-level alarms (HighErrorRate, NoTraffic)
        /// <summary>
        /// Gets or sets the service-level alarms, such as high error rates or no traffic conditions.
        /// </summary>
        /// <remarks>Use this property to monitor and respond to critical service conditions that may
        /// require attention. The alarms provide insight into the current operational state of the service.</remarks>
        public ServiceAlarmDto? Alarms { get; set; }

        // Infrastructure-level alarms (RDS, EC2)
        /// <summary>
        /// Gets or sets the infrastructure-level alarms for services such as RDS and EC2.
        /// </summary>
        /// <remarks>Use this property to access or update alarms related to the application's underlying
        /// infrastructure. Monitoring these alarms can help ensure the health and availability of dependent
        /// services.</remarks>
        public InfrastructureAlarmDto? Infrastructure { get; set; }
    }
}