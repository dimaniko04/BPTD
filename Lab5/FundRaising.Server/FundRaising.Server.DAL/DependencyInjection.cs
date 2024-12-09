using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FundRaising.Server.DAL;

public static class DependencyInjection
{
    public static void AddDal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.ConfigureDbContext(configuration);
    }
    
    private static void ConfigureDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = configuration
                .GetConnectionString("SqlConnection");
            options.UseNpgsql(connectionString);
        });
    }
}