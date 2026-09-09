using FluentValidation;

namespace CarShare.Application.Features.Vehicles.Commands.CreateAvailabilityBlock;

public sealed class CreateAvailabilityBlockCommandValidator
    : AbstractValidator<CreateAvailabilityBlockCommand>
{
    public CreateAvailabilityBlockCommandValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty();

        RuleFor(x => x.StartDateTimeUtc)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage(
                "Početak perioda mora biti u budućnosti.");

        RuleFor(x => x.EndDateTimeUtc)
            .GreaterThan(x => x.StartDateTimeUtc)
            .WithMessage(
                "Kraj perioda mora biti posle početka.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(200);
    }
}