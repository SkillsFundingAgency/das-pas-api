using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PAS.Application.UnitTests.Models;

public class UserModelTests
{
    [Test, RecursiveMoqAutoData]
    public void WhenCastingFromUser_ThenMapsPropertiesCorrectly(User user)
    {
        // Action
        var userModel = (UserModel)user;

        Assert.Multiple(() =>
        {
            Assert.That(userModel.UserRef, Is.EqualTo(user.UserRef));
            Assert.That(userModel.EmailAddress, Is.EqualTo(user.Email));
            Assert.That(userModel.DisplayName, Is.EqualTo(user.DisplayName));
            Assert.That(userModel.ReceiveNotifications, Is.EqualTo(user.UserSettings?.ReceiveNotifications ?? true));
            Assert.That(userModel.IsSuperUser, Is.EqualTo(user.UserType == UserType.SuperUser));
        });
    }

    [Test, RecursiveMoqAutoData]
    public void WhenCastingFromUser_AndUserSettingsIsNull_ThenReceiveNotificationsIsTrue(User user)
    {
        // Arrange
        user.UserSettings = null;
        // Action
        var userModel = (UserModel)user;
        Assert.That(userModel.ReceiveNotifications, Is.True);
    }

    [Test]
    [RecursiveMoqInlineAutoData(UserType.NormalUser, false)]
    [RecursiveMoqInlineAutoData(UserType.SuperUser, true)]
    public void WhenCastingFromUser_ThenMapsIsSuperUserCorrectly(UserType userType, bool isSuperUser, User user)
    {
        // Arrange
        user.UserType = userType;
        // Action
        var userModel = (UserModel)user;
        Assert.That(userModel.IsSuperUser, Is.EqualTo(isSuperUser));
    }
}
