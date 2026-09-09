using System.Security.Cryptography;
using System.Text;
using CarShare.Application.Common.Authentication;
using CarShare.Application.Common.Interfaces;

using CarShare.Domain.Entities;
using MediatR;

namespace CarShare.Application.Features.Authentication.Commands.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IRepository<User> userRepository,
        IRepository<RefreshToken> refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<AuthResult> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = _userRepository
            .Query()
            .FirstOrDefault(user => user.Email == email);

        if (user is null ||
            !_passwordService.Verify(
                user,
                user.PasswordHash,
                request.Password))
        {
            throw new UnauthorizedAccessException(
                "Pogrešan email ili lozinka.");
        }

        var tokens = _tokenService.CreateTokens(user);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = HashToken(tokens.RefreshToken),
            ExpiresAtUtc = tokens.RefreshTokenExpiresAtUtc
        };

        await _refreshTokenRepository.AddAsync(
            refreshToken,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return tokens;
    }

    private static string HashToken(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }
}