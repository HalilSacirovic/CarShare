using FluentValidation;

namespace CarShare.Application.Features.Admin.Commands.ResolveDispute;

public sealed class ResolveDisputeCommandValidator
    : AbstractValidator<ResolveDisputeCommand>
{
    public ResolveDisputeCommandValidator()
    {
        RuleFor(command => command.DisputeId)
            .NotEmpty()
            .WithMessage("ID spora je obavezan.");

        RuleFor(command => command.AdminDecision)
            .NotEmpty()
            .WithMessage("Odluka administratora je obavezna.")
            .MaximumLength(1000)
            .WithMessage(
                "Odluka može imati najviše 1000 karaktera.");
    }
}