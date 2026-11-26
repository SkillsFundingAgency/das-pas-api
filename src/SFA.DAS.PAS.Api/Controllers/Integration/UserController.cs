using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.PAS.Api.Authentication;
using SFA.DAS.PAS.Api.Infrastructure;
using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Application.Services;

namespace SFA.DAS.PAS.Api.Controllers.Integration;

[Route("api/[controller]")]
[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = Policies.ReadUserSettings)]
[Tags(ApiTags.Integration)]
public class UserController(IUserSettingsService userSettingsService) : ControllerBase
{
    [HttpGet("{userRef}")]
    [ProducesResponseType<UserModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserSettings([FromRoute] string userRef, CancellationToken cancellationToken)
    {
        var settings = await userSettingsService.GetUserSettings(userRef, cancellationToken);
        if (settings is null)
        {
            return NotFound();
        }

        return Ok(settings);
    }
}
