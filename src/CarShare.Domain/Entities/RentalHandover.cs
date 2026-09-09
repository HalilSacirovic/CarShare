using CarShare.Domain.Common;

namespace CarShare.Domain.Entities;

public sealed class RentalHandover : BaseEntity
{
    public Guid ReservationId { get; set; }

    public Reservation Reservation { get; set; } = null!;

    public int? PickupMileage { get; set; }

    public string? PickupPhotoUrl { get; set; }

    public bool PickupConfirmedByOwner { get; set; }

    public bool PickupConfirmedByRenter { get; set; }

    public DateTime? PickupConfirmedAtUtc { get; set; }

    public int? ReturnMileage { get; set; }

    public string? ReturnPhotoUrl { get; set; }

    public bool ReturnConfirmedByOwner { get; set; }

    public bool ReturnConfirmedByRenter { get; set; }

    public DateTime? ReturnConfirmedAtUtc { get; set; }
}