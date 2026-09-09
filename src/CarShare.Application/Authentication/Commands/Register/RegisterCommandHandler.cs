using System.Security.Cryptography;
using System.Text;
using CarShare.Application.Common.Authentication;
using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using MediatR;

namespace CarShare.Application.Features.Authentication.Commands.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, AuthResult>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public RegisterCommandHandler(
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
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var emailAlreadyExists = _userRepository
            .Query()
            .Any(user => user.Email == email);

        if (emailAlreadyExists)
        {
            throw new InvalidOperationException(
                "Korisnik sa ovim emailom već postoji.");
        }

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            IsRenter = true,
            IsOwner = request.IsOwner,
            IsAdmin = false
        };

        user.PasswordHash = _passwordService.Hash(
            user,
            request.Password);

        var tokens = _tokenService.CreateTokens(user);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = HashToken(tokens.RefreshToken),
            ExpiresAtUtc = tokens.RefreshTokenExpiresAtUtc
        };

        await _userRepository.AddAsync(user, cancellationToken);
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