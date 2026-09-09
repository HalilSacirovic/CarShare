using Asp.Versioning;
using CarShare.API.Contracts.Disputes;
using CarShare.Application.Features.Disputes.Commands.CreateDispute;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShare.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/reservations")]
[Authorize(Roles = "Owner,Renter")]
public sealed class DisputesController : ControllerBase
{
    private readonly ISender _sender;

    public DisputesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("{reservationId:guid}/disputes")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Guid>> CreateDispute(
        Guid reservationId,
        CreateDisputeRequest request,
        CancellationToken cancellationToken)
    {
        var disputeId = await _sender.Send(
            new CreateDisputeCommand(
                reservationId,
                request.Reason,
                request.Description),
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            disputeId);
    }
}