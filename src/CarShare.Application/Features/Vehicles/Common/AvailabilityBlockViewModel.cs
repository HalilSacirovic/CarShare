namespace CarShare.Application.Features.Vehicles.Common;

public sealed record AvailabilityBlockViewModel(
    Guid Id,
    Guid VehicleId,
    DateTime StartDateTimeUtc,
    DateTime EndDateTimeUtc,
    string Reason);