using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Domain.Entities;
using SFA.DAS.PAS.Domain.Interfaces;

namespace SFA.DAS.PAS.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserModel>> GetUsersByUkprn(int ukprn, CancellationToken cancellationToken);
}

public class UserService(IUserRepository _userRepository) : IUserService
{
    public async Task<IEnumerable<UserModel>> GetUsersByUkprn(int ukprn, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetUsersByUkprn(ukprn, cancellationToken);
        return users.Select(u => new UserModel
        {
            UserRef = u.UserRef,
            EmailAddress = u.Email,
            DisplayName = u.DisplayName,
            ReceiveNotifications = u.UserSettings?.ReceiveNotifications ?? true,
            IsSuperUser = u.UserType == UserType.SuperUser
        });
    }
}
