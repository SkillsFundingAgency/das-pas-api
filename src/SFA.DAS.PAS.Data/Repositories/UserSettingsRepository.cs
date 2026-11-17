using Microsoft.EntityFrameworkCore;
using SFA.DAS.PAS.Domain.Entities;
using SFA.DAS.PAS.Domain.Interfaces;

namespace SFA.DAS.PAS.Data.Repositories;

internal class UserSettingsRepository(IPasDataContext pasDataContext) : IUserSettingsRepository
{
    public async Task<UserSettings?> GetById(string userRef, CancellationToken cancellationToken)
    {
        return await pasDataContext.UserSettings.FirstOrDefaultAsync(us => us.UserRef == userRef, cancellationToken);
    }

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
