namespace CarShare.Application.Features.Reviews.Commands.CreateReview;

public sealed record CreateReviewResult(
    Guid ReviewId,
    Guid ReservationId,
    Guid ReviewerId,
    Guid ReviewedUserId,
    int Rating,
    string? Comment);