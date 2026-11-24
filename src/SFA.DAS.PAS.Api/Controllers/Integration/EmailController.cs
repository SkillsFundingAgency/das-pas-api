using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.PAS.Api.Authentication;
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
public class EmailController(IEmailService _emailService) : ControllerBase
{
    [HttpPost]
    [Route("{ukprn}/send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendEmailToAllProviderRecipients(int ukprn, [FromBody] ProviderEmailModel emailModel, CancellationToken cancellationToken)
    {
        if (ukprn <= 0 || string.IsNullOrWhiteSpace(emailModel.TemplateId))
        {
            return BadRequest("Invalid ukprn");
        }

        await _emailService.SendEmailToAllProviderRecipients(new SendEmailModel
        {
            Ukprn = ukprn,
            TemplateId = emailModel.TemplateId,
            Tokens = emailModel.Tokens,
            ExplicitEmailAddresses = emailModel.ExplicitEmailAddresses
        }, cancellationToken);
        return Ok();
    }
}
