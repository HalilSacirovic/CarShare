using FluentValidation;

namespace CarShare.Application.Features.Disputes.Commands.CreateDispute;

public sealed class CreateDisputeCommandValidator
    : AbstractValidator<CreateDisputeCommand>
{
    public CreateDisputeCommandValidator()
    {
        RuleFor(command => command.ReservationId)
            .NotEmpty()
            .WithMessage("ID rezervacije je obavezan.");

        RuleFor(command => command.Reason)
            .NotEmpty()
            .WithMessage("Razlog spora je obavezan.")
            .MaximumLength(100)
            .WithMessage("Razlog može imati najviše 100 karaktera.");

        RuleFor(command => command.Description)
            .NotEmpty()
            .WithMessage("Opis spora je obavezan.")
            .MaximumLength(1000)
            .WithMessage("Opis može imati najviše 1000 karaktera.");
    }
}