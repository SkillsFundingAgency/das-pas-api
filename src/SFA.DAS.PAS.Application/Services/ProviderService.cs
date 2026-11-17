using SFA.DAS.PAS.Domain.Interfaces;

namespace SFA.DAS.PAS.Application.Services;

public interface IProviderService
{
    Task<bool> Exists(int ukprn, CancellationToken cancellationToken);
}

public class ProviderService(IProviderRepository providerRepository) : IProviderService
{
    public Task<bool> Exists(int ukprn, CancellationToken cancellationToken)
    {
        return providerRepository.Exists(ukprn, cancellationToken);
    }
}
