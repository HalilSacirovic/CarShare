namespace CarShare.API.Contracts.Reviews;

public sealed record CreateReviewRequest(
    int Rating,
    string? Comment);