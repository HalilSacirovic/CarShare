namespace CarShare.Application.Features.Vehicles.Queries.GetVehicleCalendar;

public sealed record VehicleUnavailablePeriodViewModel(
    Guid Id,
    DateTime StartDateTimeUtc,
    DateTime EndDateTimeUtc,
    string Type);