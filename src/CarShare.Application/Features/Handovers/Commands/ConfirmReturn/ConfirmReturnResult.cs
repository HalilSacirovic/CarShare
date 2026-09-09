using CarShare.Domain.Enums;

namespace CarShare.Application.Features.Handovers.Commands.ConfirmReturn;

public sealed record ConfirmReturnResult(
    Guid HandoverId,
    bool ReturnConfirmedByOwner,
    bool ReturnConfirmedByRenter,
    DateTime? ReturnConfirmedAtUtc,
    ReservationStatus ReservationStatus,
    VehicleStatus VehicleStatus,
    DepositStatus DepositStatus);