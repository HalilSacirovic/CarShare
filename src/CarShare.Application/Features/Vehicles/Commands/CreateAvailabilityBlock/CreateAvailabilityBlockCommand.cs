using MediatR;

namespace CarShare.Application.Features.Vehicles.Commands.CreateAvailabilityBlock;

public sealed record CreateAvailabilityBlockCommand(
    Guid VehicleId,
    DateTime StartDateTimeUtc,
    DateTime EndDateTimeUtc,
    string Reason) : IRequest<Guid>;