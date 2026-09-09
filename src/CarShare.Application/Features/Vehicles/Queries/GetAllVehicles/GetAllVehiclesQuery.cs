using CarShare.Application.Features.Vehicles.Common;
using MediatR;

namespace CarShare.Application.Features.Vehicles.Queries.GetAllVehicles;

public sealed record GetAllVehiclesQuery
    : IRequest<IReadOnlyList<VehicleViewModel>>;