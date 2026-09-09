using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using MediatR;

namespace CarShare.Application.Features.Vehicles.Commands.DeleteAvailabilityBlock;

public sealed class DeleteAvailabilityBlockCommandHandler
    : IRequestHandler<DeleteAvailabilityBlockCommand>
{
    private readonly IRepository<AvailabilityBlock>
        _availabilityRepository;
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAvailabilityBlockCommandHandler(
        IRepository<AvailabilityBlock> availabilityRepository,
        IRepository<Vehicle> vehicleRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _availabilityRepository = availabilityRepository;
        _vehicleRepository = vehicleRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        DeleteAvailabilityBlockCommand request,
        CancellationToken cancellationToken)
    {
        var block = await _availabilityRepository.GetByIdAsync(
            request.AvailabilityBlockId,
            cancellationToken)
            ?? throw new KeyNotFoundException(
                "Blokirani period nije pronađen.");

        var vehicle = await _vehicleRepository.GetByIdAsync(
            block.VehicleId,
            cancellationToken)
            ?? throw new KeyNotFoundException(
                "Vozilo nije pronađeno.");

        if (vehicle.OwnerId != _currentUserService.UserId)
        {
            throw new UnauthorizedAccessException(
                "Možete ukloniti dostupnost samo svojih vozila.");
        }

        _availabilityRepository.Remove(block);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}