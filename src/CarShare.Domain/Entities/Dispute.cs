using CarShare.Domain.Common;
using CarShare.Domain.Enums;

namespace CarShare.Domain.Entities;

public sealed class Dispute : BaseEntity
{
    public Guid ReservationId { get; set; }

    public Reservation Reservation { get; set; } = null!;

    public Guid OpenedByUserId { get; set; }

    public User OpenedByUser { get; set; } = null!;

    public string Reason { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DisputeStatus Status { get; set; }
        = DisputeStatus.Open;

    public Guid? ResolvedByAdminId { get; set; }

    public User? ResolvedByAdmin { get; set; }

    public string? AdminDecision { get; set; }

    public DateTime? ResolvedAtUtc { get; set; }
}