namespace SFA.DAS.PAS.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public required string UserRef { get; set; }
    public required string DisplayName { get; set; }
    public required int Ukprn { get; set; }
    public required string Email { get; set; }
    public bool IsDeleted { get; set; }
    public UserType UserType { get; set; }
    public DateTime? LastLogin { get; set; }
    public virtual UserSettings? UserSettings { get; set; }
}
