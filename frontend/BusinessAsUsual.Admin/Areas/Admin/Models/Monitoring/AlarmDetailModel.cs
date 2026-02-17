using BusinessAsUsual.Admin.Dtos;

namespace BusinessAsUsual.Admin.Areas.Admin.Models.Monitoring
{
    /// <summary>
    /// Represents the details of an alarm, including its title, current states, recent metrics, and optional links and
    /// descriptions.
    /// </summary>
    /// <remarks>The Alarms property holds the current states of the alarms, while RecentMetrics contains the
    /// last five metric snapshots. CloudWatchLinks and Descriptions are optional and provide additional context for the
    /// alarms.</remarks>
    public class AlarmDetailModel
    {
        /// <summary>
        /// Name of the alarm or the environment it represents (e.g., Backend, Admin, Web, RDS, EC2). This title is used
        /// </summary>
        public string? Title { get; set; }

        // Current alarm states
        /// <summary>
        /// Gets or sets the collection of current alarm states, where each entry maps an alarm identifier to its
        /// corresponding state.
        /// </summary>
        /// <remarks>Use this property to track and update the status of multiple alarms. Each key in the
        /// dictionary represents a unique alarm, and the associated value indicates its current state. This property is
        /// useful for monitoring systems that require easy access to alarm information and state management.</remarks>
        public Dictionary<string, string>? Alarms { get; set; }

        // Recent metric snapshots (last 5 polls)
        /// <summary>
        /// Last five metric snapshots, where each entry maps a timestamp to the corresponding service metrics at that time.
        /// </summary>
        public List<ServiceMetricsDto>? RecentMetrics { get; set; }

        // Optional: CloudWatch console linkes
        /// <summary>
        /// Gets or sets the optional CloudWatch console links associated with the resource.
        /// </summary>
        /// <remarks>This property allows for the storage of links to CloudWatch console dashboards or
        /// metrics, facilitating quick access to monitoring tools. The dictionary's keys represent the names of the
        /// links, while the values are the corresponding URLs.</remarks>
        public Dictionary<string, string>? CloudWatchLinks { get; set; }

        // Optional: Human-readable descriptions
        /// <summary>
        /// Gets or sets a dictionary containing human-readable descriptions associated with specific keys.
        /// </summary>
        /// <remarks>This property allows for the storage of descriptive text that can be used to provide
        /// additional context or information related to the keys in the dictionary. The descriptions can be null if not
        /// set.</remarks>
        public Dictionary<string, string>? Descriptions { get; set; }
    }
}
