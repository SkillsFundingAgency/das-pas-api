using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;

namespace SFA.DAS.PAS.Api.Authentication;

public static class AddAuthenticationExtension
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration, bool isLocalEnvironment)
    {
        if (!isLocalEnvironment)
        {
            var azureAdConfiguration = configuration
                .GetSection("AzureAd")
                .Get<AzureActiveDirectoryConfiguration>()!;

            var policies = new Dictionary<string, string>
            {
                {Policies.ReadAccountUsers, Policies.ReadAccountUsers},
                {Policies.ReadUserSettings, Policies.ReadUserSettings},
                {Policies.Management, Policies.Management}
            };

            services.AddAuthentication(azureAdConfiguration, policies);
        }
        else
        {
            services.AddAuthentication();
            services.AddAuthorization(options =>
            {
                foreach (var policyName in new string[] { Policies.ReadAccountUsers, Policies.ReadUserSettings, Policies.Management })
                {
                    options.AddPolicy(policyName, policy =>
                    {
                        policy.RequireAssertion(_ => true);
                    });
                }
            });
        }

        return services;
    }
}
