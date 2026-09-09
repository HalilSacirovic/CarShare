using MediatR;

namespace CarShare.Application.Features.Vehicles.Commands.DeleteVehicle;

public sealed record DeleteVehicleCommand(
    Guid VehicleId) : IRequest;