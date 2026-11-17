using SFA.DAS.PAS.Domain.Entities;
using SFA.DAS.PAS.Domain.Interfaces;

namespace SFA.DAS.PAS.Data.Repositories;

internal class UserSettingsRepository(IPasDataContext pasDataContext) : IUserSettingsRepository
{
    public async Task Insert(UserSettings userSettings, CancellationToken cancellationToken)
    {
        await pasDataContext.UserSettings.AddAsync(userSettings, cancellationToken);
        await pasDataContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(UserSettings userSettings, CancellationToken cancellationToken)
    {
        await pasDataContext.SaveChangesAsync(cancellationToken);
    }
}
