using CarShare.Domain.Common;
using CarShare.Domain.Enums;

namespace CarShare.Domain.Entities;

public sealed class Payment : BaseEntity
{
    public Guid ReservationId { get; set; }

    public Reservation Reservation { get; set; } = null!;

    public decimal RentalAmount { get; set; }

    public decimal DepositAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public PaymentStatus Status { get; set; }
        = PaymentStatus.Pending;

    public DepositStatus DepositStatus { get; set; }
        = DepositStatus.Pending;

    public string PaymentMethod { get; set; } = "MockCard";

    public string? CardLastFourDigits { get; set; }

    public DateTime? PaidAtUtc { get; set; }
}