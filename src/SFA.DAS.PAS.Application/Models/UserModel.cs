using SFA.DAS.PAS.Domain.Entities;

namespace SFA.DAS.PAS.Application.Models;

public class UserModel
{
    public required string UserRef { get; set; }
    public required string EmailAddress { get; set; }
    public required string DisplayName { get; set; }
    public required bool ReceiveNotifications { get; set; }
    public required bool IsSuperUser { get; set; }

    public static implicit operator UserModel(User user) => new()
    {
        UserRef = user.UserRef,
        EmailAddress = user.Email,
        DisplayName = user.DisplayName,
        ReceiveNotifications = user.UserSettings?.ReceiveNotifications ?? true,
        IsSuperUser = user.UserType == UserType.SuperUser
    };
}
