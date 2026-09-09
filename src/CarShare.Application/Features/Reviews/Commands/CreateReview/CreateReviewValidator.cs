using FluentValidation;

namespace CarShare.Application.Features.Reviews.Commands.CreateReview;

public sealed class CreateReviewCommandValidator
    : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty();

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage(
                "Ocena mora biti između 1 i 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(500)
            .When(x =>
                !string.IsNullOrWhiteSpace(x.Comment));
    }
}