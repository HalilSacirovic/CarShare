using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Admin.Commands.RejectVehicleVerification;

public sealed class RejectVehicleVerificationCommandHandler
    : IRequestHandler<RejectVehicleVerificationCommand>
{
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectVehicleVerificationCommandHandler(
        IRepository<Vehicle> vehicleRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        RejectVehicleVerificationCommand request,
        CancellationToken cancellationToken)
    {
        var vehicle = _vehicleRepository
            .Query()
            .FirstOrDefault(vehicle =>
                vehicle.Id == request.VehicleId)
            ?? throw new KeyNotFoundException(
                "Vozilo nije pronađeno.");

        if (vehicle.VerificationStatus !=
            VerificationStatus.Pending)
        {
            throw new InvalidOperationException(
                "Samo vozilo koje čeka proveru može biti odbijeno.");
        }

        vehicle.VerificationStatus =
            VerificationStatus.Rejected;

        _vehicleRepository.Update(vehicle);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}