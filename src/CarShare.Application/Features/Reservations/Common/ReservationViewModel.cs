using CarShare.Domain.Enums;

namespace CarShare.Application.Features.Reservations.Common;

public sealed record ReservationViewModel(
    Guid Id,
    Guid VehicleId,
    string VehicleBrand,
    string VehicleModel,
    Guid RenterId,
    string RenterName,
    string RenterEmail,
    DateTime StartDateTimeUtc,
    DateTime EndDateTimeUtc,
    string TravelPurpose,
    ReservationStatus Status,
    DateTime HoldExpiresAtUtc,
    decimal RentalAmount,
    decimal DepositAmount);