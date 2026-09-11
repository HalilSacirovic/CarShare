using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Vehicles.Queries.GetVehicleCalendar;

public sealed class GetVehicleCalendarQueryHandler
    : IRequestHandler<
        GetVehicleCalendarQuery,
        IReadOnlyList<VehicleUnavailablePeriodViewModel>>
{
    private readonly IRepository<Vehicle>
        _vehicleRepository;

    private readonly IRepository<AvailabilityBlock>
        _availabilityRepository;

    private readonly IRepository<Reservation>
        _reservationRepository;

    public GetVehicleCalendarQueryHandler(
        IRepository<Vehicle> vehicleRepository,
        IRepository<AvailabilityBlock>
            availabilityRepository,
        IRepository<Reservation> reservationRepository)
    {
        _vehicleRepository = vehicleRepository;
        _availabilityRepository = availabilityRepository;
        _reservationRepository = reservationRepository;
    }

    public Task<
        IReadOnlyList<VehicleUnavailablePeriodViewModel>>
        Handle(
            GetVehicleCalendarQuery request,
            CancellationToken cancellationToken)
    {
        var vehicleExists = _vehicleRepository
            .Query()
            .Any(vehicle =>
                vehicle.Id == request.VehicleId);

        if (!vehicleExists)
        {
            throw new KeyNotFoundException(
                "Vozilo nije pronađeno.");
        }

        var availabilityBlocks = _availabilityRepository
            .Query()
            .Where(block =>
                block.VehicleId == request.VehicleId)
            .Select(block =>
                new VehicleUnavailablePeriodViewModel(
                    block.Id,
                    block.StartDateTimeUtc,
                    block.EndDateTimeUtc,
                    "Blocked"))
            .ToList();

        var reservations = _reservationRepository
            .Query()
            .Where(reservation =>
                reservation.VehicleId == request.VehicleId &&
                (
                    reservation.Status ==
                        ReservationStatus.Approved ||
                    reservation.Status ==
                        ReservationStatus.Active
                ))
            .Select(reservation =>
                new VehicleUnavailablePeriodViewModel(
                    reservation.Id,
                    reservation.StartDateTimeUtc,
                    reservation.EndDateTimeUtc,
                    "Reserved"))
            .ToList();

        var unavailablePeriods = availabilityBlocks
            .Concat(reservations)
            .OrderBy(period => period.StartDateTimeUtc)
            .ToList();

        return Task.FromResult<
            IReadOnlyList<
                VehicleUnavailablePeriodViewModel>>(
                    unavailablePeriods);
    }
}