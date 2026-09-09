using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using MediatR;

namespace CarShare.Application.Features.Users.Queries.GetMyProfile;

public sealed class GetMyProfileQueryHandler
    : IRequestHandler<GetMyProfileQuery, UserProfileViewModel>
{
    private readonly IRepository<User> _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyProfileQueryHandler(
        IRepository<User> userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public Task<UserProfileViewModel> Handle(
        GetMyProfileQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var profile = _userRepository
            .Query()
            .Where(user => user.Id == currentUserId)
            .Select(user =>
                new UserProfileViewModel(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.AvatarUrl,
                    user.DriverLicenseUrl,
                    user.DriverVerificationStatus,
                    user.IsOwner,
                    user.IsRenter,
                    user.IsAdmin,
                    user.AverageRating,
                    user.TrustScore,
                    user.CreatedAtUtc))
            .FirstOrDefault();

        if (profile is null)
        {
            throw new KeyNotFoundException(
                "Korisnički profil nije pronađen.");
        }

        return Task.FromResult(profile);
    }
}