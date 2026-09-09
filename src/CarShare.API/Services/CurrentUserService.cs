using System.Security.Claims;
using CarShare.Application.Common.Interfaces;

namespace CarShare.API.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?
            .User.Identity?
            .IsAuthenticated == true;

    public Guid UserId
    {
        get
        {
            var userIdValue = _httpContextAccessor
                .HttpContext?
                .User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                throw new UnauthorizedAccessException(
                    "Korisnik nije prijavljen.");
            }

            return userId;
        }
    }
}