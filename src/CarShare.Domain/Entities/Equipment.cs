using CarShare.Domain.Common;

namespace CarShare.Domain.Entities;

public sealed class Equipment : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Vehicle> Vehicles { get; set; }
        = new List<Vehicle>();
}