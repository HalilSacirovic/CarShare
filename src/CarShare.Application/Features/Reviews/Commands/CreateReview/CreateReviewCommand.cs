using MediatR;

namespace CarShare.Application.Features.Reviews.Commands.CreateReview;

public sealed record CreateReviewCommand(
    Guid ReservationId,
    int Rating,
    string? Comment) : IRequest<CreateReviewResult>;