using Microsoft.OpenApi.Models;

namespace Api.Extensions;

public static class ConfigureSwaggerExtension
{
    public static void ConfigureSwagger(this WebApplicationBuilder builder)
    {
        
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Estudo",
                Version = "v1",
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT authorization header using Bearer scheme.
                        Enter 'Bearer' [space] and the your token in the text input below.
                        Example: 'Bearer 123123asdasda'
                        ",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id="Bearer"
                        },
                        Scheme = "oauth2",
                        Name= "Bearer",
                        In = ParameterLocation.Header
                    } , new List<string>()
                }

            });

        });
    }
}