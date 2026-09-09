using MediatR;

namespace CarShare.Application.Features.Authentication.Commands.Logout;

public sealed record LogoutCommand(
    string RefreshToken) : IRequest<Unit>;