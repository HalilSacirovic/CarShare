using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Handovers.Commands.ConfirmReturn;

public sealed class ConfirmReturnCommandHandler
    : IRequestHandler<
        ConfirmReturnCommand,
        ConfirmReturnResult>
{
    private readonly IRepository<Reservation>
        _reservationRepository;

    private readonly IRepository<Vehicle>
        _vehicleRepository;

    private readonly IRepository<Payment>
        _paymentRepository;

    private readonly IRepository<RentalHandover>
        _handoverRepository;

    private readonly ICurrentUserService
        _currentUserService;

    private readonly IUnitOfWork _unitOfWork;

    public ConfirmReturnCommandHandler(
        IRepository<Reservation> reservationRepository,
        IRepository<Vehicle> vehicleRepository,
        IRepository<Payment> paymentRepository,
        IRepository<RentalHandover> handoverRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _vehicleRepository = vehicleRepository;
        _paymentRepository = paymentRepository;
        _handoverRepository = handoverRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ConfirmReturnResult> Handle(
        ConfirmReturnCommand request,
        CancellationToken cancellationToken)
    {
        var reservation = _reservationRepository
            .Query()
            .FirstOrDefault(reservation =>
                reservation.Id == request.ReservationId);

        if (reservation is null)
        {
            throw new KeyNotFoundException(
                "Rezervacija nije pronađena.");
        }

        if (reservation.Status !=
            ReservationStatus.Active)
        {
            throw new InvalidOperationException(
                "Vraćanje je moguće samo za aktivnu rezervaciju.");
        }

        var vehicle = _vehicleRepository
            .Query()
            .FirstOrDefault(vehicle =>
                vehicle.Id == reservation.VehicleId);

        if (vehicle is null)
        {
            throw new KeyNotFoundException(
                "Vozilo nije pronađeno.");
        }

        var payment = _paymentRepository
            .Query()
            .FirstOrDefault(payment =>
                payment.ReservationId ==
                    reservation.Id);

        if (payment is null)
        {
            throw new KeyNotFoundException(
                "Plaćanje nije pronađeno.");
        }

        var handover = _handoverRepository
            .Query()
            .FirstOrDefault(handover =>
                handover.ReservationId ==
                    reservation.Id);

        if (handover is null ||
            !handover.PickupConfirmedByOwner ||
            !handover.PickupConfirmedByRenter)
        {
            throw new InvalidOperationException(
                "Preuzimanje vozila nije potpuno potvrđeno.");
        }

        if (handover.PickupMileage.HasValue &&
            request.ReturnMileage <
                handover.PickupMileage.Value)
        {
            throw new InvalidOperationException(
                "Kilometraža pri vraćanju ne može biti manja od početne.");
        }

        var currentUserId =
            _currentUserService.UserId;

        var isOwner =
            vehicle.OwnerId == currentUserId;

        var isRenter =
            reservation.RenterId == currentUserId;

        if (!isOwner && !isRenter)
        {
            throw new UnauthorizedAccessException(
                "Nemate pravo da potvrdite vraćanje.");
        }

        handover.ReturnMileage =
            request.ReturnMileage;

        handover.ReturnPhotoUrl =
            request.ReturnPhotoUrl.Trim();

        if (isOwner)
        {
            handover.ReturnConfirmedByOwner = true;
        }

        if (isRenter)
        {
            handover.ReturnConfirmedByRenter = true;
        }

        if (handover.ReturnConfirmedByOwner &&
            handover.ReturnConfirmedByRenter)
        {
            var now = DateTime.UtcNow;

            handover.ReturnConfirmedAtUtc = now;

            reservation.Status =
                ReservationStatus.Completed;

            reservation.UpdatedAtUtc = now;

            vehicle.Status =
                VehicleStatus.Available;

            vehicle.UpdatedAtUtc = now;

            _reservationRepository.Update(
                reservation);

            _vehicleRepository.Update(vehicle);
        }

        _handoverRepository.Update(handover);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new ConfirmReturnResult(
            handover.Id,
            handover.ReturnConfirmedByOwner,
            handover.ReturnConfirmedByRenter,
            handover.ReturnConfirmedAtUtc,
            reservation.Status,
            vehicle.Status,
            payment.DepositStatus);
    }
}