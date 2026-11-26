using FluentValidation;
using SFA.DAS.PAS.Api.Models;

namespace SFA.DAS.PAS.Api.Validators;

public class ProviderEmailModelValidator : AbstractValidator<ProviderEmailModel>
{
    public ProviderEmailModelValidator()
    {
        RuleFor(x => x.Ukprn).GreaterThan(0).WithMessage("Ukprn must be greater than zero.");
        RuleFor(x => x.TemplateId).NotEmpty().WithMessage("TemplateId is required.");
    }
}
