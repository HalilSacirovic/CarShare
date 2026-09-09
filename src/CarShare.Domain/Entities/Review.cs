using CarShare.Domain.Common;

namespace CarShare.Domain.Entities;

public sealed class Review : BaseEntity
{
    public Guid ReservationId { get; set; }

    public Reservation Reservation { get; set; } = null!;

    public Guid ReviewerId { get; set; }

    public User Reviewer { get; set; } = null!;

    public Guid ReviewedUserId { get; set; }

    public User ReviewedUser { get; set; } = null!;

    public int Rating { get; set; }

    public string? Comment { get; set; }
}