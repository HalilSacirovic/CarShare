using CarShare.Domain.Enums;

namespace CarShare.Application.Features.Payments.Commands.CreatePayment;

public sealed record CreatePaymentResult(
    Guid PaymentId,
    Guid ReservationId,
    decimal RentalAmount,
    decimal DepositAmount,
    decimal TotalAmount,
    PaymentStatus Status,
    DepositStatus DepositStatus,
    string PaymentMethod,
    string CardLastFourDigits,
    DateTime PaidAtUtc);