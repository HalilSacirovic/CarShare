using CarShare.Application.Features.Vehicles.Common;
using MediatR;

namespace CarShare.Application.Features.Vehicles.Queries.GetVehicleById;

public sealed record GetVehicleByIdQuery(
    Guid Id) : IRequest<VehicleViewModel>;