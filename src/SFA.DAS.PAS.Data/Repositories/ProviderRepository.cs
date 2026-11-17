using Microsoft.EntityFrameworkCore;
using SFA.DAS.PAS.Domain.Interfaces;

namespace SFA.DAS.PAS.Data.Repositories;

internal class ProviderRepository(IPasDataContext context) : IProviderRepository
{
    public Task<bool> Exists(int ukprn, CancellationToken cancellationToken)
    {
        return context.Providers.AnyAsync(provider => provider.Ukprn == ukprn, cancellationToken);
    }
}
