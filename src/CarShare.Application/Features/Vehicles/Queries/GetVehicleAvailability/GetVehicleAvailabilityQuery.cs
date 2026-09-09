using CarShare.Application.Features.Vehicles.Common;
using MediatR;

namespace CarShare.Application.Features.Vehicles.Queries.GetVehicleAvailability;

public sealed record GetVehicleAvailabilityQuery(
    Guid VehicleId)
    : IRequest<IReadOnlyList<AvailabilityBlockViewModel>>;