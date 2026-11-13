using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddTransient<IPasDataContext, PasDataContext>(provider => provider.GetService<PasDataContext>()!);

        services
            .AddHealthChecks()
            .AddDbContextCheck<PasDataContext>();

        return services;
    }
}
