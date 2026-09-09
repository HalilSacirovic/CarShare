using System.Security.Cryptography;
using System.Text;
using CarShare.Application.Common.Authentication;
using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using MediatR;

namespace CarShare.Application.Features.Authentication.Commands.Refresh;

public sealed class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Domain.Entities.RefreshToken>
        _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(
        IRepository<User> userRepository,
        IRepository<Domain.Entities.RefreshToken> refreshTokenRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<AuthResult> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var oldTokenHash = HashToken(request.RefreshToken);

        var oldToken = _refreshTokenRepository
            .Query()
            .FirstOrDefault(token =>
                token.TokenHash == oldTokenHash);

        if (oldToken is null ||
            oldToken.RevokedAtUtc is not null ||
            oldToken.ExpiresAtUtc <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException(
                "Refresh token nije važeći.");
        }

        var user = _userRepository
            .Query()
            .FirstOrDefault(user =>
                user.Id == oldToken.UserId);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "Korisnik nije pronađen.");
        }

        var newTokens = _tokenService.CreateTokens(user);
        var newTokenHash = HashToken(newTokens.RefreshToken);

        oldToken.RevokedAtUtc = DateTime.UtcNow;
        oldToken.ReplacedByTokenHash = newTokenHash;
        oldToken.RevocationReason = "Rotacija tokena";

        _refreshTokenRepository.Update(oldToken);

        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            TokenHash = newTokenHash,
            ExpiresAtUtc = newTokens.RefreshTokenExpiresAtUtc
        };

        await _refreshTokenRepository.AddAsync(
            newRefreshToken,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return newTokens;
    }

    private static string HashToken(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }
}