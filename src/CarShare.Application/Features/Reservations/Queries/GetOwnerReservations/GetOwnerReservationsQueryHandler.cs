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

    private readonly IRepository<RentalHandover>
        _handoverRepository;

    private readonly ICurrentUserService
        _currentUserService;

    public GetOwnerReservationsQueryHandler(
        IRepository<Reservation> reservationRepository,
        IRepository<RentalHandover> handoverRepository,
        ICurrentUserService currentUserService)
    {
        _reservationRepository = reservationRepository;
        _handoverRepository = handoverRepository;
        _currentUserService = currentUserService;
    }

    public Task<IReadOnlyList<ReservationViewModel>> Handle(
        GetOwnerReservationsQuery request,
        CancellationToken cancellationToken)
    {
        var ownerId = _currentUserService.UserId;

        var reservations =
            (
                from reservation
                    in _reservationRepository.Query()

                join handover
                    in _handoverRepository.Query()
                    on reservation.Id
                    equals handover.ReservationId
                    into handoverGroup

                from handover
                    in handoverGroup.DefaultIfEmpty()

                where reservation.Vehicle.OwnerId ==
                    ownerId

                orderby reservation.CreatedAtUtc descending

                select new ReservationViewModel(
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
                    reservation.DepositAmount)
                {
                    PickupMileage =
                        handover == null
                            ? null
                            : handover.PickupMileage,

                    PickupPhotoUrl =
                        handover == null
                            ? null
                            : handover.PickupPhotoUrl,

                    PickupConfirmedByOwner =
                        handover != null &&
                        handover.PickupConfirmedByOwner,

                    PickupConfirmedByRenter =
                        handover != null &&
                        handover.PickupConfirmedByRenter,

                    ReturnMileage =
                        handover == null
                            ? null
                            : handover.ReturnMileage,

                    ReturnPhotoUrl =
                        handover == null
                            ? null
                            : handover.ReturnPhotoUrl,

                    ReturnConfirmedByOwner =
                        handover != null &&
                        handover.ReturnConfirmedByOwner,

                    ReturnConfirmedByRenter =
                        handover != null &&
                        handover.ReturnConfirmedByRenter
                }
            )
            .ToList();

        return Task.FromResult<
            IReadOnlyList<ReservationViewModel>>(
                reservations);
    }
}