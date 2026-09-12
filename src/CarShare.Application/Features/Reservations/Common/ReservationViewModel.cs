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
    decimal DepositAmount)
{
    public int? PickupMileage { get; init; }

    public string? PickupPhotoUrl { get; init; }

    public bool PickupConfirmedByOwner { get; init; }

    public bool PickupConfirmedByRenter { get; init; }

    public int? ReturnMileage { get; init; }

    public string? ReturnPhotoUrl { get; init; }

    public bool ReturnConfirmedByOwner { get; init; }

    public bool ReturnConfirmedByRenter { get; init; }
}