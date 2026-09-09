using FluentValidation;

namespace CarShare.Application.Features.Handovers.Commands.ConfirmPickup;

public sealed class ConfirmPickupCommandValidator
    : AbstractValidator<ConfirmPickupCommand>
{
    public ConfirmPickupCommandValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty();

        RuleFor(x => x.PickupMileage)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.PickupPhotoUrl)
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