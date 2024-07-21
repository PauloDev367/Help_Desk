using Application.Client;
using Application.Client.Ports;
using Application.Support;
using Application.Support.Ports;
using DataEF.Repositories;
using Domain.Ports;
using IdentityAuth;

namespace Api.Extensions;

public static class ConfiguraAppDependenciesExtension
{
    public static void ConfiguraAppDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();
        builder.Services.AddTransient<IClientRepository, ClientRepository>();
        builder.Services.AddTransient<IClientManager, ClientManager>();
        builder.Services.AddTransient<ISupportRepository, SupportRepostory>();
        builder.Services.AddTransient<ISupportManager, SupportManager>();
        builder.Services.AddTransient<IAuthUserService, IdentityService>();
    }
}
