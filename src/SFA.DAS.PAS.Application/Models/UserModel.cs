namespace SFA.DAS.PAS.Application.Models;

public class UserModel
{
    public required string UserRef { get; set; }
    public required string EmailAddress { get; set; }
    public required string DisplayName { get; set; }
    public required bool ReceiveNotifications { get; set; }
    public required bool IsSuperUser { get; set; }
}
