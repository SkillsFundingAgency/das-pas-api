using AutoFixture.NUnit4;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.PAS.Api.Controllers.Integration;
using SFA.DAS.PAS.Api.Models;
using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Application.Services;

namespace SFA.DAS.PAS.Api.UnitTests.Controllers;

public class EmailControllerTests
{
    [Test, AutoData]
    public async Task WhenEmailRequestIsValid_ThenSendsMappedEmailAndReturnsOk(int ukprn, ProviderEmailModel emailModel, CancellationToken cancellationToken)
    {
        var emailServiceMock = new Mock<IEmailService>();
        var sut = new EmailController(emailServiceMock.Object);

        var result = await sut.SendEmailToAllProviderRecipients(ukprn, emailModel, cancellationToken);

        Assert.That(result, Is.TypeOf<OkResult>());
        emailServiceMock.Verify(service => service.SendEmailToAllProviderRecipients(
            It.Is<SendEmailModel>(model =>
                model.Ukprn == ukprn &&
                model.TemplateId == emailModel.TemplateId &&
                ReferenceEquals(model.Tokens, emailModel.Tokens) &&
                model.ExplicitEmailAddresses.SequenceEqual(emailModel.ExplicitEmailAddresses)),
            cancellationToken), Times.Once);
        emailServiceMock.VerifyNoOtherCalls();
    }
}
