using Asp.Versioning;
using CarShare.API.Contracts.Reservations;
using CarShare.Application.Features.Reservations.Commands.CreateReservation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShare.Application.Features.Reservations.Common;
using CarShare.Application.Features.Reservations.Queries.GetMyReservations;
using CarShare.Application.Features.Reservations.Queries.GetOwnerReservations;
using CarShare.Application.Features.Reservations.Commands.ApproveReservation;
using CarShare.Application.Features.Reservations.Commands.RejectReservation;
using CarShare.Application.Features.Reservations.Commands.CancelReservation;
using CarShare.API.Contracts.Handovers;
using CarShare.Application.Features.Handovers.Commands.ConfirmPickup;
using CarShare.Application.Features.Handovers.Commands.ConfirmReturn;
using CarShare.Application.Features.Reservations.Queries.GetRentalContract;


namespace CarShare.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/reservations")]
public sealed class ReservationsController : ControllerBase
{
    private readonly ISender _sender;

    public ReservationsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Roles = "Renter")]
    [ProducesResponseType(
        typeof(CreateReservationResult),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateReservationResult>> Create(
        CreateReservationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateReservationCommand(
            request.VehicleId,
            request.StartDateTimeUtc,
            request.EndDateTimeUtc,
            request.TravelPurpose);

        var result = await _sender.Send(
            command,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            result);
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Renter")]
    [ProducesResponseType(
    typeof(IReadOnlyList<ReservationViewModel>),
    StatusCodes.Status200OK)]
    public async Task<
    ActionResult<IReadOnlyList<ReservationViewModel>>> GetMine(
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetMyReservationsQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("owner")]
    [Authorize(Roles = "Owner")]
    [ProducesResponseType(
    typeof(IReadOnlyList<ReservationViewModel>),
    StatusCodes.Status200OK)]
    public async Task<
    ActionResult<IReadOnlyList<ReservationViewModel>>> GetForOwner(
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetOwnerReservationsQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpPatch("{reservationId:guid}/approve")]
    [Authorize(Roles = "Owner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Approve(
    Guid reservationId,
    CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ApproveReservationCommand(reservationId),
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("{reservationId:guid}/reject")]
    [Authorize(Roles = "Owner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Reject(
    Guid reservationId,
    CancellationToken cancellationToken)
    {
        await _sender.Send(
            new RejectReservationCommand(reservationId),
            cancellationToken);

        return NoContent();
    }


    [HttpPatch("{reservationId:guid}/cancel")]
    [Authorize(Roles = "Renter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(
    Guid reservationId,
    CancellationToken cancellationToken)
    {
        await _sender.Send(
            new CancelReservationCommand(reservationId),
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{reservationId:guid}/pickup")]
    [Authorize(Roles = "Owner,Renter")]
    [ProducesResponseType(
    typeof(ConfirmPickupResult),
    StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ConfirmPickupResult>> ConfirmPickup(
    Guid reservationId,
    ConfirmPickupRequest request,
    CancellationToken cancellationToken)
    {
        var command = new ConfirmPickupCommand(
            reservationId,
            request.PickupMileage,
            request.PickupPhotoUrl);

        var result = await _sender.Send(
            command,
            cancellationToken);

        return Ok(result);
    }
    [HttpPost("{reservationId:guid}/return")]
    [Authorize(Roles = "Owner,Renter")]
    [ProducesResponseType(
    typeof(ConfirmReturnResult),
    StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ConfirmReturnResult>> ConfirmReturn(
    Guid reservationId,
    ConfirmReturnRequest request,
    CancellationToken cancellationToken)
    {
        var command = new ConfirmReturnCommand(
            reservationId,
            request.ReturnMileage,
            request.ReturnPhotoUrl);

        var result = await _sender.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{reservationId:guid}/contract")]
    [Authorize(Roles = "Owner,Renter")]
    [ProducesResponseType(
    typeof(FileContentResult),
    StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetRentalContract(
    Guid reservationId,
    CancellationToken cancellationToken)
    {
        var pdf = await _sender.Send(
            new GetRentalContractQuery(reservationId),
            cancellationToken);

        var fileName =
            $"CarShare-Ugovor-{reservationId}.pdf";

        return File(
            pdf,
            "application/pdf",
            fileName);
    }
}