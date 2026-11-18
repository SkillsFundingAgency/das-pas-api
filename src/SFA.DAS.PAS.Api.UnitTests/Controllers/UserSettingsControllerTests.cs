using AutoFixture.NUnit4;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.PAS.Api.Controllers;
using SFA.DAS.PAS.Api.Models;
using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Application.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PAS.Api.UnitTests.Controllers;

public class UserSettingsControllerTests
{
    [Test]
    [MoqInlineAutoData(true)]
    [MoqInlineAutoData(false)]
    public async Task WhenUpdatingUserSettings_ThenOnSuccessReturnsNoContent(
        bool receiveNotifications,
        [Frozen] Mock<IUserSettingsService> serviceMock,
        [Greedy] UserSettingsController sut)
    {
        using var cancellation = new CancellationTokenSource();
        var model = new UserSettingsModel("user-ref", receiveNotifications);
        serviceMock.Setup(service => service.UpsertUserSettings(model, cancellation.Token)).ReturnsAsync(true);

        var result = await sut.UpdateUserSettings(model.UserRef, new UpdateUserSettingsModel(receiveNotifications), cancellation.Token);

        Assert.That(result, Is.TypeOf<NoContentResult>());
        serviceMock.Verify(service => service.UpsertUserSettings(model, cancellation.Token), Times.Once);
        serviceMock.VerifyNoOtherCalls();
    }

    [Test, MoqAutoData]
    public async Task WhenRouteAndBodyUserRefsDiffer_ThenUsesRouteUserRef(
        string userRef,
        UserSettingsModel model,
        [Frozen] Mock<IUserSettingsService> serviceMock,
        [Greedy] UserSettingsController sut)
    {
        using var cancellation = new CancellationTokenSource();
        var expected = model with { UserRef = userRef };
        serviceMock.Setup(service => service.UpsertUserSettings(expected, cancellation.Token)).ReturnsAsync(true);

        var result = await sut.UpdateUserSettings(userRef, new UpdateUserSettingsModel(model.ReceiveNotifications), cancellation.Token);

        Assert.That(result, Is.TypeOf<NoContentResult>());
        serviceMock.Verify(service => service.UpsertUserSettings(expected, cancellation.Token), Times.Once);
        serviceMock.VerifyNoOtherCalls();
    }

    [Test, MoqAutoData]
    public async Task WhenUserDoesNotExist_ThenReturnsNotFound(
        UserSettingsModel model,
        [Frozen] Mock<IUserSettingsService> serviceMock,
        [Greedy] UserSettingsController sut)
    {
        using var cancellation = new CancellationTokenSource();
        serviceMock.Setup(service => service.UpsertUserSettings(model, cancellation.Token)).ReturnsAsync(false);

        var result = await sut.UpdateUserSettings(model.UserRef, new UpdateUserSettingsModel(model.ReceiveNotifications), cancellation.Token);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
        serviceMock.Verify(service => service.UpsertUserSettings(model, cancellation.Token), Times.Once);
        serviceMock.VerifyNoOtherCalls();
    }
}
