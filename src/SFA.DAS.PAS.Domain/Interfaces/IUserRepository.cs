using SFA.DAS.PAS.Domain.Entities;

namespace SFA.DAS.PAS.Domain.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetUsersByUkprn(int ukprn, CancellationToken cancellationToken);
    Task<User?> GetActiveUserByUserRef(string userRef, CancellationToken cancellationToken);
}
