using CarShare.Domain.Enums;

namespace CarShare.Application.Features.Reservations.Commands.CreateReservation;

public sealed record CreateReservationResult(
    Guid Id,
    ReservationStatus Status,
    DateTime HoldExpiresAtUtc,
    decimal RentalAmount,
    decimal DepositAmount);