using AutoFixture.NUnit4;
using Moq;
using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Application.Services;
using SFA.DAS.PAS.Domain.Entities;
using SFA.DAS.PAS.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PAS.Application.UnitTests.Services;

public class UserSettingsServiceTests
{
    [Test, RecursiveMoqAutoData]
    public async Task WhenUpsertingUserSettings_ThenLooksUpSettingsByUserRef(
        UserSettingsModel model,
        UserSettings existing,
        [Frozen] Mock<IUserSettingsRepository> repositoryMock,
        UserSettingsService sut)
    {
        using var cancellation = new CancellationTokenSource();
        repositoryMock.Setup(repository => repository.GetById(model.UserRef, cancellation.Token)).ReturnsAsync(existing);

        await sut.UpsertUserSettings(model, cancellation.Token);

        repositoryMock.Verify(repository => repository.GetById(model.UserRef, cancellation.Token), Times.Once);
    }

    [Test]
    [RecursiveMoqInlineAutoData(true)]
    [RecursiveMoqInlineAutoData(false)]
    public async Task WhenUpsertingUserSettings_AndSettingsDoNotExist_ThenInsertsNewSettings(
        bool receiveNotifications,
        UserSettingsModel model,
        [Frozen] Mock<IUserSettingsRepository> repositoryMock,
        UserSettingsService sut)
    {
        using var cancellation = new CancellationTokenSource();
        model = model with { ReceiveNotifications = receiveNotifications };
        repositoryMock.Setup(repository => repository.GetById(model.UserRef, cancellation.Token)).ReturnsAsync((UserSettings?)null);

        await sut.UpsertUserSettings(model, cancellation.Token);

        repositoryMock.Verify(repository => repository.Insert(
            It.Is<UserSettings>(settings => settings.UserRef == model.UserRef && settings.ReceiveNotifications == receiveNotifications),
            cancellation.Token), Times.Once);
        repositoryMock.Verify(repository => repository.Update(It.IsAny<UserSettings>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    [RecursiveMoqInlineAutoData(true)]
    [RecursiveMoqInlineAutoData(false)]
    public async Task WhenUpsertingUserSettings_AndSettingsExist_ThenUpdatesExistingSettings(
        bool receiveNotifications,
        UserSettingsModel model,
        UserSettings existing,
        [Frozen] Mock<IUserSettingsRepository> repositoryMock,
        UserSettingsService sut)
    {
        using var cancellation = new CancellationTokenSource();
        model = model with { ReceiveNotifications = receiveNotifications };
        existing.ReceiveNotifications = !receiveNotifications;
        repositoryMock.Setup(repository => repository.GetById(model.UserRef, cancellation.Token)).ReturnsAsync(existing);

        await sut.UpsertUserSettings(model, cancellation.Token);

        Assert.That(existing.ReceiveNotifications, Is.EqualTo(receiveNotifications));
        repositoryMock.Verify(repository => repository.Update(existing, cancellation.Token), Times.Once);
        repositoryMock.Verify(repository => repository.Insert(It.IsAny<UserSettings>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenUpsertingUserSettings_AndSettingsExist_ThenDoesNotChangeSettingsIdentity(
        UserSettingsModel model,
        UserSettings existing,
        [Frozen] Mock<IUserSettingsRepository> repositoryMock,
        UserSettingsService sut)
    {
        using var cancellation = new CancellationTokenSource();
        var (id, userId, userRef) = (existing.Id, existing.UserId, existing.UserRef);
        repositoryMock.Setup(repository => repository.GetById(model.UserRef, cancellation.Token)).ReturnsAsync(existing);

        await sut.UpsertUserSettings(model, cancellation.Token);

        Assert.Multiple(() =>
        {
            Assert.That(existing.Id, Is.EqualTo(id));
            Assert.That(existing.UserId, Is.EqualTo(userId));
            Assert.That(existing.UserRef, Is.EqualTo(userRef));
        });
    }
}
