using FluentValidation;

namespace CarShare.Application.Features.Users.Commands.SubmitDriverLicense;

public sealed class SubmitDriverLicenseCommandValidator
    : AbstractValidator<SubmitDriverLicenseCommand>
{
    public SubmitDriverLicenseCommandValidator()
    {
        RuleFor(x => x.DriverLicenseUrl)
            .NotEmpty()
            .MaximumLength(500)
            .Must(url =>
                Uri.TryCreate(
                    url,
                    UriKind.Absolute,
                    out _))
            .WithMessage(
                "URL vozačke dozvole nije ispravan.");
    }
}