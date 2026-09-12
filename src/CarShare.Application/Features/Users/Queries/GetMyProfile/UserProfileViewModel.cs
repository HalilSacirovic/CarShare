using CarShare.Domain.Enums;

namespace CarShare.Application.Features.Users.Queries.GetMyProfile;

public sealed record UserProfileViewModel(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? AvatarUrl,
    string? DriverLicenseUrl,
    VerificationStatus DriverVerificationStatus,
    bool IsOwner,
    bool IsRenter,
    bool IsAdmin,
    decimal AverageRating,
    decimal TrustScore,
    DateTime RegisteredAtUtc,
    IReadOnlyList<ReceivedReviewViewModel> ReceivedReviews);

public sealed record ReceivedReviewViewModel(
    Guid ReviewId,
    string ReviewerName,
    int Rating,
    string? Comment,
    DateTime CreatedAtUtc);