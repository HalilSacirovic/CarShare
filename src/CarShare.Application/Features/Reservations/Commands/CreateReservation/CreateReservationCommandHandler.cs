using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Reservations.Commands.CreateReservation;

public sealed class CreateReservationCommandHandler
    : IRequestHandler<
        CreateReservationCommand,
        CreateReservationResult>
{
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IRepository<Reservation> _reservationRepository;
    private readonly IRepository<AvailabilityBlock>
        _availabilityRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReservationCommandHandler(
        IRepository<Vehicle> vehicleRepository,
        IRepository<Reservation> reservationRepository,
        IRepository<AvailabilityBlock> availabilityRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _reservationRepository = reservationRepository;
        _availabilityRepository = availabilityRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateReservationResult> Handle(
        CreateReservationCommand request,
        CancellationToken cancellationToken)
    {
        var vehicle = _vehicleRepository
            .Query()
            .FirstOrDefault(vehicle =>
                vehicle.Id == request.VehicleId);

        if (vehicle is null)
        {
            throw new KeyNotFoundException(
                "Vozilo nije pronađeno.");
        }

        if (vehicle.OwnerId == _currentUserService.UserId)
        {
            throw new InvalidOperationException(
                "Vlasnik ne može rezervisati svoje vozilo.");
        }

        if (vehicle.Status != VehicleStatus.Available)
        {
            throw new InvalidOperationException(
                "Vozilo trenutno nije dostupno.");
        }

        var hasAvailabilityBlock = _availabilityRepository
            .Query()
            .Any(block =>
                block.VehicleId == request.VehicleId &&
                block.StartDateTimeUtc < request.EndDateTimeUtc &&
                block.EndDateTimeUtc > request.StartDateTimeUtc);

        if (hasAvailabilityBlock)
        {
            throw new InvalidOperationException(
                "Vozilo nije dostupno u izabranom periodu.");
        }

        var now = DateTime.UtcNow;

        var hasReservationOverlap = _reservationRepository
            .Query()
            .Any(reservation =>
                reservation.VehicleId == request.VehicleId &&
                reservation.StartDateTimeUtc <
                    request.EndDateTimeUtc &&
                reservation.EndDateTimeUtc >
                    request.StartDateTimeUtc &&
                (
                    reservation.Status ==
                        ReservationStatus.Approved ||
                    reservation.Status ==
                        ReservationStatus.Active ||
                    (
                        reservation.Status ==
                            ReservationStatus.Pending &&
                        reservation.HoldExpiresAtUtc > now
                    )
                ));

        if (hasReservationOverlap)
        {
            throw new InvalidOperationException(
                "Vozilo je već rezervisano u izabranom periodu.");
        }

        var rentalAmount = CalculateRentalAmount(
            vehicle,
            request.StartDateTimeUtc,
            request.EndDateTimeUtc);

        var reservation = new Reservation
        {
            VehicleId = request.VehicleId,
            RenterId = _currentUserService.UserId,
            StartDateTimeUtc = request.StartDateTimeUtc,
            EndDateTimeUtc = request.EndDateTimeUtc,
            TravelPurpose = request.TravelPurpose.Trim(),
            Status = vehicle.AutoApproveReservations
                ? ReservationStatus.Approved
                : ReservationStatus.Pending,
            HoldExpiresAtUtc = now.AddMinutes(30),
            RentalAmount = rentalAmount,
            DepositAmount = vehicle.DepositAmount
        };

        await _reservationRepository.AddAsync(
            reservation,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateReservationResult(
            reservation.Id,
            reservation.Status,
            reservation.HoldExpiresAtUtc,
            reservation.RentalAmount,
            reservation.DepositAmount);
    }

    private static decimal CalculateRentalAmount(
        Vehicle vehicle,
        DateTime start,
        DateTime end)
    {
        var duration = end - start;

        if (duration.TotalHours >= 24 &&
            vehicle.PricePerDay.HasValue)
        {
            var days = (decimal)Math.Ceiling(
                duration.TotalDays);

            return days * vehicle.PricePerDay.Value;
        }

        if (vehicle.PricePerHour.HasValue)
        {
            var hours = (decimal)Math.Ceiling(
                duration.TotalHours);

            return hours * vehicle.PricePerHour.Value;
        }

        if (vehicle.PricePerDay.HasValue)
        {
            return vehicle.PricePerDay.Value;
        }

        throw new InvalidOperationException(
            "Vozilo nema definisanu cenu iznajmljivanja.");
    }
}