using CarShare.Application.Features.Vehicles.Queries.SearchAvailableVehicles;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace CarShare.Application.Tests.Vehicles;

public sealed class SearchAvailableVehiclesQueryValidatorTests
{
    private readonly SearchAvailableVehiclesQueryValidator
        _validator = new();

    [Fact]
    public async Task Validate_ShouldBeValid_WhenDataIsCorrect()
    {
        var query = new SearchAvailableVehiclesQuery(
            new DateTime(
                2026, 10, 10, 10, 0, 0,
                DateTimeKind.Utc),
            new DateTime(
                2026, 10, 11, 10, 0, 0,
                DateTimeKind.Utc),
            43.1367,
            20.5122,
            50);

        var result =
            await _validator.TestValidateAsync(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenEndIsBeforeStart()
    {
        var query = new SearchAvailableVehiclesQuery(
            new DateTime(
                2026, 10, 11, 10, 0, 0,
                DateTimeKind.Utc),
            new DateTime(
                2026, 10, 10, 10, 0, 0,
                DateTimeKind.Utc),
            43.1367,
            20.5122,
            50);

        var result =
            await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(
            query => query.EndDateTimeUtc);
    }
}