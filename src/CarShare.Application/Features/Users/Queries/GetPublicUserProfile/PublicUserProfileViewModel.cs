namespace CarShare.Application.Features.Users.Queries.GetPublicUserProfile;

public sealed record PublicUserProfileViewModel(
    Guid UserId,
    string FullName,
    bool IsDriverVerified,
    decimal AverageRating,
    decimal TrustScore,
    int CompletedReservations,
    IReadOnlyList<PublicReviewViewModel> Reviews);

public sealed record PublicReviewViewModel(
    Guid ReviewId,
    string ReviewerName,
    int Rating,
    string? Comment,
    DateTime CreatedAtUtc);