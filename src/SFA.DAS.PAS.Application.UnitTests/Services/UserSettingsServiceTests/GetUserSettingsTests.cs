using AutoFixture.NUnit4;
using Moq;
using SFA.DAS.PAS.Application.Services;
using SFA.DAS.PAS.Domain.Entities;
using SFA.DAS.PAS.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PAS.Application.UnitTests.Services.UserSettingsServiceTests;

public class GetUserSettingsTests
{
    [Test, RecursiveMoqAutoData]
    public async Task WhenGettingUserSettings_ThenReturnsSavedSettings(
        User user,
        UserSettings existing,
        [Frozen] Mock<IUserRepository> userRepositoryMock,
        [Frozen] Mock<IUserSettingsRepository> repositoryMock,
        UserSettingsService sut)
    {
        using var cancellation = new CancellationTokenSource();
        user.UserSettings = existing;
        userRepositoryMock.Setup(repository => repository.GetUserByUserRef(user.UserRef, cancellation.Token)).ReturnsAsync(user);

        var result = await sut.GetUserSettings(user.UserRef, cancellation.Token);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.UserRef, Is.EqualTo(user.UserRef));
            Assert.That(result.EmailAddress, Is.EqualTo(user.Email));
            Assert.That(result.DisplayName, Is.EqualTo(user.DisplayName));
            Assert.That(result.ReceiveNotifications, Is.EqualTo(existing.ReceiveNotifications));
            Assert.That(result.IsSuperUser, Is.EqualTo(user.UserType == UserType.SuperUser));
        });
        userRepositoryMock.Verify(repository => repository.GetUserByUserRef(user.UserRef, cancellation.Token), Times.Once);
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
        userRepositoryMock.Setup(repository => repository.GetUserByUserRef(userRef, cancellation.Token)).ReturnsAsync((User?)null);

        var result = await sut.GetUserSettings(userRef, cancellation.Token);

        Assert.That(result, Is.Null);
        userRepositoryMock.Verify(repository => repository.GetUserByUserRef(userRef, cancellation.Token), Times.Once);
        userRepositoryMock.VerifyNoOtherCalls();
        repositoryMock.VerifyNoOtherCalls();
    }
}
