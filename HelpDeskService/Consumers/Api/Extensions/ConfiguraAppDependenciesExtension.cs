using Application.Client;
using Application.Client.Ports;
using DataEF.Repositories;
using Domain.Ports;

namespace Api.Extensions;

public static class ConfiguraAppDependenciesExtension
{
    public static void ConfiguraAppDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();
        builder.Services.AddTransient<IClientRepository, ClientRepository>();
        builder.Services.AddTransient<IClientManager, ClientManager>();
    }
}
