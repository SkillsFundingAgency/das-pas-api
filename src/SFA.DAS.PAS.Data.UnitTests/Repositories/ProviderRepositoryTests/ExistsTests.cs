using Microsoft.EntityFrameworkCore;
using SFA.DAS.PAS.Data.Repositories;
using SFA.DAS.PAS.Domain.Entities;

namespace SFA.DAS.PAS.Data.UnitTests.Repositories.ProviderRepositoryTests;

internal class ExistsTests
{
    [TestCase(10000001, true)]
    [TestCase(10000002, false)]
    public async Task WhenCheckingProvider_ThenReturnsWhetherUkprnExists(int ukprn, bool expected)
    {
        var options = new DbContextOptionsBuilder<PasDataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var context = new PasDataContext(options);
        context.Providers.Add(new Provider { Ukprn = 10000001, Name = "Provider" });
        await context.SaveChangesAsync(CancellationToken.None);
        context.ChangeTracker.Clear();
        var sut = new ProviderRepository(context);

        var result = await sut.Exists(ukprn, CancellationToken.None);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void WhenCancellationIsRequested_ThenThrowsOperationCanceledException()
    {
        var options = new DbContextOptionsBuilder<PasDataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        using var context = new PasDataContext(options);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var sut = new ProviderRepository(context);

        Assert.That(async () => await sut.Exists(10000001, cancellation.Token),
            Throws.InstanceOf<OperationCanceledException>());
    }
}
