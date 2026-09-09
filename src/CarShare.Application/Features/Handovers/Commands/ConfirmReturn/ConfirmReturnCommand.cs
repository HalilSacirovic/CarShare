using MediatR;

namespace CarShare.Application.Features.Handovers.Commands.ConfirmReturn;

public sealed record ConfirmReturnCommand(
    Guid ReservationId,
    int ReturnMileage,
    string ReturnPhotoUrl)
    : IRequest<ConfirmReturnResult>;