using Application.Client;
using Application.Client.Ports;
using Application.Support;
using Application.Support.Ports;
using DataEF.Repositories;
using Domain.Ports;
using IdentityAuth;
using IdentityAuth.Jwt;

namespace Api.Extensions;

public static class ConfiguraAppDependenciesExtension
{
    public static void ConfiguraAppDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();
        builder.Services.AddTransient<IAuthUserService, IdentityService>();
        builder.Services.AddScoped<JwtGenerator>();
        builder.Services.AddTransient<IClientRepository, ClientRepository>();
        builder.Services.AddTransient<IClientManager, ClientManager>();
        builder.Services.AddTransient<ISupportRepository, SupportRepostory>();
        builder.Services.AddTransient<ISupportManager, SupportManager>();
    }
}
