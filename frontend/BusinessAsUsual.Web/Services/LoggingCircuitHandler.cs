using Microsoft.AspNetCore.Components.Server.Circuits;

namespace BusinessAsUsual.Web.Services
{
    /// <summary>
    /// Logging circuit handler to log connection events.
    /// </summary>
    public class LoggingCircuitHandler : CircuitHandler
    {
        private readonly ILogger<LoggingCircuitHandler> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public LoggingCircuitHandler(ILogger<LoggingCircuitHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// called when a circuit is disconnected
        /// </summary>
        /// <param name="circuit"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _logger.LogWarning("Circuit disconnected: {CircuitId}", circuit.Id);
            return Task.CompletedTask;
        }

        /// <summary>
        /// called when a circuit is connected
        /// </summary>
        /// <param name="circuit"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Circuit connected: {CircuitId}", circuit.Id);
            return Task.CompletedTask;
        }
    }
}