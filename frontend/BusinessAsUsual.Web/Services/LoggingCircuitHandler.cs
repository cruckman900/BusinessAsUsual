using Microsoft.AspNetCore.Components.Server.Circuits;

namespace BusinessAsUsual.Web.Services
{
    public class LoggingCircuitHandler : CircuitHandler
    {
        private readonly ILogger<LoggingCircuitHandler> _logger;

        public LoggingCircuitHandler(ILogger<LoggingCircuitHandler> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _logger.LogWarning("Circuit disconnected: {CircuitId}", circuit.Id);
            return Task.CompletedTask;
        }

        public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Circuit connected: {CircuitId}", circuit.Id);
            return Task.CompletedTask;
        }
    }
}