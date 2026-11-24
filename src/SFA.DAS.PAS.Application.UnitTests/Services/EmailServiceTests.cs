using AutoFixture.NUnit4;
using Moq;
using NServiceBus;
using SFA.DAS.Notifications.Messages.Commands;
using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Application.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PAS.Application.UnitTests.Services;

public class EmailServiceTests
{
    [Test, MoqAutoData]
    public async Task WhenExplicitAddressesAreSupplied_AndAllAreEligible_ThenSendsToEachAddress(
        SendEmailModel model,
        [Frozen] Mock<IUserService> userServiceMock,
        [Frozen] Mock<IMessageSession> messageSessionMock)
    {
        var users = new[]
        {
            CreateUser("first@example.com", isSuperUser: false, receiveNotifications: true),
            CreateUser("second@example.com", isSuperUser: false, receiveNotifications: true)
        };

        var recipients = await SendEmailAndGetRecipients(
            model, ["first@example.com", "second@example.com"], users, userServiceMock, messageSessionMock);

        Assert.That(recipients, Is.EquivalentTo(["first@example.com", "second@example.com"]));
    }

    [Test, MoqAutoData]
    public async Task WhenExplicitAddressesAreSupplied_AndTheyDifferOnlyByCase_ThenSendsOnlyOneEmail(
        SendEmailModel model,
        [Frozen] Mock<IUserService> userServiceMock,
        [Frozen] Mock<IMessageSession> messageSessionMock)
    {
        var users = Array.Empty<UserModel>();

        var recipients = await SendEmailAndGetRecipients(
            model, ["person@example.com", "PERSON@example.com"], users, userServiceMock, messageSessionMock);

        Assert.That(recipients, Is.EqualTo(["person@example.com"]));
    }

    [Test, MoqAutoData]
    public async Task WhenExplicitEmailMatchesUserEmailIgnoringCase_AndNotificationsAreDisabled_ThenSendsNoEmails(
        SendEmailModel model,
        [Frozen] Mock<IUserService> userServiceMock,
        [Frozen] Mock<IMessageSession> messageSessionMock)
    {
        var users = new[]
        {
            CreateUser("person@example.com",  false,  false)
        };

        var recipients = await SendEmailAndGetRecipients(
            model, ["PERSON@example.com"], users, userServiceMock, messageSessionMock);

        Assert.That(recipients, Is.Empty);
    }

    [Test, MoqAutoData]
    public async Task WhenExplicitEmailIsSupplied_AndItMatchesOptedOutUserEmailIgnoringCase_ThenDoesNotSend(
        SendEmailModel model,
        [Frozen] Mock<IUserService> userServiceMock,
        [Frozen] Mock<IMessageSession> messageSessionMock)
    {
        var users = new[]
        {
            CreateUser("person@example.com", isSuperUser: false, receiveNotifications: false)
        };

        var recipients = await SendEmailAndGetRecipients(
            model, ["PERSON@example.com"], users, userServiceMock, messageSessionMock);

        Assert.That(recipients, Is.Empty);
    }

    [Test, MoqAutoData]
    public async Task WhenExplicitAddressesAreSupplied_AndOtherProviderUsersAreEligible_ThenSendsOnlyToExplicitAddresses(
        SendEmailModel model,
        [Frozen] Mock<IUserService> userServiceMock,
        [Frozen] Mock<IMessageSession> messageSessionMock)
    {
        var users = new[]
        {
            CreateUser("explicit@example.com", isSuperUser: false, receiveNotifications: true),
            CreateUser("other-provider@example.com", isSuperUser: false, receiveNotifications: true)
        };

        var recipients = await SendEmailAndGetRecipients(
            model, ["explicit@example.com"], users, userServiceMock, messageSessionMock);

        Assert.That(recipients, Is.EquivalentTo(["explicit@example.com"]));
    }

    [Test, MoqAutoData]
    public async Task WhenExplicitAddressesAreSupplied_AndSomeUsersHaveOptedOut_ThenSkipsOptedOutAddresses(
        SendEmailModel model,
        [Frozen] Mock<IUserService> userServiceMock,
        [Frozen] Mock<IMessageSession> messageSessionMock)
    {
        var users = new[]
        {
            CreateUser("eligible@example.com", isSuperUser: false, receiveNotifications: true),
            CreateUser("opted-out@example.com", isSuperUser: false, receiveNotifications: false)
        };

        var recipients = await SendEmailAndGetRecipients(
            model, ["eligible@example.com", "opted-out@example.com"], users, userServiceMock, messageSessionMock);

        Assert.That(recipients, Is.EquivalentTo(["eligible@example.com"]));
    }

    [Test, MoqAutoData]
    public async Task WhenExplicitAddressesAreSupplied_AndTheyIncludeUnknownAndOptedOutAddresses_ThenSendsOnlyToUnknownAddresses(
        SendEmailModel model,
        [Frozen] Mock<IUserService> userServiceMock,
        [Frozen] Mock<IMessageSession> messageSessionMock)
    {
        var users = new[]
        {
            CreateUser("opted-out@example.com", isSuperUser: false, receiveNotifications: false)
        };

        var recipients = await SendEmailAndGetRecipients(
            model, ["unknown@example.com", "opted-out@example.com"], users, userServiceMock, messageSessionMock);

        Assert.That(recipients, Is.EquivalentTo(["unknown@example.com"]));
    }

