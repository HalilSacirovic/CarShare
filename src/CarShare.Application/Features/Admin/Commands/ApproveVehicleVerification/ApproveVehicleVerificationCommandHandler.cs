using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Admin.Commands.ApproveVehicleVerification;

public sealed class ApproveVehicleVerificationCommandHandler
    : IRequestHandler<ApproveVehicleVerificationCommand>
{
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveVehicleVerificationCommandHandler(
        IRepository<Vehicle> vehicleRepository,
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        ApproveVehicleVerificationCommand request,
        CancellationToken cancellationToken)
    {
        var vehicle = _vehicleRepository
            .Query()
            .FirstOrDefault(vehicle =>
                vehicle.Id == request.VehicleId)
            ?? throw new KeyNotFoundException(
                "Vozilo nije pronađeno.");

        if (string.IsNullOrWhiteSpace(
            vehicle.RegistrationDocumentUrl))
        {
            throw new InvalidOperationException(
                "Dokument vozila nije poslat.");
        }

        if (vehicle.VerificationStatus !=
            VerificationStatus.Pending)
        {
            throw new InvalidOperationException(
                "Samo vozilo koje čeka proveru može biti odobreno.");
        }

        vehicle.VerificationStatus =
            VerificationStatus.Approved;

        _vehicleRepository.Update(vehicle);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}