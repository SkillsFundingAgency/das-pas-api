using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.PAS.Api.Controllers;
using SFA.DAS.PAS.Api.Controllers.Integration;
using SFA.DAS.PAS.Application.Extensions;
using SFA.DAS.PAS.Data.Extensions;

namespace SFA.DAS.PAS.Api.UnitTests;

public class DependencyRegistrationTests
{
    [Test]
    public void WhenRegisteringApplicationAndDataServices_ThenControllersCanBeResolved()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplicationServices();
        services.AddPasDataContext("Server=(localdb)\\MSSQLLocalDB;Database=PasRegistrationTests;Integrated Security=true");
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });
        using var scope = provider.CreateScope();

        Assert.That(ActivatorUtilities.CreateInstance<AccountController>(scope.ServiceProvider), Is.Not.Null);
        Assert.That(ActivatorUtilities.CreateInstance<UserSettingsController>(scope.ServiceProvider), Is.Not.Null);
        Assert.That(ActivatorUtilities.CreateInstance<UserController>(scope.ServiceProvider), Is.Not.Null);
    }
}
