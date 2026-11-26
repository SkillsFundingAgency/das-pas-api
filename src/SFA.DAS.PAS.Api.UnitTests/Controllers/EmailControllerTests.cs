using AutoFixture.NUnit4;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.PAS.Api.Controllers.Integration;
using SFA.DAS.PAS.Api.Models;
using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Application.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PAS.Api.UnitTests.Controllers;

public class EmailControllerTests
{
    [Test, MoqAutoData]
    public async Task WhenEmailRequestIsValid_ThenSendsMappedEmailAndReturnsOk(
        int ukprn,
        ProviderEmailModel emailModel,
        CancellationToken cancellationToken,
        [Frozen] Mock<IValidator<ProviderEmailModel>> validatorMock,
        [Frozen] Mock<IEmailService> emailServiceMock,
        [Greedy] EmailController sut)
    {
        validatorMock.Setup(v => v.ValidateAsync(emailModel, cancellationToken)).ReturnsAsync(new ValidationResult());

        var result = await sut.SendEmailToAllProviderRecipients(ukprn, emailModel, cancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<OkResult>());
            emailServiceMock.Verify(service => service.SendEmailToAllProviderRecipients(
                It.Is<SendEmailModel>(model =>
                    model.Ukprn == ukprn &&
                    model.TemplateId == emailModel.TemplateId &&
                    ReferenceEquals(model.Tokens, emailModel.Tokens) &&
                    model.ExplicitEmailAddresses.SequenceEqual(emailModel.ExplicitEmailAddresses)),
                cancellationToken), Times.Once);
            emailServiceMock.VerifyNoOtherCalls();
        });
    }

    [Test, MoqAutoData]
    public async Task WhenEmailRequestIsValid_ThenUkprnFromRouteIsValidated(
        int ukprn,
        ProviderEmailModel emailModel,
        CancellationToken cancellationToken,
        [Frozen] Mock<IValidator<ProviderEmailModel>> validatorMock,
        [Greedy] EmailController sut)
    {
        validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ProviderEmailModel>(), cancellationToken)).ReturnsAsync(new ValidationResult());

        await sut.SendEmailToAllProviderRecipients(ukprn, emailModel, cancellationToken);

        validatorMock.Verify(v => v.ValidateAsync(It.Is<ProviderEmailModel>(m => m.Ukprn == ukprn), cancellationToken), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenValidationFails_ThenReturnsBadRequestAndDoesNotSendEmail(
        int ukprn,
        ProviderEmailModel emailModel,
        CancellationToken cancellationToken,
        List<ValidationFailure> failures,
        [Frozen] Mock<IValidator<ProviderEmailModel>> validatorMock,
        [Frozen] Mock<IEmailService> emailServiceMock,
        [Greedy] EmailController sut)
    {
        validatorMock.Setup(v => v.ValidateAsync(emailModel, cancellationToken)).ReturnsAsync(new ValidationResult(failures));

        var result = await sut.SendEmailToAllProviderRecipients(ukprn, emailModel, cancellationToken);

        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest, Is.Not.Null);
        var errors = badRequest!.Value as List<ValidationErrorModel>;
        Assert.That(errors, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(badRequest.StatusCode, Is.EqualTo(400));
            Assert.That(errors!.Select(error => (error.PropertyName, error.ErrorMessage)),
                Is.EqualTo(failures.Select(failure => (failure.PropertyName, failure.ErrorMessage))));
            emailServiceMock.VerifyNoOtherCalls();
        });
    }
}
