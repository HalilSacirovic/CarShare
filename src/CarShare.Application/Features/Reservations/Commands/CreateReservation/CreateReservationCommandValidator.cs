using FluentValidation;

namespace CarShare.Application.Features.Reservations.Commands.CreateReservation;

public sealed class CreateReservationCommandValidator
    : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty();

        RuleFor(x => x.StartDateTimeUtc)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage(
                "Početak rezervacije mora biti u budućnosti.");

        RuleFor(x => x.EndDateTimeUtc)
            .GreaterThan(x => x.StartDateTimeUtc)
            .WithMessage(
                "Kraj rezervacije mora biti posle početka.");

        RuleFor(x => x.TravelPurpose)
            .NotEmpty()
            .MaximumLength(300);
    }
}
