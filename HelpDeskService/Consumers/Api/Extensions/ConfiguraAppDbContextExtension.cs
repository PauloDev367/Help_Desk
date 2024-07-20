using DataEF;
using Microsoft.EntityFrameworkCore;

namespace Api.Extensions;

public static class ConfiguraAppDbContextExtension
{
    public static void ConfiguraADbContext(this WebApplicationBuilder builder)
    {
        var connString = builder.Configuration.GetConnectionString("SqlServer");
        builder.Services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlServer(connString);
        });
       
    }
}
