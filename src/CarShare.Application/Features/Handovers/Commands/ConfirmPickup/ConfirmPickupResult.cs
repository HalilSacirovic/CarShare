using CarShare.Domain.Enums;

namespace CarShare.Application.Features.Handovers.Commands.ConfirmPickup;

public sealed record ConfirmPickupResult(
    Guid HandoverId,
    bool PickupConfirmedByOwner,
    bool PickupConfirmedByRenter,
    DateTime? PickupConfirmedAtUtc,
    ReservationStatus ReservationStatus,
    VehicleStatus VehicleStatus);