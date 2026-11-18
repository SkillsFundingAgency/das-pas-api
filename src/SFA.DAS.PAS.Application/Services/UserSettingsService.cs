using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Domain.Entities;
using SFA.DAS.PAS.Domain.Interfaces;

namespace SFA.DAS.PAS.Application.Services;

public interface IUserSettingsService
{
    Task<bool> UpsertUserSettings(UserSettingsModel userSettings, CancellationToken cancellationToken);
}

public class UserSettingsService(IUserSettingsRepository _userSettingsRepository, IUserRepository userRepository) : IUserSettingsService
{
    public async Task<bool> UpsertUserSettings(UserSettingsModel userSettings, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetActiveUserByUserRef(userSettings.UserRef, cancellationToken);
        if (user is null)
        {
            return false;
        }

        var entity = user.UserSettings;
        if (entity is null)
        {
            entity = new UserSettings
            {
                UserId = user.Id,
                UserRef = user.UserRef,
                ReceiveNotifications = userSettings.ReceiveNotifications,
                User = user
            };
            await _userSettingsRepository.Insert(entity, cancellationToken);
        }
        else
        {
            entity.ReceiveNotifications = userSettings.ReceiveNotifications;
            await _userSettingsRepository.Update(entity, cancellationToken);
        }

        return true;
    }
}
