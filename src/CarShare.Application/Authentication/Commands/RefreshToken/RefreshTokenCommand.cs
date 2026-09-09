using CarShare.Application.Common.Authentication;
using MediatR;

namespace CarShare.Application.Features.Authentication.Commands.Refresh;

public sealed record RefreshTokenCommand(
    string RefreshToken) : IRequest<AuthResult>;