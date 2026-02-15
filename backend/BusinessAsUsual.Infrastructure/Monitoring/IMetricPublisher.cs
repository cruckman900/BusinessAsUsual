using Amazon.CloudWatch;

namespace BusinessAsUsual.Infrastructure.Monitoring;

/// <summary>
/// Defines a contract for asynchronously publishing a metric to a monitoring system for a specified service and
/// environment.
/// </summary>
/// <remarks>Implementations should ensure reliable delivery of metrics, including appropriate error handling and
/// retry logic as needed. The method is intended to be non-blocking to support high-throughput scenarios. Callers
/// should provide valid metric names and service identifiers to ensure accurate monitoring and reporting.</remarks>
public interface IMetricPublisher
{
    Task PublishAsync(
        string metricName,
        double value,
        string serviceName,
        string environment,
        StandardUnit unit
    );
}
