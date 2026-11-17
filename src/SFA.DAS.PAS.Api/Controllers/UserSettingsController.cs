using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.PAS.Api.Authentication;
using SFA.DAS.PAS.Api.Infrastructure;
using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Application.Services;

namespace SFA.DAS.PAS.Api.Controllers;

[Route("users/{userref}/settings")]
[ApiController]
[Authorize(Policy = Policies.Management)]
[Tags(ApiTags.Management)]
public class UserSettingsController(IUserSettingsService userSettingsService) : ControllerBase
{
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateUserSettings([FromBody] UserSettingsModel userSettings, CancellationToken cancellationToken)
    {
        await userSettingsService.UpsertUserSettings(userSettings, cancellationToken);
        return NoContent();
    }
}
