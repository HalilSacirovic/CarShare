using FluentValidation;

namespace CarShare.Application.Features.Vehicles.Commands.UpdateVehicle;

public sealed class UpdateVehicleCommandValidator
    : AbstractValidator<UpdateVehicleCommand>
{
    public UpdateVehicleCommandValidator()
    {
        RuleFor(command => command.VehicleId)
            .NotEmpty();

        RuleFor(command => command.Brand)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(command => command.Model)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(command => command.Year)
            .InclusiveBetween(
                1950,
                DateTime.UtcNow.Year + 1);

        RuleFor(command => command.Color)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(command => command.Mileage)
            .GreaterThanOrEqualTo(0);

        RuleFor(command => command.SeatCount)
            .InclusiveBetween(1, 20);

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

        RuleFor(command => command.DepositAmount)
            .GreaterThanOrEqualTo(0);

        RuleFor(command => command.DailyMileageLimit)
            .GreaterThan(0);

        RuleFor(command => command)
            .Must(command =>
                command.PricePerHour.HasValue ||
                command.PricePerDay.HasValue)
            .WithMessage(
                "Cena po satu ili cena po danu je obavezna.");

        RuleFor(command => command.PricePerHour)
            .GreaterThan(0)
            .When(command =>
                command.PricePerHour.HasValue);

        RuleFor(command => command.PricePerDay)
            .GreaterThan(0)
            .When(command =>
                command.PricePerDay.HasValue);
    }
}