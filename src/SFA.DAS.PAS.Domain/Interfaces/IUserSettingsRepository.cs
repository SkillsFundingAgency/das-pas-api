using SFA.DAS.PAS.Domain.Entities;

namespace SFA.DAS.PAS.Domain.Interfaces;

public interface IUserSettingsRepository
{
    Task Update(UserSettings userSettings, CancellationToken cancellationToken);
    Task Insert(UserSettings userSettings, CancellationToken cancellationToken);
}
