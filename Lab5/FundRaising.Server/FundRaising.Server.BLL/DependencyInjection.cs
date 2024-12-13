using System.Reflection;
using FundRaising.Server.BLL.Services.Auth;
using FundRaising.Server.BLL.Services.Fundraisers;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace FundRaising.Server.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBll(this IServiceCollection services)
    {
        services.ConfigureServices();
        services.ConfigureMapping();

        return services;
    }

    private static void ConfigureServices(
        this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFundraisersService, FundraisersService>();
    }

    private static void ConfigureMapping(
        this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }
}