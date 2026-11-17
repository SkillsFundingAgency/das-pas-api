using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Domain.Entities;
using SFA.DAS.PAS.Domain.Interfaces;

namespace SFA.DAS.PAS.Application.Services;

public interface IUserSettingsService
{
    Task UpsertUserSettings(UserSettingsModel userSettings, CancellationToken cancellationToken);
}

public class UserSettingsService(IUserSettingsRepository _userSettingsRepository) : IUserSettingsService
{
    public async Task UpsertUserSettings(UserSettingsModel userSettings, CancellationToken cancellationToken)
    {
        UserSettings? entity = await _userSettingsRepository.GetById(userSettings.UserRef, cancellationToken);
        if (entity is null)
        {
            entity = new UserSettings
            {
                UserRef = userSettings.UserRef,
                ReceiveNotifications = userSettings.ReceiveNotifications
            };
            await _userSettingsRepository.Insert(entity, cancellationToken);
        }
        else
        {
            entity.ReceiveNotifications = userSettings.ReceiveNotifications;
            await _userSettingsRepository.Update(entity, cancellationToken);
        }
    }
}
