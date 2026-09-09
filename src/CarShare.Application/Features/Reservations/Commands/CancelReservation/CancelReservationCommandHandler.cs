using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Reservations.Commands.CancelReservation;

public sealed class CancelReservationCommandHandler
    : IRequestHandler<CancelReservationCommand, Unit>
{
    private readonly IRepository<Reservation>
        _reservationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CancelReservationCommandHandler(
        IRepository<Reservation> reservationRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        CancelReservationCommand request,
        CancellationToken cancellationToken)
    {
        var renterId = _currentUserService.UserId;

        var reservation = _reservationRepository
            .Query()
            .FirstOrDefault(reservation =>
                reservation.Id == request.ReservationId &&
                reservation.RenterId == renterId);

        if (reservation is null)
        {
            throw new KeyNotFoundException(
                "Rezervacija nije pronađena.");
        }

        if (reservation.Status != ReservationStatus.Pending &&
            reservation.Status != ReservationStatus.Approved)
        {
            throw new InvalidOperationException(
                "Ovu rezervaciju nije moguće otkazati.");
        }

        reservation.Status = ReservationStatus.Cancelled;
        reservation.UpdatedAtUtc = DateTime.UtcNow;

        _reservationRepository.Update(reservation);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}