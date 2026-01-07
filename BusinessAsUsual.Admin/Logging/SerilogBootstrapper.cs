using Amazon;
using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Serilog;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.AwsCloudWatch;

namespace BusinessAsUsual.Admin.Logging
{
    /// <summary>
    /// Configures and initializes the Serilog logger with predefined settings for application-wide logging.
    /// </summary>
    /// <remarks>This method sets up Serilog to log messages to both the console and a rolling file, applying
    /// standard enrichers and log level overrides. It should be called once at application startup before any logging
    /// occurs to ensure consistent logging behavior throughout the application.</remarks>
    public class SerilogBootstrapper
    {
        /// <summary>
        /// Configures and initializes the application's logging system with predefined settings.
        /// </summary>
        /// <remarks>This method sets up Serilog as the logging provider, configuring log enrichment,
        /// minimum log levels, and output sinks for both console and file logging. It should be called once at
        /// application startup before any logging is performed to ensure that log events are captured and formatted as
        /// intended. Subsequent calls will overwrite the existing logger configuration.</remarks>
        public static void Initialize()
        {
            try
            {
                var region = Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-1";
                var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
                var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");

                // If AWS is not configured, skip CloudWatch entirely
                if (string.IsNullOrWhiteSpace(region) ||
                    string.IsNullOrWhiteSpace(accessKey) ||
                    string.IsNullOrWhiteSpace(secretKey))
                {
                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console()
                        .CreateLogger();

                    return;
                }

                var client = new AmazonCloudWatchLogsClient(
                    new BasicAWSCredentials(accessKey, secretKey),
                    RegionEndpoint.GetBySystemName(region)
                );

                // Configure Serilog
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithCorrelationId()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Enrich.WithEnvironmentUserName()
                    .Enrich.With(new UserContextEnricher(new HttpContextAccessor()))
                    .Enrich.WithProperty("Service", "BusinessAsUsual.Admin")
                    .WriteTo.Async(a => a.Console(
                        formatter: new Serilog.Formatting.Compact.RenderedCompactJsonFormatter()
                    ))
                    .WriteTo.Async(a => a.File(
                        path: "logs/bau.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        shared: true,
                        restrictedToMinimumLevel: LogEventLevel.Information
                    ))
                    .WriteTo.Seq("http://localhost:5341") // Example Seq server URL; adjust as needed
                    .WriteTo.Console()
                    .WriteTo.AmazonCloudWatch(
                        new CloudWatchSinkOptions
                        {
                            LogGroupName = "BAU-Admin",
                            TextFormatter = new Serilog.Formatting.Compact.RenderedCompactJsonFormatter(),
                            MinimumLogEventLevel = LogEventLevel.Information,
                        },
                        client
                    )
                    .CreateLogger();
            }
            catch (Exception ex)
            {
                // Fallback to console logging
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();

                Log.Error(ex, "Failed to initialize CloudWatch logging");
            }

            // If you want to run Seq locally, you can use Docker: docker run -d --name seq -e ACCEPT_EULA=Y -p 5341:80 datalust/seq
        }
    }
}
