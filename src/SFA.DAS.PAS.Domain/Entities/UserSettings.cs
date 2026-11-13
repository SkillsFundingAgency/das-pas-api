namespace SFA.DAS.PAS.Domain.Entities;

public class UserSettings
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string UserRef { get; set; }
    public bool ReceiveNotifications { get; set; }
}
