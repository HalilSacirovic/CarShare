using Asp.Versioning;
using CarShare.Application.Features.Vehicles.Commands.CreateVehicle;
using CarShare.Application.Features.Vehicles.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarShare.Application.Features.Vehicles.Queries.GetAllVehicles;
using CarShare.Application.Features.Vehicles.Queries.GetVehicleById;
using CarShare.API.Contracts.Vehicles;
using CarShare.Application.Features.Vehicles.Commands.CreateAvailabilityBlock;
using CarShare.Application.Features.Vehicles.Commands.SubmitVehicleVerification;
using CarShare.Application.Features.Vehicles.Queries.SearchAvailableVehicles;
using CarShare.Application.Features.Vehicles.Common;
using CarShare.Application.Features.Vehicles.Queries.GetVehicleAvailability;
using CarShare.Application.Features.Vehicles.Commands.DeleteAvailabilityBlock;
using CarShare.Application.Features.Vehicles.Commands.UpdateVehicle;
using CarShare.Application.Features.Vehicles.Commands.DeleteVehicle;

namespace CarShare.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/vehicles")]
public sealed class VehiclesController : ControllerBase
{
    private readonly ISender _sender;

    public VehiclesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(VehicleViewModel),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]

    [Authorize(Roles = "Owner")]
    public async Task<ActionResult<VehicleViewModel>> Create(
        [FromBody] CreateVehicleCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command,
            cancellationToken);

        return Created(
            $"/api/v1/vehicles/{result.Id}",
            result);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(
    typeof(IReadOnlyList<VehicleViewModel>),
    StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<VehicleViewModel>>> GetAll(
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAllVehiclesQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(
    typeof(VehicleViewModel),
    StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleViewModel>> GetById(
    Guid id,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetVehicleByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{vehicleId:guid}/availability")]
    [Authorize(Roles = "Owner")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAvailabilityBlock(
    Guid vehicleId,
    CreateAvailabilityBlockRequest request,
    CancellationToken cancellationToken)
    {
        var command = new CreateAvailabilityBlockCommand(
            vehicleId,
            request.StartDateTimeUtc,
            request.EndDateTimeUtc,
            request.Reason);

        var id = await _sender.Send(
            command,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            new { id });
    }
    [HttpPatch("{vehicleId:guid}/verification")]
    [Authorize(Roles = "Owner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SubmitVehicleVerification(
    Guid vehicleId,
    SubmitVehicleVerificationRequest request,
    CancellationToken cancellationToken)
    {
        await _sender.Send(
            new SubmitVehicleVerificationCommand(
                vehicleId,
                request.RegistrationDocumentUrl),
            cancellationToken);

        return NoContent();
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(
    typeof(IReadOnlyList<VehicleViewModel>),
    StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<VehicleViewModel>>>
    SearchAvailableVehicles(
        [FromQuery] DateTime startDateTimeUtc,
        [FromQuery] DateTime endDateTimeUtc,
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKilometers,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new SearchAvailableVehiclesQuery(
                startDateTimeUtc,
                endDateTimeUtc,
                latitude,
                longitude,
                radiusKilometers),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{vehicleId:guid}/availability-blocks")]
    [Authorize(Roles = "Owner")]
    [ProducesResponseType(
    typeof(IReadOnlyList<AvailabilityBlockViewModel>),
    StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<
    ActionResult<IReadOnlyList<AvailabilityBlockViewModel>>>
    GetVehicleAvailability(
        Guid vehicleId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetVehicleAvailabilityQuery(vehicleId),
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("availability-blocks/{availabilityBlockId:guid}")]
    [Authorize(Roles = "Owner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAvailabilityBlock(
    Guid availabilityBlockId,
    CancellationToken cancellationToken)
    {
        await _sender.Send(
            new DeleteAvailabilityBlockCommand(
                availabilityBlockId),
            cancellationToken);

        return NoContent();
    }

    [HttpPut("{vehicleId:guid}")]
    [Authorize(Roles = "Owner")]
    [ProducesResponseType(
    typeof(VehicleViewModel),
    StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VehicleViewModel>> UpdateVehicle(
    Guid vehicleId,
    UpdateVehicleRequest request,
    CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new UpdateVehicleCommand(
                vehicleId,
                request.Brand,
                request.Model,
                request.Year,
                request.Color,
                request.Mileage,
                request.SeatCount,
                request.TransmissionType,
                request.FuelType,
                request.Description,
                request.Address,
                request.Latitude,
                request.Longitude,
                request.PricePerHour,
                request.PricePerDay,
                request.DepositAmount,
                request.SmokingAllowed,
                request.PetsAllowed,
                request.DailyMileageLimit,
                request.AutoApproveReservations),
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{vehicleId:guid}")]
    [Authorize(Roles = "Owner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteVehicle(
    Guid vehicleId,
    CancellationToken cancellationToken)
    {
        await _sender.Send(
            new DeleteVehicleCommand(vehicleId),
            cancellationToken);

        return NoContent();
    }
}