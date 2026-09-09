using CarShare.Application.Common.Authentication;
using CarShare.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CarShare.Infrastructure.Authentication;

public sealed class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string Hash(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool Verify(
        User user,
        string passwordHash,
        string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            user,
            passwordHash,
            password);

        return result != PasswordVerificationResult.Failed;
    }
}