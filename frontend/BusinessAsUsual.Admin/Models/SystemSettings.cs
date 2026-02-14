namespace BusinessAsUsual.Admin.Models
{
    /// <summary>
    /// Represents a set of configuration options for system monitoring, logging, and user interface behavior.
    /// </summary>
    /// <remarks>The SystemSettings class provides properties to control metrics polling intervals, enable or
    /// disable specific monitoring features, configure log retention and logging destinations, and adjust user
    /// interface preferences. These settings can be used to customize the behavior of a monitoring application or
    /// service at runtime.</remarks>
    public class SystemSettings
    {
        // Polling & Refresh

        /// <summary>
        /// Gets or sets the interval, in seconds, at which metrics are polled.
        /// </summary>
        public int MetricsPollingIntervalSeconds { get; set; } = 5;

        /// <summary>
        /// Gets or sets a value indicating whether metrics are automatically refreshed at regular intervals.
        /// </summary>
        public bool AutoRefreshMetrics { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether logs are automatically refreshed when changes occur.
        /// </summary>
        public bool AutoRefreshLogs { get; set; } = true;

        // Metrics Engine

        /// <summary>
        /// Gets or sets a value indicating whether CPU usage metrics are collected.
        /// </summary>
        public bool EnableCpu { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether in-memory storage is enabled.
        /// </summary>
        public bool EnableMemory { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether disk storage is enabled.
        /// </summary>
        public bool EnableDisk { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether network connectivity is enabled.
        /// </summary>
        public bool EnableNetwork { get; set; } = true;

        /// <summary>
        /// Gets or sets the number of data points to retain in the chart history.
        /// </summary>
        public int ChartHistoryLength { get; set; } = 20;

        // Logs

        /// <summary>
        /// Gets or sets the number of days to retain log entries before they are deleted.
        /// </summary>
        public int logRetentionDays { get; set; } = 7;

        /// <summary>
        /// Gets or sets the default log level used for logging operations.
        /// </summary>
        public string DefaultLogLevel { get; set; } = "Information";

        /// <summary>
        /// Gets or sets a value indicating whether Amazon CloudWatch integration is enabled.
        /// </summary>
        public bool EnableCloudWatch { get; set; } = false;

        /// <summary>
        /// Gets or sets the name of the Amazon CloudWatch log group to which logs are written.
        /// </summary>
        public string CloudWatchLogGroup { get; set; } = "";

        // UI & System Behavior

        /// <summary>
        /// Gets or sets the theme mode for the application's user interface.
        /// </summary>
        /// <remarks>Valid values typically include "Light", "Dark", or "System". The default value is
        /// "System", which applies the system's current theme preference. Setting this property affects the appearance
        /// of the application's UI.</remarks>
        public string ThemeMode { get; set; } = "System";

        /// <summary>
        /// Gets or sets a value indicating whether the application should remember the state of the sidebar between
        /// sessions.
        /// </summary>
        public bool RememberSidebarState { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug mode is enabled for the application.
        /// </summary>
        public bool EnableDebugMode { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether dangerous tools are visible to the user.
        /// </summary>
        public bool ShowDangerTools { get; set; } = false;
    }
}
