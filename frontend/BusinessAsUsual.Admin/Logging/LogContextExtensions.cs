using Serilog;
using Serilog.Context;

namespace BusinessAsUsual.Admin.Logging
{
    /// <summary>
    /// Provides extension methods for enriching the Serilog log context with service and user information.
    /// </summary>
    /// <remarks>These methods allow you to attach contextual properties, such as service name and user ID, to
    /// log events. This enables more detailed and structured logging, which can be useful for filtering and analyzing
    /// logs by service or user. The returned <see cref="IDisposable"/> should be disposed when the contextual
    /// information is no longer needed, typically using a <c>using</c> statement.</remarks>
    public static class LogContextExtensions
    {
        /// <summary>
        /// Adds a service name property to the logging context for the duration of the returned scope.
        /// </summary>
        /// <remarks>Use this method to ensure that all log events within the returned scope include the
        /// specified service name. This is useful for correlating logs with specific services in distributed
        /// applications.</remarks>
        /// <param name="serviceName">The name of the service to associate with log events within the scope. Cannot be null.</param>
        /// <returns>An <see cref="IDisposable"/> that, when disposed, removes the service name property from the logging
        /// context.</returns>
        public static IDisposable WithService(string serviceName) =>
            LogContext.PushProperty("Service", serviceName);

        /// <summary>
        /// Adds a "UserId" property to the logging context for the duration of the returned scope.
        /// </summary>
        /// <remarks>Use this method to ensure that all log events within the returned scope include the
        /// specified user identifier. This is useful for correlating log entries with specific users in multi-user
        /// applications.</remarks>
        /// <param name="userId">The identifier of the user to associate with log events. Cannot be null.</param>
        /// <returns>An <see cref="IDisposable"/> that, when disposed, removes the "UserId" property from the logging context.</returns>
        public static IDisposable WithUser(string userId) =>
            LogContext.PushProperty("UserId", userId);
    }
}
