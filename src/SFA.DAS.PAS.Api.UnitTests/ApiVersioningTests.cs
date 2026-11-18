using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.PAS.Api.AppStart;
using SFA.DAS.PAS.Api.Authentication;
using SFA.DAS.PAS.Api.Controllers.Integration;
using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Application.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PAS.Api.UnitTests;

public class ApiVersioningTests
{
    [Test]
    [MoqInlineAutoData("GET", "1.0", HttpStatusCode.OK)]
    [MoqInlineAutoData("GET", "1", HttpStatusCode.OK)]
    [MoqInlineAutoData("GET", "2.0", HttpStatusCode.BadRequest)]
    [MoqInlineAutoData("GET", "99.0", HttpStatusCode.BadRequest)]
    [MoqInlineAutoData("GET", null, HttpStatusCode.OK)]
    [MoqInlineAutoData("GET", "invalid", HttpStatusCode.BadRequest)]
    [MoqInlineAutoData("PUT", "1.0", HttpStatusCode.NoContent)]
    [MoqInlineAutoData("PUT", "1", HttpStatusCode.NoContent)]
    [MoqInlineAutoData("PUT", "2.0", HttpStatusCode.BadRequest)]
    [MoqInlineAutoData("PUT", "99.0", HttpStatusCode.BadRequest)]
    [MoqInlineAutoData("PUT", null, HttpStatusCode.BadRequest)]
    [MoqInlineAutoData("PUT", "invalid", HttpStatusCode.BadRequest)]
    public async Task WhenCallingVersionedController_ThenEnforcesDeclaredVersion(
        string method,
        string? version,
        HttpStatusCode expectedStatus,
        Mock<IProviderService> providerService,
        Mock<IUserService> userService,
        Mock<IUserSettingsService> userSettingsService)
    {
        providerService.Setup(service => service.Exists(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        userService.Setup(service => service.GetUsersByUkprn(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<UserModel>());
        userSettingsService.Setup(service => service.UpsertUserSettings(It.IsAny<UserSettingsModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers().AddApplicationPart(typeof(AccountController).Assembly);
        builder.Services.AddApiVersioningRegistration();
        builder.Services.AddAuthentication(new ConfigurationBuilder().Build(), isLocalEnvironment: true);
        builder.Services.AddSingleton(providerService.Object);
        builder.Services.AddSingleton(userService.Object);
        builder.Services.AddSingleton(userSettingsService.Object);

        await using var app = builder.Build();
        app.Urls.Add("http://127.0.0.1:0");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        await app.StartAsync();

        using var client = new HttpClient { BaseAddress = new Uri(app.Urls.Single()) };
        using var request = new HttpRequestMessage(new HttpMethod(method),
            method == "GET" ? "/api/account/10000001/users" : "/users/test-user/settings");
        if (version is not null)
        {
            request.Headers.Add("X-Version", version);
        }
        if (method == "PUT")
        {
            request.Content = JsonContent.Create(new { ReceiveNotifications = true });
        }

        using var response = await client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.That(response.StatusCode, Is.EqualTo(expectedStatus), responseBody);
        if (expectedStatus == HttpStatusCode.BadRequest)
        {
            providerService.VerifyNoOtherCalls();
            userService.VerifyNoOtherCalls();
            userSettingsService.VerifyNoOtherCalls();
        }
    }
}
