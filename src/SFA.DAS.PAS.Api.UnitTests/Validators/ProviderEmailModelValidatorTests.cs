using AutoFixture.NUnit4;
using FluentValidation.TestHelper;
using SFA.DAS.PAS.Api.Models;
using SFA.DAS.PAS.Api.Validators;

namespace SFA.DAS.PAS.Api.UnitTests.Validators;

public class ProviderEmailModelValidatorTests
{
    private readonly ProviderEmailModelValidator _sut = new();

    [Test, AutoData]
    public void WhenUkprnIsPositive_AndTemplateIdIsSupplied_ThenValidationPasses(ProviderEmailModel model)
    {
        model.Ukprn = 10000001;

        var result = _sut.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    [InlineAutoData(0)]
    [InlineAutoData(-1)]
    public void WhenUkprnIsNotPositive_AndTemplateIdIsSupplied_ThenValidationFailsOnUkprnOnly(int ukprn, ProviderEmailModel model)
    {
        model.Ukprn = ukprn;

        var result = _sut.TestValidate(model);

        Assert.Multiple(() =>
        {
            result.ShouldHaveValidationErrorFor(x => x.Ukprn);
            result.ShouldNotHaveValidationErrorFor(x => x.TemplateId);
        });
    }

    [Test]
    [InlineAutoData("")]
    [InlineAutoData("   ")]
    public void WhenTemplateIdIsMissing_AndUkprnIsPositive_ThenValidationFailsOnTemplateIdOnly(string templateId, ProviderEmailModel model)
    {
        model.Ukprn = 10000001;
        model.TemplateId = templateId;

        var result = _sut.TestValidate(model);

        Assert.Multiple(() =>
        {
            result.ShouldHaveValidationErrorFor(x => x.TemplateId);
            result.ShouldNotHaveValidationErrorFor(x => x.Ukprn);
        });
    }

    [Test, AutoData]
    public void WhenTemplateIdIsNull_AndUkprnIsPositive_ThenValidationFailsOnTemplateIdOnly(ProviderEmailModel model)
    {
        model.Ukprn = 10000001;
        model.TemplateId = null!;

        var result = _sut.TestValidate(model);

        Assert.Multiple(() =>
        {
            result.ShouldHaveValidationErrorFor(x => x.TemplateId);
            result.ShouldNotHaveValidationErrorFor(x => x.Ukprn);
        });
    }

    [Test, AutoData]
    public void WhenUkprnIsNotPositive_AndTemplateIdIsMissing_ThenValidationFailsOnBoth(ProviderEmailModel model)
    {
        model.Ukprn = 0;
        model.TemplateId = string.Empty;

        var result = _sut.TestValidate(model);

        Assert.Multiple(() =>
        {
            result.ShouldHaveValidationErrorFor(x => x.Ukprn);
            result.ShouldHaveValidationErrorFor(x => x.TemplateId);
        });
    }
}
