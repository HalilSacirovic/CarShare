using CarShare.Application.Common.Authentication;
using MediatR;

namespace CarShare.Application.Features.Authentication.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResult>;