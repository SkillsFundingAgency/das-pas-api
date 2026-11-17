using AutoFixture.NUnit4;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.PAS.Api.Controllers;
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
        serviceMock.Setup(service => service.UpsertUserSettings(model, cancellation.Token)).Returns(Task.CompletedTask);

        var result = await sut.UpdateUserSettings(model, cancellation.Token);

        Assert.That(result, Is.TypeOf<NoContentResult>());
        serviceMock.Verify(service => service.UpsertUserSettings(model, cancellation.Token), Times.Once);
        serviceMock.VerifyNoOtherCalls();
    }
}
