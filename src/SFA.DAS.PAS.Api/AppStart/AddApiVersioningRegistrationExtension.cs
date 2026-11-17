using Asp.Versioning;

namespace SFA.DAS.PAS.Api.AppStart;

public static class AddApiVersioningRegistrationExtension
{
    public static IServiceCollection AddApiVersioningRegistration(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ApiVersionReader = new HeaderApiVersionReader("X-Version");
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
        }).AddMvc();

        return services;
    }
}
