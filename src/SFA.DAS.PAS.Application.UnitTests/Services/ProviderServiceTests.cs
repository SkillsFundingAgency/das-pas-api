using AutoFixture.NUnit4;
using Moq;
using SFA.DAS.PAS.Application.Services;
using SFA.DAS.PAS.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PAS.Application.UnitTests.Services;

public class ProviderServiceTests
{
    [Test]
    [MoqInlineAutoData(true)]
    [MoqInlineAutoData(false)]
    public async Task WhenCheckingProviderExists_ThenReturnsRepositoryResult(
        bool exists,
        int ukprn,
        [Frozen] Mock<IProviderRepository> repositoryMock,
        ProviderService sut)
    {
        using var cancellation = new CancellationTokenSource();
        repositoryMock.Setup(repository => repository.Exists(ukprn, cancellation.Token)).ReturnsAsync(exists);

        var result = await sut.Exists(ukprn, cancellation.Token);

        Assert.That(result, Is.EqualTo(exists));
        repositoryMock.Verify(repository => repository.Exists(ukprn, cancellation.Token), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }
}
