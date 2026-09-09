using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using MediatR;

namespace CarShare.Application.Features.Vehicles.Commands.CreateAvailabilityBlock;

public sealed class CreateAvailabilityBlockCommandHandler
    : IRequestHandler<CreateAvailabilityBlockCommand, Guid>
{
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IRepository<AvailabilityBlock>
        _availabilityRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAvailabilityBlockCommandHandler(
        IRepository<Vehicle> vehicleRepository,
        IRepository<AvailabilityBlock> availabilityRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _availabilityRepository = availabilityRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateAvailabilityBlockCommand request,
        CancellationToken cancellationToken)
    {
        var vehicle = _vehicleRepository
            .Query()
            .FirstOrDefault(vehicle =>
                vehicle.Id == request.VehicleId);

        if (vehicle is null)
        {
            throw new KeyNotFoundException(
                "Vozilo nije pronađeno.");
        }

        if (vehicle.OwnerId != _currentUserService.UserId)
        {
            throw new UnauthorizedAccessException(
                "Možete menjati dostupnost samo svojih vozila.");
        }

        var periodOverlaps = _availabilityRepository
            .Query()
            .Any(block =>
                block.VehicleId == request.VehicleId &&
                block.StartDateTimeUtc < request.EndDateTimeUtc &&
                block.EndDateTimeUtc > request.StartDateTimeUtc);

        if (periodOverlaps)
        {
            throw new InvalidOperationException(
                "Vozilo već ima blokiran period koji se preklapa.");
        }

        var availabilityBlock = new AvailabilityBlock
        {
            VehicleId = request.VehicleId,
            StartDateTimeUtc = request.StartDateTimeUtc,
            EndDateTimeUtc = request.EndDateTimeUtc,
            Reason = request.Reason.Trim()
        };

        await _availabilityRepository.AddAsync(
                       availabilityBlock,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return availabilityBlock.Id;
    }
}