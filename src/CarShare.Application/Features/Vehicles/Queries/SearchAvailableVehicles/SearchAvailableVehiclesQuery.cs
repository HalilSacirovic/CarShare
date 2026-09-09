using CarShare.Application.Features.Vehicles.Common;
using MediatR;

namespace CarShare.Application.Features.Vehicles.Queries.SearchAvailableVehicles;

public sealed record SearchAvailableVehiclesQuery(
    DateTime StartDateTimeUtc,
    DateTime EndDateTimeUtc,
    double Latitude,
    double Longitude,
    double RadiusKilometers)
    : IRequest<IReadOnlyList<VehicleViewModel>>;