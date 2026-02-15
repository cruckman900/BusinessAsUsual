using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;

namespace BusinessAsUsual.Infrastructure.Monitoring
{
    /// <summary>
    /// Publishes custom application metrics to Amazon CloudWatch for monitoring and analysis.
    /// </summary>
    /// <remarks>This class requires an instance of IAmazonCloudWatch to interact with the CloudWatch service.
    /// Metrics are published asynchronously. It is recommended to implement retry logic when using this publisher to
    /// handle transient failures that may occur during metric submission.</remarks>
    public class CloudWatchMetricPublisher : IMetricPublisher
    {
        private readonly IAmazonCloudWatch _cloudWatch;
        private const string Namespace = "BAU/Platform";

        /// <summary>
        /// Initializes a new instance of the CloudWatchMetricPublisher class using the specified Amazon CloudWatch
        /// client.
        /// </summary>
        /// <remarks>Ensure that the provided IAmazonCloudWatch instance is properly configured and
        /// authorized to communicate with AWS services before using this constructor.</remarks>
        /// <param name="cloudWatch">The IAmazonCloudWatch instance used to publish metrics to Amazon CloudWatch. This parameter cannot be null.</param>
        public CloudWatchMetricPublisher(IAmazonCloudWatch cloudWatch)
        {
            _cloudWatch = cloudWatch;
        }

        /// <summary>
        /// Asynchronously publishes a custom metric to Amazon CloudWatch with the specified name, value, and
        /// dimensions.
        /// </summary>
        /// <remarks>Ensure that AWS credentials and permissions are properly configured to allow
        /// publishing metrics to CloudWatch. This method does not throw exceptions for publishing failures; instead,
        /// failures are logged to the console.</remarks>
        /// <param name="metricName">The name of the metric to publish. This value identifies the metric in CloudWatch and cannot be null or
        /// empty.</param>
        /// <param name="value">The numerical value to associate with the metric. Represents the measurement to be recorded.</param>
        /// <param name="serviceName">The name of the service associated with the metric. Used as a dimension to categorize the metric in
        /// CloudWatch.</param>
        /// <param name="environment">The environment in which the service is running, such as 'production' or 'staging'. Used as a dimension in
        /// CloudWatch.</param>
        /// <param name="unit">The unit of measurement for the metric value, specified as a member of the StandardUnit enumeration.</param>
        /// <returns>A task that represents the asynchronous operation of publishing the metric data to CloudWatch.</returns>
        public async Task PublishAsync(
            string metricName,
            double value,
            string serviceName,
            string environment,
            StandardUnit unit
        )
        {
            var request = new PutMetricDataRequest
            {
                Namespace = Namespace,
                MetricData = new List<MetricDatum>
                {
                    new MetricDatum
                    {
                        MetricName = metricName,
                        Timestamp = DateTime.UtcNow,
                        Unit = unit,
                        Value = value,
                        Dimensions = new List<Dimension>
                        {
                            new() { Name = "ServiceName", Value = serviceName },
                            new() { Name = "Environment", Value = environment }
                        }
                    }
                }
            };
            try
            {
                await _cloudWatch.PutMetricDataAsync(request);
                Console.WriteLine($"✅ Published metric: {metricName} with value: {value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to publish metric: {ex.Message}");
                // Consider retry logic or fallback here if needed
            }
        }
    }
}
