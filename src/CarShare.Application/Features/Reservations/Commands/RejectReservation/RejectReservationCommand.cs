using MediatR;

namespace CarShare.Application.Features.Reservations.Commands.RejectReservation;

public sealed record RejectReservationCommand(
    Guid ReservationId) : IRequest<Unit>;