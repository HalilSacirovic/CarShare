using MediatR;

namespace CarShare.Application.Features.Disputes.Commands.CreateDispute;

public sealed record CreateDisputeCommand(
    Guid ReservationId,
    string Reason,
    string Description) : IRequest<Guid>;