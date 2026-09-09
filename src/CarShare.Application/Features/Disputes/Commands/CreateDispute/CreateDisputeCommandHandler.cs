using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Disputes.Commands.CreateDispute;

public sealed class CreateDisputeCommandHandler
    : IRequestHandler<CreateDisputeCommand, Guid>
{
    private readonly IRepository<Reservation> _reservationRepository;
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IRepository<Dispute> _disputeRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDisputeCommandHandler(
        IRepository<Reservation> reservationRepository,
        IRepository<Vehicle> vehicleRepository,
        IRepository<Dispute> disputeRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _vehicleRepository = vehicleRepository;
        _disputeRepository = disputeRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateDisputeCommand request,
        CancellationToken cancellationToken)
    {
        var reservation = _reservationRepository
            .Query()
            .FirstOrDefault(reservation =>
                reservation.Id == request.ReservationId)
            ?? throw new KeyNotFoundException(
                "Rezervacija nije pronađena.");

        var vehicle = _vehicleRepository
            .Query()
            .FirstOrDefault(vehicle =>
                vehicle.Id == reservation.VehicleId)
            ?? throw new KeyNotFoundException(
                "Vozilo nije pronađeno.");

        var currentUserId = _currentUserService.UserId;

        var userParticipatesInReservation =
            reservation.RenterId == currentUserId ||
            vehicle.OwnerId == currentUserId;

        if (!userParticipatesInReservation)
        {
            throw new UnauthorizedAccessException(
                "Spor mogu otvoriti samo renter ili vlasnik vozila.");
        }

        var allowedStatus =
            reservation.Status == ReservationStatus.Approved ||
            reservation.Status == ReservationStatus.Active ||
            reservation.Status == ReservationStatus.Completed;

        if (!allowedStatus)
        {
            throw new InvalidOperationException(
                "Za ovu rezervaciju nije moguće otvoriti spor.");
        }

        var disputeAlreadyExists = _disputeRepository
            .Query()
            .Any(dispute =>
                dispute.ReservationId == request.ReservationId);

        if (disputeAlreadyExists)
        {
            throw new InvalidOperationException(
                "Za ovu rezervaciju već postoji spor.");
        }

        var dispute = new Dispute
        {
            ReservationId = request.ReservationId,
            OpenedByUserId = currentUserId,
            Reason = request.Reason.Trim(),
            Description = request.Description.Trim(),
            Status = DisputeStatus.Open
        };

        reservation.Status = ReservationStatus.Disputed;

        await _disputeRepository.AddAsync(
            dispute,
            cancellationToken);

        _reservationRepository.Update(reservation);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return dispute.Id;
    }
}