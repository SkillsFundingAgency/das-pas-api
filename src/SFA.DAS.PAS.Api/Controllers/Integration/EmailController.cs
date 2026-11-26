using Asp.Versioning;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.PAS.Api.Authentication;
using SFA.DAS.PAS.Api.Extensions;
using SFA.DAS.PAS.Api.Infrastructure;
using SFA.DAS.PAS.Api.Models;
using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Application.Services;

namespace SFA.DAS.PAS.Api.Controllers.Integration;

[Route("api/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = Policies.ReadAccountUsers)]
[Tags(ApiTags.Integration)]
public class EmailController(IEmailService _emailService, IValidator<ProviderEmailModel> _validator) : ControllerBase
{
    [HttpPost]
    [Route("{ukprn}/send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<ValidationErrorModel>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendEmailToAllProviderRecipients(int ukprn, [FromBody] ProviderEmailModel emailModel, CancellationToken cancellationToken)
    {
        emailModel.Ukprn = ukprn;

        ValidationResult validationResult = await _validator.ValidateAsync(emailModel, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToValidationErrors());
        }

        await _emailService.SendEmailToAllProviderRecipients(new SendEmailModel
        {
            Ukprn = emailModel.Ukprn,
            TemplateId = emailModel.TemplateId,
            Tokens = emailModel.Tokens,
            ExplicitEmailAddresses = emailModel.ExplicitEmailAddresses
        }, cancellationToken);

        return Ok();
    }
}
