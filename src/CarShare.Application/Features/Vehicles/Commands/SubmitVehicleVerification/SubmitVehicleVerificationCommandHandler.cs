using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Vehicles.Commands.SubmitVehicleVerification;

public sealed class SubmitVehicleVerificationCommandHandler
    : IRequestHandler<SubmitVehicleVerificationCommand>
{
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitVehicleVerificationCommandHandler(
        IRepository<Vehicle> vehicleRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        SubmitVehicleVerificationCommand request,
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
                "Možete poslati dokument samo za svoje vozilo.");
        }

        if (vehicle.VerificationStatus ==
            VerificationStatus.Pending)
        {
            throw new InvalidOperationException(
                "Dokument vozila već čeka proveru.");
        }

        if (vehicle.VerificationStatus ==
            VerificationStatus.Approved)
        {
            throw new InvalidOperationException(
                "Vozilo je već verifikovano.");
        }

        vehicle.RegistrationDocumentUrl =
            request.RegistrationDocumentUrl.Trim();

        vehicle.VerificationStatus =
            VerificationStatus.Pending;

        _vehicleRepository.Update(vehicle);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}