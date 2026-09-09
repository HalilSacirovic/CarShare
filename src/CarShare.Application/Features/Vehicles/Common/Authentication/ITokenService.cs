using CarShare.Domain.Entities;

namespace CarShare.Application.Common.Authentication;

public interface ITokenService
{
    AuthResult CreateTokens(User user);
}