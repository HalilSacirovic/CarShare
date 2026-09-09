using CarShare.Application.Features.Reservations.Common;
using MediatR;

namespace CarShare.Application.Features.Reservations.Queries.GetOwnerReservations;

public sealed record GetOwnerReservationsQuery
    : IRequest<IReadOnlyList<ReservationViewModel>>;