namespace CarShare.Application.Features.Reservations.Common;

public sealed record RentalContractData(
    Guid ReservationId,
    string OwnerName,
    string OwnerEmail,
    string RenterName,
    string RenterEmail,
    string VehicleBrand,
    string VehicleModel,
    DateTime StartDateTimeUtc,
    DateTime EndDateTimeUtc,
    decimal RentalAmount,
    decimal DepositAmount);