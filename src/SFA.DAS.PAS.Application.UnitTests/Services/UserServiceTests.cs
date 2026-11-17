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

        var users = await sut.GetUsersByUkprn(user.Ukprn, cancellation.Token);

        Assert.Multiple(() =>
        {
            Assert.That(users.Count(), Is.EqualTo(1));
            Assert.That(users.First().UserRef, Is.EqualTo(user.UserRef));
            Assert.That(users.First().EmailAddress, Is.EqualTo(user.Email));
            Assert.That(users.First().DisplayName, Is.EqualTo(user.DisplayName));
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

        var users = await sut.GetUsersByUkprn(user.Ukprn, cancellation.Token);

        Assert.That(users.First().IsSuperUser, Is.EqualTo(userType == UserType.SuperUser));
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

        var users = await sut.GetUsersByUkprn(user.Ukprn, cancellation.Token);

        Assert.That(users.First().ReceiveNotifications, Is.EqualTo(receiveNotifications));
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenGettingUsers_AndUserSettingsAreMissing_ThenSetsReceiveNotificationsToTrue(
        User user,
        [Frozen] Mock<IUserRepository> repositoryMock,
        UserService sut)
    {
        using var cancellation = new CancellationTokenSource();
        user.UserSettings = null;
        repositoryMock.Setup(repository => repository.GetUsersByUkprn(user.Ukprn, cancellation.Token)).ReturnsAsync([user]);

        var users = await sut.GetUsersByUkprn(user.Ukprn, cancellation.Token);

        Assert.That(users.First().ReceiveNotifications, Is.EqualTo(true));
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenThereAreNoUsers_ThenReturnsEmptyList(
        int ukprn,
        [Frozen] Mock<IUserRepository> repositoryMock,
        UserService sut)
    {
        using var cancellation = new CancellationTokenSource();
        repositoryMock.Setup(repository => repository.GetUsersByUkprn(ukprn, cancellation.Token)).ReturnsAsync([]);

        var result = await sut.GetUsersByUkprn(ukprn, cancellation.Token);

        Assert.That(result, Is.Not.Null.And.Empty);
        repositoryMock.Verify(repository => repository.GetUsersByUkprn(ukprn, cancellation.Token), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }
}
