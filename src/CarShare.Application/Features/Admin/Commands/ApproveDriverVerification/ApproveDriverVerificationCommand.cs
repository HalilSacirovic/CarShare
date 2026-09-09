using MediatR;

namespace CarShare.Application.Features.Admin.Commands.ApproveDriverVerification;

public sealed record ApproveDriverVerificationCommand(
    Guid UserId) : IRequest;