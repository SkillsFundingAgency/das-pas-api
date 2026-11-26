using Microsoft.EntityFrameworkCore;
using SFA.DAS.PAS.Domain.Entities;
using SFA.DAS.PAS.Domain.Interfaces;

namespace SFA.DAS.PAS.Data.Repositories;

internal class UserRepository(IPasDataContext _pasDataContext) : IUserRepository
{
    public Task<User?> GetUserByUserRef(string userRef, CancellationToken cancellationToken)
    {
        return _pasDataContext.Users
            .Include(user => user.UserSettings)
            .FirstOrDefaultAsync(user => user.UserRef == userRef, cancellationToken);
    }

    public Task<List<User>> GetUsersByUkprn(int ukprn, CancellationToken cancellationToken)
    {
        return _pasDataContext
            .Users
            .Include(u => u.UserSettings)
            .Where(u => u.Ukprn == ukprn && !u.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}
