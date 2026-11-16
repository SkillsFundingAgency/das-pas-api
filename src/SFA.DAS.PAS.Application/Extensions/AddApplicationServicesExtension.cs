using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.PAS.Application.Services;

namespace SFA.DAS.PAS.Application.Extensions;

public static class AddApplicationServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        return services;
    }
}
