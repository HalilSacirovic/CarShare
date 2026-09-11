using Asp.Versioning;
using CarShare.Application.Features.Vehicles.Queries.GetVehicleCalendar;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShare.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route(
    "api/v{version:apiVersion}/vehicles/{vehicleId:guid}/calendar")]
[Authorize]
public sealed class VehicleCalendarController
    : ControllerBase
{
    private readonly ISender _sender;

    public VehicleCalendarController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(
            IReadOnlyList<
                VehicleUnavailablePeriodViewModel>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<
        ActionResult<
            IReadOnlyList<
                VehicleUnavailablePeriodViewModel>>>
        GetCalendar(
            Guid vehicleId,
            CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetVehicleCalendarQuery(vehicleId),
            cancellationToken);

        return Ok(result);
    }
}