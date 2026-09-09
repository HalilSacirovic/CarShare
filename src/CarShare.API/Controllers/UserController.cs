using Asp.Versioning;
using CarShare.Application.Features.Users.Queries.GetMyProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShare.API.Contracts.Users;
using CarShare.Application.Features.Users.Commands.SubmitDriverLicense;


namespace CarShare.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/users")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("me")]
    [ProducesResponseType(
        typeof(UserProfileViewModel),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<UserProfileViewModel>> GetMe(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetMyProfileQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("me/driver-license")]
    [Authorize(Roles = "Renter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SubmitDriverLicense(
    SubmitDriverLicenseRequest request,
    CancellationToken cancellationToken)
    {
        await _sender.Send(
            new SubmitDriverLicenseCommand(
                request.DriverLicenseUrl),
            cancellationToken);

        return NoContent();
    }
}