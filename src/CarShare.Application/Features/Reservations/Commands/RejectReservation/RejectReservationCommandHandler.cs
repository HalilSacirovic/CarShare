using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Reservations.Commands.RejectReservation;

public sealed class RejectReservationCommandHandler
    : IRequestHandler<RejectReservationCommand, Unit>
{
    private readonly IRepository<Reservation>
        _reservationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RejectReservationCommandHandler(
        IRepository<Reservation> reservationRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        RejectReservationCommand request,
        CancellationToken cancellationToken)
    {
        var ownerId = _currentUserService.UserId;

        var reservation = _reservationRepository
            .Query()
            .FirstOrDefault(reservation =>
                reservation.Id == request.ReservationId &&
                reservation.Vehicle.OwnerId == ownerId);

        if (reservation is null)
        {
            throw new KeyNotFoundException(
                "Rezervacija nije pronađena.");
        }

        if (reservation.Status != ReservationStatus.Pending)
        {
            throw new InvalidOperationException(
                "Samo rezervacija na čekanju može biti odbijena.");
        }

        if (reservation.HoldExpiresAtUtc <= DateTime.UtcNow)
        {
            reservation.Status = ReservationStatus.Cancelled;
            _reservationRepository.Update(reservation);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            throw new InvalidOperationException(
                "Hold rezervacije je istekao.");
        }

        reservation.Status = ReservationStatus.Rejected;
        reservation.UpdatedAtUtc = DateTime.UtcNow;

        _reservationRepository.Update(reservation);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}