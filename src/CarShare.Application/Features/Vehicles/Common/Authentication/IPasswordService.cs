using CarShare.Domain.Entities;

namespace CarShare.Application.Common.Authentication;

public interface IPasswordService
{
    string Hash(User user, string password);

    bool Verify(User user, string passwordHash, string password);
}