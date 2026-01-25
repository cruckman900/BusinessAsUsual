namespace BusinessAsUsual.Infrastructure.System.Metrics.Telemetry
{
    /// <summary>
    /// Represents metadata and configuration details about the current application environment, including versioning,
    /// build information, and loaded configuration sources.
    /// </summary>
    /// <remarks>Use this class to access environment-specific information such as the deployment environment
    /// name, application version, commit details, build time, and the sources from which configuration and secrets were
    /// loaded. This information is typically used for diagnostics, logging, or displaying environment details in
    /// application dashboards.</remarks>
    public class EnvironmentInfo
    {
        /// <summary>
        /// Gets or sets the name of the application associated with the current context.
        /// </summary>
        public string? ApplicationName { get; set; } = default;

        /// <summary>
        /// Gets or sets the name of the environment in which the application is running.
        /// </summary>
        public string? Environment { get; set; } = default;

        /// <summary>
        /// Gets or sets the version identifier for the current instance.
        /// </summary>
        public string? Version { get; set; } = default;

        /// <summary>
        /// Gets or sets the commit identifier associated with the current operation.
        /// </summary>
        public string? Commit { get; set; } = default;

        /// <summary>
        /// Gets or sets the date and time when the commit was created.
        /// </summary>
        public string? CommitDate { get; set; } = default;

        /// <summary>
        /// Gets or sets the build timestamp for the current application or component.
        /// </summary>
        public string? BuildTime { get; set; } = default;

        /// <summary>
        /// Gets or sets the collection of configuration source identifiers used by the application.
        /// </summary>
        public IEnumerable<string>? ConfigSources { get; set; } = default;

        /// <summary>
        /// Gets or sets the collection of secret names that have been loaded.
        /// </summary>
        public IEnumerable<string>? SecretsLoaded { get; set; } = default;

        /// <summary>
        /// Gets or sets the collection of container objects associated with this instance.
        /// </summary>
        public IEnumerable<object>? Containers { get; set; }
    }
}
