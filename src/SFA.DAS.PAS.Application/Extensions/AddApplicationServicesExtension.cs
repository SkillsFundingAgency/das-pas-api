using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.PAS.Application.Services;

namespace SFA.DAS.PAS.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class AddApplicationServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IUserSettingsService, UserSettingsService>();
        services.AddTransient<IProviderService, ProviderService>();
        services.AddTransient<IEmailService, EmailService>();
        return services;
    }
}
