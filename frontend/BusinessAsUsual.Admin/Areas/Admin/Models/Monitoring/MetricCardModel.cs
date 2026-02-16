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

        // Service-level alarms (filtered)
        /// <summary>
        /// Gets or sets the collection of service-level alarms, where each entry associates an alarm identifier with
        /// its corresponding message.
        /// </summary>
        /// <remarks>Use this property to retrieve or update alarms related to service health and
        /// performance. Each key-value pair represents a specific alarm, enabling monitoring and alerting for
        /// individual services.</remarks>
        public Dictionary<string, string>? ServiceAlarms { get; set; }

        // Infrastructure-level alarms (filtered)
        /// <summary>
        /// Gets or sets the collection of infrastructure-level alarms, where each key represents an alarm identifier
        /// and each value provides the alarm description.
        /// </summary>
        /// <remarks>Use this property to monitor and manage the current state of infrastructure alarms.
        /// The dictionary allows for efficient retrieval and updating of alarm information, supporting system health
        /// checks and alerting scenarios.</remarks>
        public Dictionary<string, string>? InfraAlarms { get; set; }
    }
}