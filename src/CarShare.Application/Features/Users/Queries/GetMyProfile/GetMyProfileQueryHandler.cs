using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using MediatR;

namespace CarShare.Application.Features.Users.Queries.GetMyProfile;

public sealed class GetMyProfileQueryHandler
    : IRequestHandler<
        GetMyProfileQuery,
        UserProfileViewModel>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Review> _reviewRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyProfileQueryHandler(
        IRepository<User> userRepository,
        IRepository<Review> reviewRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _reviewRepository = reviewRepository;
        _currentUserService = currentUserService;
    }

    public Task<UserProfileViewModel> Handle(
        GetMyProfileQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var user = _userRepository
            .Query()
            .FirstOrDefault(user =>
                user.Id == currentUserId);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "Korisnički profil nije pronađen.");
        }

        var receivedReviews =
            (
                from review in _reviewRepository.Query()
                join reviewer in _userRepository.Query()
                    on review.ReviewerId equals reviewer.Id
                where review.ReviewedUserId ==
                    currentUserId
                orderby review.CreatedAtUtc descending
                select new ReceivedReviewViewModel(
                    review.Id,
                    reviewer.FirstName +
                        " " +
                        reviewer.LastName,
                    review.Rating,
                    review.Comment,
                    review.CreatedAtUtc)
            )
            .ToList();

        var profile = new UserProfileViewModel(
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
            user.CreatedAtUtc,
            receivedReviews);

        return Task.FromResult(profile);
    }
}