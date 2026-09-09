namespace CarShare.API.Contracts.Reservations;

public sealed record CreateReservationRequest(
    Guid VehicleId,
    DateTime StartDateTimeUtc,
    DateTime EndDateTimeUtc,
    string TravelPurpose);