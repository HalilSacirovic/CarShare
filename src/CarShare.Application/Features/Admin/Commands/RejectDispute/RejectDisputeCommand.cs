using MediatR;

namespace CarShare.Application.Features.Admin.Commands.RejectDispute;

public sealed record RejectDisputeCommand(
    Guid DisputeId,
    string AdminDecision) : IRequest;