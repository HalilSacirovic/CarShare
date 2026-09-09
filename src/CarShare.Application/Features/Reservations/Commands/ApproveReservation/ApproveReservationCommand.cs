using MediatR;

namespace CarShare.Application.Features.Reservations.Commands.ApproveReservation;

public sealed record ApproveReservationCommand(
    Guid ReservationId) : IRequest<Unit>;