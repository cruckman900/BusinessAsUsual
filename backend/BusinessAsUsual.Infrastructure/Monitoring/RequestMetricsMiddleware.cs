using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace BusinessAsUsual.Infrastructure.Monitoring
{

    /// <summary>
    /// Middleware that collects and publishes request metrics, including latency and error rates, to a specified metrics
    /// publisher.
    /// </summary>
    /// <remarks>This middleware measures the time taken to process each request and publishes the latency in
    /// milliseconds. If the response status code indicates an error (500 or higher), it also publishes an error rate
    /// metric. The service name and environment can be configured through the application's configuration
    /// settings.</remarks>
    public class RequestMetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMetricPublisher _metrics;
        private readonly string _serviceName;
        private readonly string _environment;

        /// <summary>
        /// Initializes a new instance of the RequestMetricsMiddleware class, which collects and publishes metrics for
        /// incoming HTTP requests.
        /// </summary>
        /// <remarks>The middleware uses the provided configuration to determine the service name and environment,
        /// defaulting to "Backend" and "Production" if not specified. Metrics are published using the specified
        /// IMetricPublisher implementation.</remarks>
        /// <param name="next">The delegate representing the next middleware component in the HTTP request processing pipeline.</param>
        /// <param name="metrics">An implementation of IMetricPublisher used to publish collected request metrics.</param>
        /// <param name="configuration">An IConfiguration instance that provides access to application settings, including service name and environment.</param>
        public RequestMetricsMiddleware(
            RequestDelegate next,
            IMetricPublisher metrics,
            IConfiguration configuration
        )
        {
            _next = next;
            _metrics = metrics;
            _serviceName = configuration["Monitoring:ServiceName"] ?? "UnknownService";
            _environment = configuration["Monitoring:Environment"] ?? "UnknownEnvironment";
        }

        /// <summary>
        /// Processes an HTTP request, records the request latency, and publishes error metrics for server errors.
        /// </summary>
        /// <remarks>This method measures the time taken to process each HTTP request and publishes the latency
        /// metric to a monitoring service. If the response status code is 500 or greater, it also publishes an error rate
        /// metric. This middleware is intended to be used in the ASP.NET Core request pipeline to enable monitoring of
        /// application performance and error rates.</remarks>
        /// <param name="context">The HTTP context for the current request, providing access to request and response information.</param>
        /// <returns>A task that represents the asynchronous operation of processing the HTTP request.</returns>
        public async Task Invoke(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            await _next(context);
            sw.Stop();

            var statusCode = context.Response.StatusCode;
            var isError = statusCode >= 500;

            // Latency
            await _metrics.PublishAsync(
                "LatencyMs",
                sw.ElapsedMilliseconds,
                _serviceName,
                _environment,
                Amazon.CloudWatch.StandardUnit.Milliseconds
            );

            // Requests per minute (increment)
            await _metrics.PublishAsync(
                "RequestsPerMinute",
                1,
                _serviceName,
                _environment,
                Amazon.CloudWatch.StandardUnit.Count
            );

            // Error rate (only if 500+)
            if (isError)
            {
                await _metrics.PublishAsync(
                    "ErrorRate",
                    1,
                    _serviceName,
                    _environment,
                    Amazon.CloudWatch.StandardUnit.Count
                );

            }
        }
    }
}
