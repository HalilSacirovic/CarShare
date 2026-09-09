using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarShare.Application.Common.Interfaces;
using CarShare.Application.Features.Vehicles.Common;
using CarShare.Domain.Entities;
using MediatR;

namespace CarShare.Application.Features.Vehicles.Queries.SearchAvailableVehicles;

public sealed class SearchAvailableVehiclesQueryHandler
    : IRequestHandler<
        SearchAvailableVehiclesQuery,
        IReadOnlyList<VehicleViewModel>>
{
    private readonly IVehicleAvailabilitySearchService
        _searchService;
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IMapper _mapper;

    public SearchAvailableVehiclesQueryHandler(
        IVehicleAvailabilitySearchService searchService,
        IRepository<Vehicle> vehicleRepository,
        IMapper mapper)
    {
        _searchService = searchService;
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<VehicleViewModel>> Handle(
        SearchAvailableVehiclesQuery request,
        CancellationToken cancellationToken)
    {
        var vehicleIds =
            await _searchService.FindAvailableVehicleIdsAsync(
                request.StartDateTimeUtc,
                request.EndDateTimeUtc,
                request.Latitude,
                request.Longitude,
                request.RadiusKilometers,
                cancellationToken);

        if (vehicleIds.Count == 0)
        {
            return Array.Empty<VehicleViewModel>();
        }

        var vehicles = _vehicleRepository
            .Query()
            .Where(vehicle => vehicleIds.Contains(vehicle.Id))
            .OrderBy(vehicle => vehicle.PricePerDay)
            .ProjectTo<VehicleViewModel>(
                _mapper.ConfigurationProvider)
            .ToList();

        return vehicles;
    }
}