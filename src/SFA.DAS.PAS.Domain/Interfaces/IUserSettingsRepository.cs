using SFA.DAS.PAS.Domain.Entities;

namespace SFA.DAS.PAS.Domain.Interfaces;

public interface IUserSettingsRepository
{
    Task<UserSettings?> GetById(string userRef, CancellationToken cancellationToken);
    Task Update(UserSettings userSettings, CancellationToken cancellationToken);
    Task Insert(UserSettings userSettings, CancellationToken cancellationToken);
}
