namespace SFA.DAS.PAS.Api.Extensions;

public static class ConfigurationExtensions
{
    public static bool IsLocalEnvironment(this IConfiguration configuration)
    {
        var environmentName = configuration["EnvironmentName"]!;
        return environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
               environmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
    }
}
