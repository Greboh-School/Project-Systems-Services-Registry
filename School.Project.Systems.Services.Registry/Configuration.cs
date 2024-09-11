using School.Project.Systems.Services.Registry.Services;
using School.Shared.Core.Abstractions;

namespace School.Project.Systems.Services.Registry;

public class Configuration : ServiceConfiguration
{
    public override void InjectMiddleware(IApplicationBuilder builder)
    {
    }

    public override void InjectServiceRegistrations(IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddScoped<IServerRegistry, ServerRegistry>();
        services.AddScoped<IPlayerRegistry, PlayerRegistry>();
    }
}