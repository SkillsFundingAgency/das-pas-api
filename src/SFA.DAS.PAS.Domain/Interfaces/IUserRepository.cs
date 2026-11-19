using SFA.DAS.PAS.Domain.Entities;

namespace SFA.DAS.PAS.Domain.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetUsersByUkprn(int ukprn, CancellationToken cancellationToken);
    Task<User?> GetUserByUserRef(string userRef, CancellationToken cancellationToken);
}
