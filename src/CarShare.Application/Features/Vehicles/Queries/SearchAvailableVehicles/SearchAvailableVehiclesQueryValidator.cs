using FluentValidation;

namespace CarShare.Application.Features.Vehicles.Queries.SearchAvailableVehicles;

public sealed class SearchAvailableVehiclesQueryValidator
    : AbstractValidator<SearchAvailableVehiclesQuery>
{
    public SearchAvailableVehiclesQueryValidator()
    {
        RuleFor(query => query.StartDateTimeUtc)
            .NotEmpty()
            .WithMessage("Početak termina je obavezan.");

        RuleFor(query => query.EndDateTimeUtc)
            .GreaterThan(query => query.StartDateTimeUtc)
            .WithMessage(
                "Kraj termina mora biti posle početka.");

        RuleFor(query => query.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Geografska širina nije ispravna.");

        RuleFor(query => query.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Geografska dužina nije ispravna.");

        RuleFor(query => query.RadiusKilometers)
            .GreaterThan(0)
            .LessThanOrEqualTo(200)
            .WithMessage(
                "Radijus mora biti između 0 i 200 kilometara.");
    }
}