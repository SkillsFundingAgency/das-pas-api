using AutoFixture.NUnit4;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.PAS.Api.Controllers.Integration;
using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Application.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PAS.Api.UnitTests.Controllers;

public class UserControllerTests
{
    [Test]
    [MoqInlineAutoData(true)]
    [MoqInlineAutoData(false)]
    public async Task WhenGettingUserSettings_ThenReturnsOkWithSettings(
        bool receiveNotifications,
        string userRef,
        [Frozen] Mock<IUserSettingsService> serviceMock,
        [Greedy] UserController sut)
    {
        using var cancellation = new CancellationTokenSource();
        var settings = new UserModel
        {
            UserRef = userRef,
            EmailAddress = "user@example.com",
            DisplayName = "Test User",
            ReceiveNotifications = receiveNotifications,
            IsSuperUser = false
        };
        serviceMock.Setup(service => service.GetUserSettings(userRef, cancellation.Token)).ReturnsAsync(settings);

        var result = await sut.GetUserSettings(userRef, cancellation.Token);

        Assert.That(result, Is.TypeOf<OkObjectResult>());
        Assert.That(((OkObjectResult)result).Value, Is.SameAs(settings));
        serviceMock.Verify(service => service.GetUserSettings(userRef, cancellation.Token), Times.Once);
        serviceMock.VerifyNoOtherCalls();
    }

    [Test, MoqAutoData]
    public async Task WhenUserDoesNotExist_ThenReturnsNotFound(
        string userRef,
        [Frozen] Mock<IUserSettingsService> serviceMock,
        [Greedy] UserController sut)
    {
        using var cancellation = new CancellationTokenSource();
        serviceMock.Setup(service => service.GetUserSettings(userRef, cancellation.Token)).ReturnsAsync((UserModel?)null);

        var result = await sut.GetUserSettings(userRef, cancellation.Token);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
        serviceMock.Verify(service => service.GetUserSettings(userRef, cancellation.Token), Times.Once);
        serviceMock.VerifyNoOtherCalls();
    }
}
