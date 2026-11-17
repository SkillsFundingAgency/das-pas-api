using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.PAS.Api.Authentication;
using SFA.DAS.PAS.Api.Infrastructure;
using SFA.DAS.PAS.Api.Models;
using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Application.Services;

namespace SFA.DAS.PAS.Api.Controllers;

[Route("users/{userRef}/settings")]
[ApiController]
[ApiVersion(1)]
[RequireApiVersionHeader]
[Authorize(Policy = Policies.Management)]
[Tags(ApiTags.Management)]
public class UserSettingsController(IUserSettingsService userSettingsService) : ControllerBase
{
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserSettings([FromRoute] string userRef, [FromBody] UpdateUserSettingsModel userSettings, CancellationToken cancellationToken)
    {
        var updated = await userSettingsService.UpsertUserSettings(new UserSettingsModel(userRef, userSettings.ReceiveNotifications), cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }
}
