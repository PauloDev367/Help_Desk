using DataEF;
using IdentityAuth;
using Microsoft.EntityFrameworkCore;

namespace Api.Extensions;

public static class ConfigureAppDbContextExtension
{
    public static void ConfiguraADbContext(this WebApplicationBuilder builder)
    {
        var connString = builder.Configuration.GetConnectionString("SqlServer");
        builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connString));
        builder.Services.AddDbContext<AuthDbContext>(opt => opt.UseSqlServer(connString));
    }
}
