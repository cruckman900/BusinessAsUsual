namespace BusinessAsUsual.Admin.Services.Logs
{
    /// <summary>
    /// Represents a log event in Serilog's compact JSON format, including timestamp, message, and related metadata.
    /// </summary>
    /// <remarks>This class models the structure of a Serilog log entry as serialized in JSON, enabling
    /// deserialization and analysis of log data produced by Serilog sinks. Property names correspond to Serilog's
    /// compact JSON field conventions (such as '@t' for timestamp and '@m' for message).</remarks>
    public class SerilogJson
    {
        /// <summary>
        /// Gets or sets the date and time associated with this instance.
        /// </summary>
        public DateTime @t { get; set; }

        /// <summary>
        /// Gets or sets the value associated with the property 'm'.
        /// </summary>
        public string @m { get; set; } = "";

        /// <summary>
        /// Gets or sets the value associated with the property 'l'.
        /// </summary>
        public string? @l { get; set; }

        /// <summary>
        /// Gets or sets the value of the x property.
        /// </summary>
        public string? @x { get; set; }
    }
}
