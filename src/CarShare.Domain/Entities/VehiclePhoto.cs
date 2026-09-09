using CarShare.Domain.Common;

namespace CarShare.Domain.Entities;

public sealed class VehiclePhoto : BaseEntity
{
    public Guid VehicleId { get; set; }

    public Vehicle Vehicle { get; set; } = null!;

    public string Url { get; set; } = string.Empty;

    public bool IsMain { get; set; }

    public int SortOrder { get; set; }
}