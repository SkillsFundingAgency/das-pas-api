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
    [Test]
    [RecursiveMoqInlineAutoData(true, UserType.NormalUser)]
    [RecursiveMoqInlineAutoData(false, UserType.NormalUser)]
    [RecursiveMoqInlineAutoData(true, UserType.SuperUser)]
    [RecursiveMoqInlineAutoData(false, UserType.SuperUser)]
    public async Task WhenGettingUserSettings_ThenReturnsSavedSettings(
        bool receiveNotifications,
        UserType userType,
        User user,
        UserSettings existing,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Frozen] Mock<IUserSettingsRepository> repositoryMock,
        UserSettingsService sut)
    {
        using var cancellation = new CancellationTokenSource();
        existing.ReceiveNotifications = receiveNotifications;
        user.UserSettings = existing;
        user.UserType = userType;
        userRepositoryMock.Setup(repository => repository.GetActiveUserByUserRef(user.UserRef, cancellation.Token)).ReturnsAsync(user);

        var result = await sut.GetUserSettings(user.UserRef, cancellation.Token);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.UserRef, Is.EqualTo(user.UserRef));
            Assert.That(result.EmailAddress, Is.EqualTo(user.Email));
            Assert.That(result.DisplayName, Is.EqualTo(user.DisplayName));
            Assert.That(result.ReceiveNotifications, Is.EqualTo(receiveNotifications));
            Assert.That(result.IsSuperUser, Is.EqualTo(userType == UserType.SuperUser));
        });
        userRepositoryMock.Verify(repository => repository.GetActiveUserByUserRef(user.UserRef, cancellation.Token), Times.Once);
        userRepositoryMock.VerifyNoOtherCalls();
        repositoryMock.VerifyNoOtherCalls();
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenGettingUserSettings_AndSettingsDoNotExist_ThenDefaultsToReceivingNotifications(
        User user,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Frozen] Mock<IUserSettingsRepository> repositoryMock,
        UserSettingsService sut)
    {
        using var cancellation = new CancellationTokenSource();
        user.UserSettings = null;
        userRepositoryMock.Setup(repository => repository.GetActiveUserByUserRef(user.UserRef, cancellation.Token)).ReturnsAsync(user);

        var result = await sut.GetUserSettings(user.UserRef, cancellation.Token);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.UserRef, Is.EqualTo(user.UserRef));
            Assert.That(result.EmailAddress, Is.EqualTo(user.Email));
            Assert.That(result.DisplayName, Is.EqualTo(user.DisplayName));
            Assert.That(result.ReceiveNotifications, Is.True);
            Assert.That(result.IsSuperUser, Is.EqualTo(user.UserType == UserType.SuperUser));
        });
        userRepositoryMock.Verify(repository => repository.GetActiveUserByUserRef(user.UserRef, cancellation.Token), Times.Once);
        userRepositoryMock.VerifyNoOtherCalls();
        repositoryMock.VerifyNoOtherCalls();
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenGettingUserSettings_AndUserDoesNotExist_ThenReturnsNull(
        string userRef,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Frozen] Mock<IUserSettingsRepository> repositoryMock,
        UserSettingsService sut)
    {
        using var cancellation = new CancellationTokenSource();
        userRepositoryMock.Setup(repository => repository.GetActiveUserByUserRef(userRef, cancellation.Token)).ReturnsAsync((User?)null);

        var result = await sut.GetUserSettings(userRef, cancellation.Token);

        Assert.That(result, Is.Null);
        userRepositoryMock.Verify(repository => repository.GetActiveUserByUserRef(userRef, cancellation.Token), Times.Once);
        userRepositoryMock.VerifyNoOtherCalls();
        repositoryMock.VerifyNoOtherCalls();
    }

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
        userRepositoryMock.Setup(repository => repository.GetActiveUserByUserRef(model.UserRef, cancellation.Token)).ReturnsAsync(user);
        user.UserSettings = existing;

        var result = await sut.UpsertUserSettings(model, cancellation.Token);

        Assert.That(result, Is.True);
        userRepositoryMock.Verify(repository => repository.GetActiveUserByUserRef(model.UserRef, cancellation.Token), Times.Once);
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
        user.UserSettings = null;
        userRepositoryMock.Setup(repository => repository.GetActiveUserByUserRef(model.UserRef, cancellation.Token)).ReturnsAsync(user);

        var result = await sut.UpsertUserSettings(model, cancellation.Token);

        Assert.That(result, Is.True);
        repositoryMock.Verify(repository => repository.Insert(
            It.Is<UserSettings>(settings => settings.UserId == user.Id && settings.UserRef == model.UserRef && settings.ReceiveNotifications == receiveNotifications),
            cancellation.Token), Times.Once);
        repositoryMock.Verify(repository => repository.Update(It.IsAny<UserSettings>(), It.IsAny<CancellationToken>()), Times.Never);
        repositoryMock.VerifyNoOtherCalls();
        userRepositoryMock.Verify(repository => repository.GetActiveUserByUserRef(model.UserRef, cancellation.Token), Times.Once);
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
        userRepositoryMock.Setup(repository => repository.GetActiveUserByUserRef(model.UserRef, cancellation.Token)).ReturnsAsync(user);
        model = model with { ReceiveNotifications = receiveNotifications };
        existing.ReceiveNotifications = !receiveNotifications;
        user.UserSettings = existing;

        await sut.UpsertUserSettings(model, cancellation.Token);

        Assert.That(existing.ReceiveNotifications, Is.EqualTo(receiveNotifications));
        repositoryMock.Verify(repository => repository.Update(existing, cancellation.Token), Times.Once);
        repositoryMock.Verify(repository => repository.Insert(It.IsAny<UserSettings>(), It.IsAny<CancellationToken>()), Times.Never);
        repositoryMock.VerifyNoOtherCalls();
        userRepositoryMock.Verify(repository => repository.GetActiveUserByUserRef(model.UserRef, cancellation.Token), Times.Once);
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
        userRepositoryMock.Setup(repository => repository.GetActiveUserByUserRef(model.UserRef, cancellation.Token)).ReturnsAsync(user);
        var (id, userId, userRef) = (existing.Id, existing.UserId, existing.UserRef);
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
    public async Task WhenUserDoesNotExist_ThenReturnsFalseWithoutWritingSettings(
        UserSettingsModel model,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Frozen] Mock<IUserSettingsRepository> repositoryMock,
        UserSettingsService sut)
    {
        using var cancellation = new CancellationTokenSource();
        userRepositoryMock.Setup(repository => repository.GetActiveUserByUserRef(model.UserRef, cancellation.Token)).ReturnsAsync((User?)null);

        var result = await sut.UpsertUserSettings(model, cancellation.Token);

        Assert.That(result, Is.False);
        userRepositoryMock.Verify(repository => repository.GetActiveUserByUserRef(model.UserRef, cancellation.Token), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }
}
