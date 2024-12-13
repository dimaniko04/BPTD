using System.Text;
using FundRaising.Server.BLL.Interfaces.Repository;
using FundRaising.Server.BLL.Interfaces.Services;
using FundRaising.Server.DAL.Repository;
using FundRaising.Server.DAL.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FundRaising.Server.DAL;

public static class DependencyInjection
{
    public static void AddDal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.ConfigureDbContext(configuration);
        services.ConfigureAuth(configuration);
        services.ConfigureServices();
        services.ConfigureRepositories();
    }

    private static void ConfigureAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);
        
        services.AddSingleton(Options.Create(jwtSettings));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });
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

    private static void ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
    }

    private static void ConfigureRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFundraisersRepository, FundraiserRepository>();
    }
}