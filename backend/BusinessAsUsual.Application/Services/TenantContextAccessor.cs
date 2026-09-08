using System.Threading;

namespace BusinessAsUsual.Application.Services
{
    /// <summary>
    /// Immutable snapshot of resolved tenant identity, captured at the moment
    /// <see cref="TenantContext.SetContext"/> is called.
    /// </summary>
    /// <param name="CompanyId">The resolved tenant's company identifier.</param>
    /// <param name="TenantDbName">The resolved tenant's database name.</param>
    /// <param name="UserId">The resolved user identifier, if any.</param>
    public sealed record TenantSnapshot(Guid CompanyId, string TenantDbName, Guid? UserId);

    /// <summary>
    /// Provides ambient access to the current tenant snapshot across DI scope boundaries that
    /// scoped services cannot cross - most notably <see cref="IHttpClientFactory"/> message
    /// handlers, whose <c>DelegatingHandler</c> instances are constructed and cached (per
    /// handler lifetime) from an internal factory-managed scope rather than the scope of the
    /// code that calls <c>CreateClient</c>/<c>CreateClient&lt;T&gt;</c>. Injecting a Scoped
    /// <see cref="ITenantContext"/> directly into a message handler is unsafe: the handler may
    /// capture a stale or unresolved tenant on first construction and keep reusing it for every
    /// subsequent circuit/request until the handler is recycled (silently sending the wrong
    /// tenant's headers - a cross-tenant data leak risk).
    /// </summary>
    /// <remarks>
    /// This accessor must be registered as a <b>Singleton</b>. It stores the current tenant as
    /// an <see cref="AsyncLocal{T}"/>, which flows correctly along the logical async call chain -
    /// including a Blazor Server circuit's per-circuit <c>ExecutionContext</c>, which is captured
    /// once when the circuit is created and restored for every dispatched render/event/JS-interop
    /// callback on that circuit. <see cref="TenantContext.SetContext"/> updates this accessor
    /// every time it resolves/re-resolves the scoped tenant context, so the ambient snapshot
    /// always mirrors the current circuit's (or request's) actual tenant.
    /// </remarks>
    public interface ITenantContextAccessor
    {
        /// <summary>
        /// Gets or sets the tenant snapshot flowing through the current logical async call
        /// chain (e.g. the current Blazor circuit or HTTP request). Null when no tenant has
        /// been resolved yet.
        /// </summary>
        TenantSnapshot? Current { get; set; }
    }

    /// <inheritdoc cref="ITenantContextAccessor"/>
    public sealed class TenantContextAccessor : ITenantContextAccessor
    {
        private static readonly AsyncLocal<TenantSnapshot?> _current = new();

        /// <inheritdoc/>
        public TenantSnapshot? Current
        {
            get => _current.Value;
            set => _current.Value = value;
        }
    }
}
