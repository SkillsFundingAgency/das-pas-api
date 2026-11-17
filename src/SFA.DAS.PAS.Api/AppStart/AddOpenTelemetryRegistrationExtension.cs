using System.Diagnostics.CodeAnalysis;
using Azure.Monitor.OpenTelemetry.AspNetCore;

namespace SFA.DAS.PAS.Api.AppStart;

[ExcludeFromCodeCoverage]
public static class AddOpenTelemetryRegistrationExtension
{
    public static void AddOpenTelemetryRegistration(this IServiceCollection services, string appInsightsConnectionString)
    {
        if (!string.IsNullOrEmpty(appInsightsConnectionString))
        {
            // This service will collect and send telemetry data to Azure Monitor.
            services.AddOpenTelemetry().UseAzureMonitor(options =>
            {
                options.ConnectionString = appInsightsConnectionString;
            });
        }
    }
}
