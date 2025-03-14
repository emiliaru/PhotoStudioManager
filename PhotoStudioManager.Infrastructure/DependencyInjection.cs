using Microsoft.Extensions.DependencyInjection;
using PhotoStudioManager.Application.Interfaces;
using PhotoStudioManager.Infrastructure.Services;

namespace PhotoStudioManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
