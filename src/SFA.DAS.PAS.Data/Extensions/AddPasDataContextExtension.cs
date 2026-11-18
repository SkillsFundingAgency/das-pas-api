using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.PAS.Data.Repositories;
using SFA.DAS.PAS.Domain.Interfaces;

namespace SFA.DAS.PAS.Data.Extensions;

public static class AddPasDataContextExtension
{
    public static IServiceCollection AddPasDataContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PasDataContext>((serviceProvider, options) =>
        {
            var connection = new SqlConnection(connectionString);

            options.UseSqlServer(connection, o => o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(20), null));
        });

        services
            .AddHealthChecks()
            .AddDbContextCheck<PasDataContext>();

        services.AddTransient<IPasDataContext, PasDataContext>(provider => provider.GetService<PasDataContext>()!);

        AddRepositories(services);

        return services;
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IUserSettingsRepository, UserSettingsRepository>();
        services.AddTransient<IProviderRepository, ProviderRepository>();
    }
}
