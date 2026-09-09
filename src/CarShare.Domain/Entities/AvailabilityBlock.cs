using CarShare.Domain.Common;

namespace CarShare.Domain.Entities;

public sealed class AvailabilityBlock : BaseEntity
{
    public Guid VehicleId { get; set; }

    public Vehicle Vehicle { get; set; } = null!;

    public DateTime StartDateTimeUtc { get; set; }

    public DateTime EndDateTimeUtc { get; set; }

    public string Reason { get; set; } = string.Empty;
}