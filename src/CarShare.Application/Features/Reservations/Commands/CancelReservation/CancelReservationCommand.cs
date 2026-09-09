using MediatR;

namespace CarShare.Application.Features.Reservations.Commands.CancelReservation;

public sealed record CancelReservationCommand(
    Guid ReservationId) : IRequest<Unit>;