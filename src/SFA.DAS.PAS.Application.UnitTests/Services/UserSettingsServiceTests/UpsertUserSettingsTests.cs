using AutoFixture.NUnit4;
using Moq;
using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Application.Services;
using SFA.DAS.PAS.Domain.Entities;
using SFA.DAS.PAS.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PAS.Application.UnitTests.Services.UserSettingsServiceTests;

public class UpsertUserSettingsTests
{
    [Test, RecursiveMoqAutoData]
    public async Task WhenUpsertingUserSettings_ThenLoadsUserWithSettingsOnce(
        UserSettingsModel model,
        User user,
        UserSettings existing,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Frozen] Mock<IUserSettingsRepository> repositoryMock,
        UserSettingsService sut)
    {
        using var cancellation = new CancellationTokenSource();
        userRepositoryMock.Setup(repository => repository.GetUserByUserRef(model.UserRef, cancellation.Token)).ReturnsAsync(user);
        user.IsDeleted = false;
        user.UserSettings = existing;

        var result = await sut.UpsertUserSettings(model, cancellation.Token);

        Assert.That(result, Is.True);
        userRepositoryMock.Verify(repository => repository.GetUserByUserRef(model.UserRef, cancellation.Token), Times.Once);
        userRepositoryMock.VerifyNoOtherCalls();
        repositoryMock.Verify(repository => repository.Update(existing, cancellation.Token), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    [RecursiveMoqInlineAutoData(true)]
    [RecursiveMoqInlineAutoData(false)]
    public async Task WhenUpsertingUserSettings_AndSettingsDoNotExist_ThenInsertsNewSettings(
        bool receiveNotifications,
        UserSettingsModel model,
        User user,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Frozen] Mock<IUserSettingsRepository> repositoryMock,
        UserSettingsService sut)
    {
        using var cancellation = new CancellationTokenSource();
        model = model with { ReceiveNotifications = receiveNotifications };
        user.UserRef = model.UserRef;
        user.IsDeleted = false;
        user.UserSettings = null;
        userRepositoryMock.Setup(repository => repository.GetUserByUserRef(model.UserRef, cancellation.Token)).ReturnsAsync(user);

        var result = await sut.UpsertUserSettings(model, cancellation.Token);

        Assert.That(result, Is.True);
        repositoryMock.Verify(repository => repository.Insert(
            It.Is<UserSettings>(settings => settings.UserId == user.Id && settings.UserRef == model.UserRef && settings.ReceiveNotifications == receiveNotifications),
            cancellation.Token), Times.Once);
        repositoryMock.Verify(repository => repository.Update(It.IsAny<UserSettings>(), It.IsAny<CancellationToken>()), Times.Never);
        repositoryMock.VerifyNoOtherCalls();
        userRepositoryMock.Verify(repository => repository.GetUserByUserRef(model.UserRef, cancellation.Token), Times.Once);
        userRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    [RecursiveMoqInlineAutoData(true)]
    [RecursiveMoqInlineAutoData(false)]
    public async Task WhenUpsertingUserSettings_AndSettingsExist_ThenUpdatesExistingSettings(
        bool receiveNotifications,
        UserSettingsModel model,
        User user,
        UserSettings existing,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Frozen] Mock<IUserSettingsRepository> repositoryMock,
        UserSettingsService sut)
    {
        using var cancellation = new CancellationTokenSource();
        userRepositoryMock.Setup(repository => repository.GetUserByUserRef(model.UserRef, cancellation.Token)).ReturnsAsync(user);
        model = model with { ReceiveNotifications = receiveNotifications };
        existing.ReceiveNotifications = !receiveNotifications;
        user.IsDeleted = false;
        user.UserSettings = existing;

        await sut.UpsertUserSettings(model, cancellation.Token);

        Assert.That(existing.ReceiveNotifications, Is.EqualTo(receiveNotifications));
        repositoryMock.Verify(repository => repository.Update(existing, cancellation.Token), Times.Once);
        repositoryMock.Verify(repository => repository.Insert(It.IsAny<UserSettings>(), It.IsAny<CancellationToken>()), Times.Never);
        repositoryMock.VerifyNoOtherCalls();
        userRepositoryMock.Verify(repository => repository.GetUserByUserRef(model.UserRef, cancellation.Token), Times.Once);
        userRepositoryMock.VerifyNoOtherCalls();
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenUpsertingUserSettings_AndSettingsExist_ThenDoesNotChangeSettingsIdentity(
        UserSettingsModel model,
        User user,
        UserSettings existing,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Frozen] Mock<IUserSettingsRepository> repositoryMock,
        UserSettingsService sut)
    {
        using var cancellation = new CancellationTokenSource();
        userRepositoryMock.Setup(repository => repository.GetUserByUserRef(model.UserRef, cancellation.Token)).ReturnsAsync(user);
        var (id, userId, userRef) = (existing.Id, existing.UserId, existing.UserRef);
        user.IsDeleted = false;
        user.UserSettings = existing;

        await sut.UpsertUserSettings(model, cancellation.Token);

        Assert.Multiple(() =>
        {
            Assert.That(existing.Id, Is.EqualTo(id));
            Assert.That(existing.UserId, Is.EqualTo(userId));
            Assert.That(existing.UserRef, Is.EqualTo(userRef));
        });
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenUpsertingUserSettings_AndUserDoesNotExist_ThenReturnsFalseWithoutWritingSettings(
        UserSettingsModel model,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Frozen] Mock<IUserSettingsRepository> repositoryMock,
        UserSettingsService sut)
    {
        using var cancellation = new CancellationTokenSource();
        userRepositoryMock.Setup(repository => repository.GetUserByUserRef(model.UserRef, cancellation.Token)).ReturnsAsync((User?)null);

        var result = await sut.UpsertUserSettings(model, cancellation.Token);

        Assert.That(result, Is.False);
        userRepositoryMock.Verify(repository => repository.GetUserByUserRef(model.UserRef, cancellation.Token), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    [RecursiveMoqInlineAutoData(true)]
    [RecursiveMoqInlineAutoData(false)]
    public async Task WhenUpsertingUserSettings_AndUserIsDeleted_ThenReturnsFalseWithoutChangingOrWritingSettings(
        bool hasSettings,
        UserSettingsModel model,
        User user,
        UserSettings existing,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Frozen] Mock<IUserSettingsRepository> repositoryMock,
        UserSettingsService sut)
    {
        using var cancellation = new CancellationTokenSource();
        user.IsDeleted = true;
        existing.ReceiveNotifications = !model.ReceiveNotifications;
        user.UserSettings = hasSettings ? existing : null;
        userRepositoryMock.Setup(repository => repository.GetUserByUserRef(model.UserRef, cancellation.Token)).ReturnsAsync(user);

        var result = await sut.UpsertUserSettings(model, cancellation.Token);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(existing.ReceiveNotifications, Is.EqualTo(!model.ReceiveNotifications));
            Assert.That(user.UserSettings, Is.SameAs(hasSettings ? existing : null));
        });
        userRepositoryMock.Verify(repository => repository.GetUserByUserRef(model.UserRef, cancellation.Token), Times.Once);
        userRepositoryMock.VerifyNoOtherCalls();
        repositoryMock.VerifyNoOtherCalls();
    }
}
