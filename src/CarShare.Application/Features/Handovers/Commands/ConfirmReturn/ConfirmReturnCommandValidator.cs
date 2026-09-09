using FluentValidation;

namespace CarShare.Application.Features.Handovers.Commands.ConfirmReturn;

public sealed class ConfirmReturnCommandValidator
    : AbstractValidator<ConfirmReturnCommand>
{
    public ConfirmReturnCommandValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty();

        RuleFor(x => x.ReturnMileage)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ReturnPhotoUrl)
            .NotEmpty()
            .MaximumLength(500)
            .Must(url =>
                Uri.TryCreate(
                    url,
                    UriKind.Absolute,
                    out _))
            .WithMessage(
                "URL fotografije nije ispravan.");
    }
}