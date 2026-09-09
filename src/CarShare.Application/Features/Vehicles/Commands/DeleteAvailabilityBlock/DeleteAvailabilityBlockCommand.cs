using MediatR;

namespace CarShare.Application.Features.Vehicles.Commands.DeleteAvailabilityBlock;

public sealed record DeleteAvailabilityBlockCommand(
    Guid AvailabilityBlockId) : IRequest;