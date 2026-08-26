using BusinessAsUsual.Application.Services;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace BusinessAsUsual.Web.Services
{
    /// <summary>
    /// Circuit handler that initializes tenant context at the start of each Blazor Server circuit.
    /// For development/testing, uses a default test tenant. In production, would extract from
    /// authentication claims or parent shell context.
    /// </summary>
    public class TenantContextCircuitHandler : CircuitHandler
    {
        private readonly ITenantContext _tenantContext;
        private readonly ILogger<TenantContextCircuitHandler> _logger;

        // TODO: In production, get these from authentication/parent shell
        private static readonly Guid DefaultCompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private const string DefaultTenantDb = "TestTenant";

        public TenantContextCircuitHandler(
            ITenantContext tenantContext,
            ILogger<TenantContextCircuitHandler> logger)
        {
            _tenantContext = tenantContext;
            _logger = logger;
        }

        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            // Initialize tenant context for this circuit
            if (!_tenantContext.IsResolved)
            {
                _tenantContext.SetContext(DefaultCompanyId, DefaultTenantDb);
                _logger.LogInformation(
                    "Tenant context initialized for circuit {CircuitId}: CompanyId={CompanyId}, TenantDb={TenantDb}",
                    circuit.Id,
                    DefaultCompanyId,
                    DefaultTenantDb);
            }

            return base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }

        public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            // Re-initialize if circuit reconnects
            if (!_tenantContext.IsResolved)
            {
                _tenantContext.SetContext(DefaultCompanyId, DefaultTenantDb);
                _logger.LogDebug(
                    "Tenant context re-initialized for circuit {CircuitId} on reconnection",
                    circuit.Id);
            }

            return base.OnConnectionUpAsync(circuit, cancellationToken);
        }
    }
}
