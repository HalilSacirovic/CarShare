using MediatR;

namespace CarShare.Application.Features.Admin.Commands.RejectDriverVerification;

public sealed record RejectDriverVerificationCommand(
    Guid UserId) : IRequest;