using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Handovers.Commands.ConfirmPickup;

public sealed class ConfirmPickupCommandHandler
    : IRequestHandler<ConfirmPickupCommand, ConfirmPickupResult>
{
    private readonly IRepository<Reservation>
        _reservationRepository;
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IRepository<RentalHandover>
        _handoverRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmPickupCommandHandler(
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

    public async Task<ConfirmPickupResult> Handle(
        ConfirmPickupCommand request,
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

        if (reservation.Status != ReservationStatus.Approved)
        {
            throw new InvalidOperationException(
                "Preuzimanje se može potvrditi samo za odobrenu rezervaciju.");
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
                payment.ReservationId == reservation.Id);

        if (payment is null ||
            payment.Status != PaymentStatus.Paid)
        {
            throw new InvalidOperationException(
                "Rezervacija mora biti plaćena pre preuzimanja.");
        }

        var currentUserId = _currentUserService.UserId;
        var isOwner = vehicle.OwnerId == currentUserId;
        var isRenter = reservation.RenterId == currentUserId;

        if (!isOwner && !isRenter)
        {
            throw new UnauthorizedAccessException(
                "Nemate pravo da potvrdite preuzimanje.");
        }

        var handover = _handoverRepository
            .Query()
            .FirstOrDefault(handover =>
                handover.ReservationId == reservation.Id);

        var isNewHandover = handover is null;

        if (handover is null)
        {
            handover = new RentalHandover
            {
                ReservationId = reservation.Id
            };
        }

        handover.PickupMileage = request.PickupMileage;
        handover.PickupPhotoUrl = request.PickupPhotoUrl;

        if (isOwner)
        {
            handover.PickupConfirmedByOwner = true;
        }

        if (isRenter)
        {
            handover.PickupConfirmedByRenter = true;
        }

        if (handover.PickupConfirmedByOwner &&
            handover.PickupConfirmedByRenter)
        {
            handover.PickupConfirmedAtUtc = DateTime.UtcNow;

            reservation.Status = ReservationStatus.Active;
            reservation.UpdatedAtUtc = DateTime.UtcNow;

            vehicle.Status = VehicleStatus.InRent;
            vehicle.UpdatedAtUtc = DateTime.UtcNow;

            _reservationRepository.Update(reservation);
            _vehicleRepository.Update(vehicle);
        }

        if (isNewHandover)
        {
            await _handoverRepository.AddAsync(
                handover,
                cancellationToken);
        }
        else
        {
            _handoverRepository.Update(handover);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ConfirmPickupResult(
            handover.Id,
            handover.PickupConfirmedByOwner,
            handover.PickupConfirmedByRenter,
            handover.PickupConfirmedAtUtc,
            reservation.Status,
            vehicle.Status);
    }
}