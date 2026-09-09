using MediatR;

namespace CarShare.Application.Features.Handovers.Commands.ConfirmPickup;

public sealed record ConfirmPickupCommand(
    Guid ReservationId,
    int PickupMileage,
    string PickupPhotoUrl)
    : IRequest<ConfirmPickupResult>;