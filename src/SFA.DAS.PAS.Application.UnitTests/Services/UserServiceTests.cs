using AutoFixture.NUnit4;
using Moq;
using SFA.DAS.PAS.Application.Services;
using SFA.DAS.PAS.Domain.Entities;
using SFA.DAS.PAS.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PAS.Application.UnitTests.Services;

public class UserServiceTests
{
    [Test, RecursiveMoqAutoData]
    public async Task WhenGettingUsers_ThenInvokesRepositoryToGetUsers(
        User user,
        [Frozen] Mock<IUserRepository> repositoryMock,
        UserService sut)
    {
        using var cancellation = new CancellationTokenSource();
        repositoryMock.Setup(repository => repository.GetUsersByUkprn(user.Ukprn, cancellation.Token)).ReturnsAsync([user]);

        var result = (await sut.GetUsersByUkprn(user.Ukprn, cancellation.Token)).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].UserRef, Is.EqualTo(user.UserRef));
            Assert.That(result[0].EmailAddress, Is.EqualTo(user.Email));
            Assert.That(result[0].DisplayName, Is.EqualTo(user.DisplayName));
        });
        repositoryMock.Verify(repository => repository.GetUsersByUkprn(user.Ukprn, cancellation.Token), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    [RecursiveMoqInlineAutoData(UserType.NormalUser)]
    [RecursiveMoqInlineAutoData(UserType.SuperUser)]
    public async Task WhenGettingUsers_ThenSetsSuperUserBaseOnUserType(
        UserType userType,
        User user,
        [Frozen] Mock<IUserRepository> repositoryMock,
        UserService sut)
    {
        using var cancellation = new CancellationTokenSource();
        user.UserType = userType;
        repositoryMock.Setup(repository => repository.GetUsersByUkprn(user.Ukprn, cancellation.Token)).ReturnsAsync([user]);

        var result = (await sut.GetUsersByUkprn(user.Ukprn, cancellation.Token)).ToList();

        Assert.That(result[0].IsSuperUser, Is.EqualTo(userType == UserType.SuperUser));
    }

    [Test]
    [RecursiveMoqInlineAutoData(true)]
    [RecursiveMoqInlineAutoData(false)]
    public async Task WhenGettingUsers_ThenSetsReceiveNotificationsBasedOnUserSettings(
        bool receiveNotifications,
        User user,
        [Frozen] Mock<IUserRepository> repositoryMock,
        UserService sut)
    {
        using var cancellation = new CancellationTokenSource();
        user.UserSettings = new UserSettings { UserRef = user.UserRef, ReceiveNotifications = receiveNotifications };
        repositoryMock.Setup(repository => repository.GetUsersByUkprn(user.Ukprn, cancellation.Token)).ReturnsAsync([user]);

        var result = (await sut.GetUsersByUkprn(user.Ukprn, cancellation.Token)).ToList();

        Assert.That(result[0].ReceiveNotifications, Is.EqualTo(receiveNotifications));
    }

    [Test, MoqAutoData]
    public async Task WhenGettingUsers_AndUserSettingsAreMissing_ThenSetsReceiveNotificationsToTrue(
        User user,
        [Frozen] Mock<IUserRepository> repositoryMock,
        UserService sut)
    {
        using var cancellation = new CancellationTokenSource();
        user.UserSettings = null;
        repositoryMock.Setup(repository => repository.GetUsersByUkprn(user.Ukprn, cancellation.Token)).ReturnsAsync([user]);

        var result = (await sut.GetUsersByUkprn(user.Ukprn, cancellation.Token)).ToList();

        Assert.That(result[0].ReceiveNotifications, Is.EqualTo(true));
    }

}
