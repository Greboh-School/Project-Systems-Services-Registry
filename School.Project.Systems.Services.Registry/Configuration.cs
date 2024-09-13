using RabbitMQ.Client;
using School.Project.Systems.Services.Registry.Configurations;
using School.Project.Systems.Services.Registry.Models.DTOs;
using School.Project.Systems.Services.Registry.Models.Options;
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
     
        services.Configure<RabbitMQOptions>(Configuration.GetRequiredSection(RabbitMQOptions.Section));
        RabbitMQConfig.Configure(Configuration);
        
        services.AddScoped<IServerRegistry, ServerRegistry>();
        services.AddScoped<IPlayerRegistry, PlayerRegistry>();
        services.AddScoped<IBrokerService, BrokerService>();
        
    }
}