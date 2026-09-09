using CarShare.Domain.Common;
using CarShare.Domain.Enums;

namespace CarShare.Domain.Entities;

public sealed class Reservation : BaseEntity
{
    public Guid VehicleId { get; set; }

    public Vehicle Vehicle { get; set; } = null!;

    public Guid RenterId { get; set; }

    public User Renter { get; set; } = null!;

    public DateTime StartDateTimeUtc { get; set; }

    public DateTime EndDateTimeUtc { get; set; }

    public string TravelPurpose { get; set; } = string.Empty;

    public ReservationStatus Status { get; set; }
        = ReservationStatus.Pending;

    public DateTime HoldExpiresAtUtc { get; set; }
        = DateTime.UtcNow.AddMinutes(30);

    public decimal RentalAmount { get; set; }

    public decimal DepositAmount { get; set; }

    public Payment? Payment { get; set; }

    public RentalHandover? Handover { get; set; }

    public ICollection<Review> Reviews { get; set; }
    = new List<Review>();

    public Dispute? Dispute { get; set; }
}