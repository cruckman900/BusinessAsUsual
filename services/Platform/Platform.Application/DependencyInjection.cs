using Microsoft.Extensions.DependencyInjection;
using Platform.Application.Interfaces;
using Platform.Application.Services;

namespace Platform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();

        return services;
    }
}
