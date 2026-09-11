using Asp.Versioning;
using CarShare.API.Contracts.Handovers;
using CarShare.Application.Features.Handovers.Commands.ConfirmPickup;
using CarShare.Application.Features.Handovers.Commands.ConfirmReturn;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShare.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/reservations")]
[Authorize]
public sealed class HandoversController : ControllerBase
{
    private readonly ISender _sender;

    public HandoversController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPatch("{reservationId:guid}/handover/pickup")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ConfirmPickup(
        Guid reservationId,
        ConfirmPickupRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ConfirmPickupCommand(
                reservationId,
                request.PickupMileage,
                request.PickupPhotoUrl),
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("{reservationId:guid}/handover/return")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ConfirmReturn(
        Guid reservationId,
        ConfirmReturnRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ConfirmReturnCommand(
                reservationId,
                request.ReturnMileage,
                request.ReturnPhotoUrl),
            cancellationToken);

        return NoContent();
    }
}