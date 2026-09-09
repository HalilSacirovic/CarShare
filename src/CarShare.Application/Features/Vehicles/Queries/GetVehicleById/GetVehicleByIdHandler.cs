using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarShare.Application.Common.Interfaces;
using CarShare.Application.Features.Vehicles.Common;
using CarShare.Domain.Entities;
using MediatR;

namespace CarShare.Application.Features.Vehicles.Queries.GetVehicleById;

public sealed class GetVehicleByIdQueryHandler
    : IRequestHandler<GetVehicleByIdQuery, VehicleViewModel>
{
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IMapper _mapper;

    public GetVehicleByIdQueryHandler(
        IRepository<Vehicle> vehicleRepository,
        IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    public Task<VehicleViewModel> Handle(
        GetVehicleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var vehicle = _vehicleRepository
            .Query()
            .Where(vehicle => vehicle.Id == request.Id)
            .ProjectTo<VehicleViewModel>(
                _mapper.ConfigurationProvider)
            .SingleOrDefault();

        if (vehicle is null)
        {
            throw new KeyNotFoundException(
                "Vozilo nije pronađeno.");
        }

        return Task.FromResult(vehicle);
    }
}