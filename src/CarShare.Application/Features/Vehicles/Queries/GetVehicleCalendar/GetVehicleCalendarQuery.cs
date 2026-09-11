using MediatR;

namespace CarShare.Application.Features.Vehicles.Queries.GetVehicleCalendar;

public sealed record GetVehicleCalendarQuery(
    Guid VehicleId)
    : IRequest<
        IReadOnlyList<VehicleUnavailablePeriodViewModel>>;