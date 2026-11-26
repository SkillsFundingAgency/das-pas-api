using AutoFixture.NUnit4;
using FluentValidation.Results;
using SFA.DAS.PAS.Api.Extensions;
using SFA.DAS.PAS.Api.Models;

namespace SFA.DAS.PAS.Api.UnitTests.Extensions;

public class ValidationResultExtensionsTests
{
    [Test]
    public void WhenValidationSucceeds_ThenReturnsAnEmptyList()
    {
        var result = new ValidationResult();

        var errors = result.ToValidationErrors();

        Assert.That(errors, Is.Not.Null);
        Assert.That(errors, Is.Empty);
    }

    [Test, AutoData]
    public void WhenValidationFails_AndMultiplePropertiesHaveErrors_ThenReturnsAllErrorsInOrder(List<ValidationFailure> failures)
    {
        var result = new ValidationResult(failures);

        var errors = result.ToValidationErrors();

        Assert.That(errors, Is.EqualTo(failures.Select(failure => new ValidationErrorModel(failure.PropertyName, failure.ErrorMessage))));
    }

    [Test, AutoData]
    public void WhenValidationFails_AndOnePropertyHasMultipleErrors_ThenPreservesEveryError(string propertyName, List<ValidationFailure> failures)
    {
        failures.ForEach(failure => failure.PropertyName = propertyName);
        var result = new ValidationResult(failures);

        var errors = result.ToValidationErrors();

        Assert.That(errors, Is.EqualTo(failures.Select(failure => new ValidationErrorModel(propertyName, failure.ErrorMessage))));
    }
}
