using FluentValidation;

namespace CarShare.Application.Features.Vehicles.Commands.SubmitVehicleVerification;

public sealed class SubmitVehicleVerificationCommandValidator
    : AbstractValidator<SubmitVehicleVerificationCommand>
{
    public SubmitVehicleVerificationCommandValidator()
    {
        RuleFor(command => command.VehicleId)
            .NotEmpty()
            .WithMessage("ID vozila je obavezan.");

        RuleFor(command => command.RegistrationDocumentUrl)
            .NotEmpty()
            .WithMessage("Dokument vozila je obavezan.")
            .Must(BeValidUrl)
            .WithMessage("URL dokumenta vozila nije ispravan.");
    }

    private static bool BeValidUrl(string url)
    {
        return Uri.TryCreate(
            url,
            UriKind.Absolute,
            out var result)
            && (result.Scheme == Uri.UriSchemeHttp ||
                result.Scheme == Uri.UriSchemeHttps);
    }
}