using CarShare.Application.Features.Reservations.Common;
using MediatR;

namespace CarShare.Application.Features.Reservations.Queries.GetMyReservations;

public sealed record GetMyReservationsQuery
    : IRequest<IReadOnlyList<ReservationViewModel>>;