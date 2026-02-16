namespace BusinessAsUsual.Admin.Dtos
{
    /// <summary>
    /// Represents the health status of a platform at a specific point in time, including service metrics and alarms for
    /// individual services.
    /// </summary>
    /// <remarks>The platform health status includes a timestamp indicating when the data was recorded, as
    /// well as collections of metrics and alarms for each monitored service. Each entry in the metrics and alarms
    /// collections is identified by the service name as the key. This class is typically used to aggregate and report
    /// the operational state of multiple services within a platform.</remarks>
    public class PlatformHealthDto
    {
        /// <summary>
        /// Gets or sets the date and time when the event occurred.
        /// </summary>
        /// <remarks>This property is typically used to record the precise moment an event is logged or
        /// processed, which can be useful for auditing, tracking, or chronological ordering of events.</remarks>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets a collection of service metrics, where each entry maps a metric name to its corresponding
        /// metrics data.
        /// </summary>
        /// <remarks>Use this property to access or update metrics associated with various services. Each
        /// key in the dictionary represents the name of a specific metric, and the value provides detailed data for
        /// that metric. This enables monitoring and analysis of service performance or health.</remarks>
        public Dictionary<string, ServiceMetricsDto>? Metrics { get; set; }

        /// <summary>
        /// Gets or sets a collection of service alarms, where each key represents an alarm identifier and each value
        /// contains the corresponding alarm details.
        /// </summary>
        /// <remarks>Use this property to access or update the set of alarms associated with the service.
        /// Each alarm is represented by a <see cref="ServiceAlarmDto"/> instance, which provides information about the
        /// alarm's status and configuration. Modifying this dictionary allows for the management of multiple alarms by
        /// their unique identifiers.</remarks>
        public Dictionary<string, ServiceAlarmDto>? Alarms { get; set; }

        /// <summary>
        /// Gets or sets the infrastructure alarm data transfer object, which contains information about the current
        /// state of infrastructure alarms.
        /// </summary>
        /// <remarks>Use this property to access or update details related to infrastructure alarms, such
        /// as their status or configuration. Monitoring this property enables effective tracking and management of
        /// infrastructure-related alerts within the platform.</remarks>
        public InfrastructureAlarmDto? Infrastructure { get; set; }
    }

    /// <summary>
    /// Represents a set of performance metrics for a service, including latency, request rate, and error rate.
    /// </summary>
    /// <remarks>Use this data transfer object to convey key operational statistics for a service, enabling
    /// monitoring and analysis of its performance over time. The metrics provided can help identify trends, detect
    /// anomalies, and support decision-making related to service reliability and efficiency.</remarks>
    public class ServiceMetricsDto
    {
        /// <summary>
        /// Gets or sets the latency, in milliseconds, for the operation.
        /// </summary>
        /// <remarks>A lower value indicates better performance. This property can be used to monitor or
        /// analyze the responsiveness of the associated operation.</remarks>
        public double LatencyMs { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of requests that are allowed per minute.
        /// </summary>
        /// <remarks>Use this property to control the rate of incoming requests and prevent system
        /// overload. Setting this value too high may result in performance degradation or trigger rate-limiting
        /// mechanisms.</remarks>
        public double RequestsPerMinute { get; set; }

        /// <summary>
        /// Gets or sets the error rate as a percentage, representing the proportion of failed operations to total
        /// operations.
        /// </summary>
        /// <remarks>This property is useful for monitoring the performance and reliability of the system.
        /// A higher error rate may indicate issues that need to be addressed.</remarks>
        public double ErrorRate { get; set; }
    }

    /// <summary>
    /// Represents a data transfer object that contains information about service alarm conditions, such as high error
    /// rates and lack of traffic.
    /// </summary>
    /// <remarks>This class is typically used to convey alarm status information for monitoring and alerting
    /// purposes in service health dashboards or APIs.</remarks>
    public class ServiceAlarmDto
    {
        /// <summary>
        /// Gets or sets the threshold that defines a high error rate condition.
        /// </summary>
        /// <remarks>This property is used to specify the error rate value at which the system should
        /// consider the error rate to be high. The format and interpretation of the value depend on the application's
        /// requirements. Setting this property appropriately is important for effective monitoring and
        /// alerting.</remarks>
        public string? HighErrorRate { get; set; }

        /// <summary>
        /// Gets or sets the status indicating whether there is no traffic.
        /// </summary>
        /// <remarks>This property can be used to determine if the system is currently experiencing no
        /// traffic, which may affect operational decisions or logging.</remarks>
        public string? NoTraffic { get; set; }
    }

    /// <summary>
    /// Represents a data transfer object that defines alarm threshold values for monitoring the health of RDS and EC2
    /// infrastructure resources.
    /// </summary>
    /// <remarks>This class encapsulates alarm conditions for key infrastructure metrics, such as low storage,
    /// high CPU usage, and failed status checks, for both RDS and EC2 instances. Each property corresponds to a
    /// specific alarm threshold that can be used to trigger monitoring alerts or health checks. This type is typically
    /// used to transfer alarm configuration data between application layers or services.</remarks>
    public class InfrastructureAlarmDto
    {
        /// <summary>
        /// Gets or sets the threshold value that indicates when the free storage space for the RDS instance is
        /// considered low.
        /// </summary>
        /// <remarks>Use this property to monitor the available storage on the RDS instance and trigger
        /// alerts or actions when the free space falls below the specified threshold. Maintaining sufficient free
        /// storage is important to ensure optimal database performance and prevent service interruptions.</remarks>
        public string? RdsFreeStorageLow { get; set; }

        /// <summary>
        /// Gets or sets the CPU utilization percentage for the RDS instance when it is high.
        /// </summary>
        /// <remarks>This property can be used to monitor the performance and resource usage of the RDS
        /// instance. High CPU utilization may indicate the need for optimization or scaling.</remarks>
        public string? RdsCpuHigh { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the freeable memory available in the RDS instance is considered low.
        /// </summary>
        /// <remarks>Monitoring this property can help identify potential memory pressure on the RDS
        /// instance. A low value may indicate that the instance is at risk of performance degradation due to
        /// insufficient available memory.</remarks>
        public string? RdsFreeableMemoryLow { get; set; }

        /// <summary>
        /// Gets or sets the high threshold for the number of RDS connections.
        /// </summary>
        /// <remarks>This property is used to monitor when the number of active connections to the RDS
        /// instance exceeds an acceptable limit. Configuring this value appropriately can help prevent performance
        /// degradation.</remarks>
        public string? RdsConnectionsHigh { get; set; }

        /// <summary>
        /// Gets or sets the CPU utilization status for EC2 instances when usage is considered high.
        /// </summary>
        /// <remarks>This property can be used to monitor and report when EC2 instance CPU usage exceeds a
        /// defined threshold, which may indicate the need for scaling or performance tuning.</remarks>
        public string? Ec2CpuHigh { get; set; }

        /// <summary>
        /// Gets or sets the status of the EC2 instance status check failure.
        /// </summary>
        /// <remarks>This property indicates whether the EC2 instance has failed any of the status checks.
        /// A value of "true" signifies a failure, while "false" indicates that the instance has passed the
        /// checks.</remarks>
        public string? Ec2StatusCheckFailed { get; set; }
    }
}
