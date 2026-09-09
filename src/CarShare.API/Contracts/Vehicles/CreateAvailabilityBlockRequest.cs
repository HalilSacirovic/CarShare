namespace CarShare.API.Contracts.Vehicles;

public sealed record CreateAvailabilityBlockRequest(
    DateTime StartDateTimeUtc,
    DateTime EndDateTimeUtc,
    string Reason);