    [Test, MoqAutoData]
    public async Task WhenNoExplicitAddressesAreSupplied_AndNormalUsersExist_ThenSendsOnlyToEligibleNormalUsers(
        SendEmailModel model,
        [Frozen] Mock<IUserService> userServiceMock,
        [Frozen] Mock<IMessageSession> messageSessionMock)
    {
        var users = new[]
        {
            CreateUser("normal@example.com", isSuperUser: false, receiveNotifications: true),
            CreateUser("opted-out@example.com", isSuperUser: false, receiveNotifications: false),
            CreateUser("super@example.com", isSuperUser: true, receiveNotifications: true)
        };

        var recipients = await SendEmailAndGetRecipients(
            model, [], users, userServiceMock, messageSessionMock);

        Assert.That(recipients, Is.EquivalentTo(["normal@example.com"]));
    }

    [Test, MoqAutoData]
    public async Task WhenNoExplicitAddressesAreSupplied_AndOnlySuperUsersExist_ThenSendsToEligibleSuperUsers(
        SendEmailModel model,
        [Frozen] Mock<IUserService> userServiceMock,
        [Frozen] Mock<IMessageSession> messageSessionMock)
    {
        var users = new[]
        {
            CreateUser("super-one@example.com", isSuperUser: true, receiveNotifications: true),
            CreateUser("super-two@example.com", isSuperUser: true, receiveNotifications: false)
        };

        var recipients = await SendEmailAndGetRecipients(
            model, [], users, userServiceMock, messageSessionMock);

        Assert.That(recipients, Is.EquivalentTo(["super-one@example.com"]));
    }

    [Test, MoqAutoData]
    public async Task WhenNoExplicitAddressesAreSupplied_AndNormalAndSuperUsersExist_ThenSendsOnlyToEligibleNormalUsers(
        SendEmailModel model,
        [Frozen] Mock<IUserService> userServiceMock,
        [Frozen] Mock<IMessageSession> messageSessionMock)
    {
        var users = new[]
        {
            CreateUser("opted-in@normal.com", isSuperUser: false, receiveNotifications: true),
            CreateUser("opted-out@normal.com", isSuperUser: false, receiveNotifications: false),
            CreateUser("super@super.com", isSuperUser: true, receiveNotifications: true)
        };

        var recipients = await SendEmailAndGetRecipients(
            model, [], users, userServiceMock, messageSessionMock);

        Assert.That(recipients, Is.EquivalentTo(["opted-in@normal.com"]));
    }

    [Test, MoqAutoData]
    public async Task WhenNoExplicitAddressesAreSupplied_AndAllNormalUsersHaveOptedOut_ThenSendsNoEmails(
        SendEmailModel model,
        [Frozen] Mock<IUserService> userServiceMock,
        [Frozen] Mock<IMessageSession> messageSessionMock)
    {
        var users = new[]
        {
            CreateUser("normal-one@example.com", isSuperUser: false, receiveNotifications: false),
            CreateUser("normal-two@example.com", isSuperUser: false, receiveNotifications: false)
        };

        var recipients = await SendEmailAndGetRecipients(
            model, [], users, userServiceMock, messageSessionMock);

        Assert.That(recipients, Is.Empty);
    }

    [Test, MoqAutoData]
    public async Task WhenNoExplicitAddressesAreSupplied_AndAllSuperUsersHaveOptedOut_ThenSendsNoEmails(
        SendEmailModel model,
        [Frozen] Mock<IUserService> userServiceMock,
        [Frozen] Mock<IMessageSession> messageSessionMock)
    {
        var users = new[]
        {
            CreateUser("super-one@example.com", isSuperUser: true, receiveNotifications: false),
            CreateUser("super-two@example.com", isSuperUser: true, receiveNotifications: false)
        };

        var recipients = await SendEmailAndGetRecipients(
            model, [], users, userServiceMock, messageSessionMock);

        Assert.That(recipients, Is.Empty);
    }

    private static async Task<string[]> SendEmailAndGetRecipients(
        SendEmailModel model,
        IEnumerable<string> explicitAddresses,
        IEnumerable<UserModel> users,
        Mock<IUserService> userServiceMock,
        Mock<IMessageSession> messageSessionMock)
    {
        model.ExplicitEmailAddresses = explicitAddresses;
        var commands = new List<SendEmailCommand>();
        userServiceMock.Setup(service => service.GetUsersByUkprn(model.Ukprn, CancellationToken.None)).ReturnsAsync(users);
        messageSessionMock.Setup(session => session.Send(It.IsAny<object>(), It.IsAny<SendOptions>()))
            .Callback<object, SendOptions>((message, _) => commands.Add((SendEmailCommand)message))
            .Returns(Task.CompletedTask);

        var sut = new EmailService(userServiceMock.Object, messageSessionMock.Object);
        await sut.SendEmailToAllProviderRecipients(model, CancellationToken.None);

        return commands.Select(command => command.RecipientsAddress).ToArray();
    }

    private static UserModel CreateUser(string emailAddress, bool isSuperUser, bool receiveNotifications) => new()
    {
        UserRef = emailAddress,
        EmailAddress = emailAddress,
        DisplayName = emailAddress,
        IsSuperUser = isSuperUser,
        ReceiveNotifications = receiveNotifications
    };
}
