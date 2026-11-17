using AutoFixture.NUnit4;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.PAS.Api.Controllers.Integration;
using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Application.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PAS.Api.UnitTests.Controllers;

public class AccountControllerTests
{
    [Test]
    [MoqInlineAutoData(0)]
    [MoqInlineAutoData(2)]
    public async Task WhenGettingUsers_ThenReturnsOkResults(
        int count,
        [Frozen] Mock<IProviderService> providerServiceMock,
        [Frozen] Mock<IUserService> serviceMock,
        [Greedy] AccountController sut,
        int Ukprn)
    {
        using var cancellation = new CancellationTokenSource();
        var users = Enumerable.Range(1, count).Select(id => new UserModel
        {
            UserRef = $"user-{id}",
            EmailAddress = $"user-{id}@example.com",
            DisplayName = $"User {id}",
            ReceiveNotifications = true,
            IsSuperUser = false
        }).ToList();
        providerServiceMock.Setup(service => service.Exists(Ukprn, cancellation.Token)).ReturnsAsync(true);
        serviceMock.Setup(service => service.GetUsersByUkprn(Ukprn, cancellation.Token)).ReturnsAsync(users);

        var result = await sut.GetProviderUsers(Ukprn, cancellation.Token);

        Assert.That(result, Is.TypeOf<OkObjectResult>());
        Assert.That(((OkObjectResult)result).Value, Is.SameAs(users));
        providerServiceMock.Verify(service => service.Exists(Ukprn, cancellation.Token), Times.Once);
        providerServiceMock.VerifyNoOtherCalls();
        serviceMock.Verify(service => service.GetUsersByUkprn(Ukprn, cancellation.Token), Times.Once);
        serviceMock.VerifyNoOtherCalls();
    }

    [Test, MoqAutoData]
    public async Task WhenProviderDoesNotExist_ThenReturnsNotFound(
        int ukprn,
        [Frozen] Mock<IProviderService> providerServiceMock,
        [Frozen] Mock<IUserService> serviceMock,
        [Greedy] AccountController sut)
    {
        using var cancellation = new CancellationTokenSource();
        providerServiceMock.Setup(service => service.Exists(ukprn, cancellation.Token)).ReturnsAsync(false);

        var result = await sut.GetProviderUsers(ukprn, cancellation.Token);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
        providerServiceMock.Verify(service => service.Exists(ukprn, cancellation.Token), Times.Once);
        providerServiceMock.VerifyNoOtherCalls();
        serviceMock.VerifyNoOtherCalls();
    }
}
