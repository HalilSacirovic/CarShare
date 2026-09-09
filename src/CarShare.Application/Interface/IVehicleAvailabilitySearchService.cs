namespace CarShare.Application.Common.Interfaces;

public interface IVehicleAvailabilitySearchService
{
    Task<IReadOnlyCollection<Guid>> FindAvailableVehicleIdsAsync(
        DateTime startDateTimeUtc,
        DateTime endDateTimeUtc,
        double latitude,
        double longitude,
        double radiusKilometers,
        CancellationToken cancellationToken);
}