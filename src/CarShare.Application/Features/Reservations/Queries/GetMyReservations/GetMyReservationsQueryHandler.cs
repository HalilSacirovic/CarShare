using CarShare.Application.Common.Interfaces;
using CarShare.Application.Features.Reservations.Common;
using CarShare.Domain.Entities;
using MediatR;

namespace CarShare.Application.Features.Reservations.Queries.GetMyReservations;

public sealed class GetMyReservationsQueryHandler
    : IRequestHandler<
        GetMyReservationsQuery,
        IReadOnlyList<ReservationViewModel>>
{
    private readonly IRepository<Reservation>
        _reservationRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyReservationsQueryHandler(
        IRepository<Reservation> reservationRepository,
        ICurrentUserService currentUserService)
    {
        _reservationRepository = reservationRepository;
        _currentUserService = currentUserService;
    }

    public Task<IReadOnlyList<ReservationViewModel>> Handle(
        GetMyReservationsQuery request,
        CancellationToken cancellationToken)
    {
        var renterId = _currentUserService.UserId;

        var reservations = _reservationRepository
            .Query()
            .Where(reservation =>
                reservation.RenterId == renterId)
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
        reservation.DepositAmount))
            .ToList();

        return Task.FromResult<
            IReadOnlyList<ReservationViewModel>>(
                reservations);
    }
}