namespace SFA.DAS.PAS.Domain.Interfaces;

public interface IProviderRepository
{
    Task<bool> Exists(int ukprn, CancellationToken cancellationToken);
}
