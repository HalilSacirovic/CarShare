using FluentValidation;

namespace CarShare.Application.Features.Vehicles.Commands.CreateVehicle;

public sealed class CreateVehicleCommandValidator
    : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(command => command.OwnerId)
            .NotEmpty();

        RuleFor(command => command.Brand)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(command => command.Model)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(command => command.Year)
            .InclusiveBetween(1990, DateTime.UtcNow.Year + 1);

        RuleFor(command => command.Color)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(command => command.Mileage)
            .GreaterThanOrEqualTo(0);

        RuleFor(command => command.SeatCount)
            .InclusiveBetween(1, 9);

        RuleFor(command => command.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(command => command.Address)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Latitude)
            .InclusiveBetween(-90, 90);

        RuleFor(command => command.Longitude)
            .InclusiveBetween(-180, 180);

        RuleFor(command => command.PricePerHour)
            .GreaterThan(0)
            .When(command => command.PricePerHour.HasValue);

        RuleFor(command => command.PricePerDay)
            .GreaterThan(0)
            .When(command => command.PricePerDay.HasValue);

        RuleFor(command => command)
            .Must(command =>
                command.PricePerHour.HasValue ||
                command.PricePerDay.HasValue)
            .WithMessage(
                "Mora biti uneta cena po satu ili cena po danu.");

        RuleFor(command => command.DepositAmount)
            .GreaterThanOrEqualTo(0);

        RuleFor(command => command.DailyMileageLimit)
            .GreaterThan(0);

        RuleFor(command => command.PhotoUrls)
            .NotNull()
            .Must(photos => photos.Count is >= 3 and <= 15)
            .WithMessage("Vozilo mora imati od 3 do 15 fotografija.");

        RuleForEach(command => command.PhotoUrls)
            .NotEmpty();

        RuleFor(command => command.EquipmentIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Ista oprema ne može biti izabrana više puta.");
    }
}