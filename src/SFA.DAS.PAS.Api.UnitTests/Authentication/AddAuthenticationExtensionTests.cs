using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.PAS.Api.Authentication;

namespace SFA.DAS.PAS.Api.UnitTests.Authentication;

public class AddAuthenticationExtensionTests
{
    [TestCase(Policies.ReadAccountUsers)]
    [TestCase(Policies.ReadUserSettings)]
    [TestCase(Policies.Management)]
    public async Task WhenRunningLocally_ThenNamedPoliciesAllowAnonymousRequests(string policy)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthentication(new ConfigurationBuilder().Build(), isLocalEnvironment: true);
        using var provider = services.BuildServiceProvider();
        var authorization = provider.GetRequiredService<IAuthorizationService>();

        var result = await authorization.AuthorizeAsync(new ClaimsPrincipal(new ClaimsIdentity()), null, policy);

        Assert.That(result.Succeeded, Is.True);
    }
}
