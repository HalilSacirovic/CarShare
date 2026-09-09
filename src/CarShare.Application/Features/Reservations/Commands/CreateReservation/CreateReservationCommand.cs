using MediatR;

namespace CarShare.Application.Features.Reservations.Commands.CreateReservation;

public sealed record CreateReservationCommand(
    Guid VehicleId,
    DateTime StartDateTimeUtc,
    DateTime EndDateTimeUtc,
    string TravelPurpose)
    : IRequest<CreateReservationResult>;