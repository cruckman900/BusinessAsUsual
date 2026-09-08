using System;

namespace BusinessAsUsual.Infrastructure.Middleware
{
    /// <summary>
    /// Marks a controller or action as exempt from mandatory tenant context resolution.
    /// Apply this to endpoints that must be reachable without a resolved tenant, such as
    /// authentication/login endpoints, health checks, or provisioning endpoints.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class AllowAnonymousTenantAttribute : Attribute
    {
    }
}
