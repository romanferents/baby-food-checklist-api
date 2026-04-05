using BabyFoodChecklist.Application.Common.Interfaces;
using BabyFoodChecklist.Infrastructure.Auth;
using BabyFoodChecklist.Infrastructure.Data;
using BabyFoodChecklist.Infrastructure.Data.Interceptors;
using BabyFoodChecklist.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BabyFoodChecklist.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory"));
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        // Auth services
        var jwtSection = configuration.GetSection(JwtSettings.SectionName);
        services.Configure<JwtSettings>(opts =>
        {
            opts.SecretKey = jwtSection[nameof(JwtSettings.SecretKey)] ?? string.Empty;
            opts.Issuer = jwtSection[nameof(JwtSettings.Issuer)] ?? string.Empty;
            opts.Audience = jwtSection[nameof(JwtSettings.Audience)] ?? string.Empty;
            if (int.TryParse(jwtSection[nameof(JwtSettings.ExpirationInMinutes)], out var exp))
            {
                opts.ExpirationInMinutes = exp;
            }
        });
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
