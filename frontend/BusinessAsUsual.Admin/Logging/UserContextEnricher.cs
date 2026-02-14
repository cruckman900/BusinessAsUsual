using Serilog.Core;
using Serilog.Events;

namespace BusinessAsUsual.Admin.Logging
{
    /// <summary>
    /// Enriches log events with the current user's identity from the HTTP context.
    /// </summary>
    /// <remarks>This enricher adds a property named "User" to each log event, containing the username from
    /// the current HTTP context. If no user is authenticated, the value is set to "anonymous". This is typically used
    /// to include user information in logs for web applications. Thread safety depends on the underlying
    /// IHttpContextAccessor implementation.</remarks>
    public class UserContextEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _http;

        /// <summary>
        /// Initializes a new instance of the UserContextEnricher class using the specified HTTP context accessor.
        /// </summary>
        /// <param name="http">The HTTP context accessor used to retrieve information about the current HTTP request and user context.
        /// Cannot be null.</param>
        public UserContextEnricher(IHttpContextAccessor http)
        {
            _http = http;
        }

        /// <summary>
        /// Enriches the specified log event with a property containing the current user's name.
        /// </summary>
        /// <remarks>If the current HTTP context or user identity is unavailable, the user name property
        /// is set to "anonymous". This method adds the user property only if it is not already present on the log
        /// event.</remarks>
        /// <param name="logEvent">The log event to enrich with additional properties. Cannot be null.</param>
        /// <param name="propertyFactory">The factory used to create log event properties. Cannot be null.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var user = _http.HttpContext?.User?.Identity?.Name ?? "anonymous";
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("User", user));
        }
    }
}
