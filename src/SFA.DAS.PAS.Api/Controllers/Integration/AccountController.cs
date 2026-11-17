using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.PAS.Api.Authentication;
using SFA.DAS.PAS.Api.Infrastructure;
using SFA.DAS.PAS.Application.Models;
using SFA.DAS.PAS.Application.Services;

namespace SFA.DAS.PAS.Api.Controllers.Integration;

[Route("api")]
[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = Policies.ReadAccountUsers)]
[Tags(ApiTags.Integration)]
public class AccountController(IUserService _userService, IProviderService providerService) : ControllerBase
{
    [HttpGet]
    [Route("account/{ukprn:int}/users")]
    [ProducesResponseType<IEnumerable<UserModel>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProviderUsers(int ukprn, CancellationToken cancellationToken)
    {
        if (!await providerService.Exists(ukprn, cancellationToken))
        {
            return NotFound();
        }

        var users = await _userService.GetUsersByUkprn(ukprn, cancellationToken);
        return Ok(users);
    }
}
