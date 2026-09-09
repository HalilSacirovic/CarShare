using CarShare.Application.Common.Interfaces;
using CarShare.Application.Features.Vehicles.Common;
using CarShare.Domain.Entities;
using MediatR;

namespace CarShare.Application.Features.Vehicles.Queries.GetVehicleAvailability;

public sealed class GetVehicleAvailabilityQueryHandler
    : IRequestHandler<
        GetVehicleAvailabilityQuery,
        IReadOnlyList<AvailabilityBlockViewModel>>
{
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IRepository<AvailabilityBlock>
        _availabilityRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetVehicleAvailabilityQueryHandler(
        IRepository<Vehicle> vehicleRepository,
        IRepository<AvailabilityBlock> availabilityRepository,
        ICurrentUserService currentUserService)
    {
        _vehicleRepository = vehicleRepository;
        _availabilityRepository = availabilityRepository;
        _currentUserService = currentUserService;
    }

    public Task<IReadOnlyList<AvailabilityBlockViewModel>> Handle(
        GetVehicleAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        var vehicle = _vehicleRepository
            .Query()
            .FirstOrDefault(vehicle =>
                vehicle.Id == request.VehicleId)
            ?? throw new KeyNotFoundException(
                "Vozilo nije pronađeno.");

        if (vehicle.OwnerId != _currentUserService.UserId)
        {
            throw new UnauthorizedAccessException(
                "Možete pregledati dostupnost samo svojih vozila.");
        }

        var blocks = _availabilityRepository
            .Query()
            .Where(block =>
                block.VehicleId == request.VehicleId)
            .OrderBy(block => block.StartDateTimeUtc)
            .Select(block =>
                new AvailabilityBlockViewModel(
                    block.Id,
                    block.VehicleId,
                    block.StartDateTimeUtc,
                    block.EndDateTimeUtc,
                    block.Reason))
            .ToList();

        return Task.FromResult<
            IReadOnlyList<AvailabilityBlockViewModel>>(
                blocks);
    }
}