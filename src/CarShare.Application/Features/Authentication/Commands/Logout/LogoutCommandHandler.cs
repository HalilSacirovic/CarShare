using System.Security.Cryptography;
using System.Text;
using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using MediatR;

namespace CarShare.Application.Features.Authentication.Commands.Logout;

public sealed class LogoutCommandHandler
    : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(
        IRepository<RefreshToken> refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash = HashToken(request.RefreshToken);

        var refreshToken = _refreshTokenRepository
            .Query()
            .FirstOrDefault(token =>
                token.TokenHash == tokenHash);

        if (refreshToken is null ||
            refreshToken.RevokedAtUtc is not null)
        {
            return Unit.Value;
        }

        refreshToken.RevokedAtUtc = DateTime.UtcNow;
        refreshToken.RevocationReason = "Logout";

        _refreshTokenRepository.Update(refreshToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private static string HashToken(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }
}