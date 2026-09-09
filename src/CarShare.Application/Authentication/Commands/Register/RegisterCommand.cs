using CarShare.Application.Common.Authentication;
using MediatR;

namespace CarShare.Application.Features.Authentication.Commands.Register;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    bool IsOwner) : IRequest<AuthResult>;