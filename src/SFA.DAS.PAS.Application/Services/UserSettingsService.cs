using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Domain.Entities;
using SFA.DAS.PAS.Domain.Interfaces;

namespace SFA.DAS.PAS.Application.Services;

public interface IUserSettingsService
{
    Task<UserModel?> GetUserSettings(string userRef, CancellationToken cancellationToken);
    Task<bool> UpsertUserSettings(UserSettingsModel userSettings, CancellationToken cancellationToken);
}

public class UserSettingsService(IUserSettingsRepository _userSettingsRepository, IUserRepository userRepository) : IUserSettingsService
{
    public async Task<UserModel?> GetUserSettings(string userRef, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetActiveUserByUserRef(userRef, cancellationToken);
        if (user is null)
        {
            return null;
        }

        return new UserModel
        {
            UserRef = user.UserRef,
            EmailAddress = user.Email,
            DisplayName = user.DisplayName,
            ReceiveNotifications = user.UserSettings?.ReceiveNotifications ?? true,
            IsSuperUser = user.UserType == UserType.SuperUser
        };
    }

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
