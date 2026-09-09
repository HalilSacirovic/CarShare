using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarShare.Application.Common.Interfaces;
using CarShare.Application.Features.Vehicles.Common;
using CarShare.Domain.Entities;
using MediatR;

namespace CarShare.Application.Features.Vehicles.Queries.GetAllVehicles;

public sealed class GetAllVehiclesQueryHandler
    : IRequestHandler<
        GetAllVehiclesQuery,
        IReadOnlyList<VehicleViewModel>>
{
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IMapper _mapper;

    public GetAllVehiclesQueryHandler(
        IRepository<Vehicle> vehicleRepository,
        IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    public Task<IReadOnlyList<VehicleViewModel>> Handle(
        GetAllVehiclesQuery request,
        CancellationToken cancellationToken)
    {
        var vehicles = _vehicleRepository
            .Query()
            .OrderByDescending(vehicle => vehicle.CreatedAtUtc)
            .ProjectTo<VehicleViewModel>(
                _mapper.ConfigurationProvider)
            .ToList();

        return Task.FromResult<IReadOnlyList<VehicleViewModel>>(
            vehicles);
    }
}