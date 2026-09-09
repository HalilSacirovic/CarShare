using CarShare.Application.Common.Interfaces;
using CarShare.Application.Features.Reservations.Common;
using CarShare.Domain.Entities;
using MediatR;

namespace CarShare.Application.Features.Reservations.Queries.GetOwnerReservations;

public sealed class GetOwnerReservationsQueryHandler
    : IRequestHandler<
        GetOwnerReservationsQuery,
        IReadOnlyList<ReservationViewModel>>
{
    private readonly IRepository<Reservation>
        _reservationRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetOwnerReservationsQueryHandler(
        IRepository<Reservation> reservationRepository,
        ICurrentUserService currentUserService)
    {
        _reservationRepository = reservationRepository;
        _currentUserService = currentUserService;
    }

    public Task<IReadOnlyList<ReservationViewModel>> Handle(
        GetOwnerReservationsQuery request,
        CancellationToken cancellationToken)
    {
        var ownerId = _currentUserService.UserId;

        var reservations = _reservationRepository
            .Query()
            .Where(reservation =>
                reservation.Vehicle.OwnerId == ownerId)
            .OrderByDescending(reservation =>
                reservation.CreatedAtUtc)
            .Select(reservation =>
    new ReservationViewModel(
        reservation.Id,
        reservation.VehicleId,
        reservation.Vehicle.Brand,
        reservation.Vehicle.Model,
        reservation.RenterId,
        reservation.Renter.FirstName +
            " " +
            reservation.Renter.LastName,
        reservation.Renter.Email,
        reservation.StartDateTimeUtc,
        reservation.EndDateTimeUtc,
        reservation.TravelPurpose,
        reservation.Status,
        reservation.HoldExpiresAtUtc,
        reservation.RentalAmount,
        reservation.DepositAmount)).ToList();

        return Task.FromResult<
            IReadOnlyList<ReservationViewModel>>(
                reservations);
    }
}