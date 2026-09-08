using Microsoft.AspNetCore.Builder;

namespace BusinessAsUsual.Infrastructure.Middleware
{
    /// <summary>
    /// Extension methods for opting minimal-API endpoints (health checks, OpenAPI docs, etc.)
    /// out of mandatory tenant context resolution enforced by <see cref="TenantResolutionMiddleware"/>.
    /// Use this for endpoints mapped via app.Map*() that cannot carry a C# attribute the way
    /// MVC controllers/actions can with <see cref="AllowAnonymousTenantAttribute"/>.
    /// </summary>
    public static class AllowAnonymousTenantEndpointExtensions
    {
        /// <summary>
        /// Marks the endpoint(s) produced by the builder as exempt from mandatory tenant
        /// context resolution.
        /// </summary>
        public static TBuilder AllowAnonymousTenant<TBuilder>(this TBuilder builder)
            where TBuilder : IEndpointConventionBuilder
        {
            builder.WithMetadata(new AllowAnonymousTenantAttribute());
            return builder;
        }
    }
}
