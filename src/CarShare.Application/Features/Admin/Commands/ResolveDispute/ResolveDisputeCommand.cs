using MediatR;

namespace CarShare.Application.Features.Admin.Commands.ResolveDispute;

public sealed record ResolveDisputeCommand(
    Guid DisputeId,
    string AdminDecision) : IRequest;