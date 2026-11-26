using FluentValidation.Results;
using SFA.DAS.PAS.Api.Models;

namespace SFA.DAS.PAS.Api.Extensions;

public static class ValidationResultExtensions
{
    public static List<ValidationErrorModel> ToValidationErrors(this ValidationResult validationResult)
    {
        return validationResult.Errors
            .Select(error => new ValidationErrorModel(error.PropertyName, error.ErrorMessage))
            .ToList();
    }
}
