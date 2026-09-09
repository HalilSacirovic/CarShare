using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using MediatR;

namespace CarShare.Application.Features.Vehicles.Commands.DeleteVehicle;

public sealed class DeleteVehicleCommandHandler
    : IRequestHandler<DeleteVehicleCommand>
{
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IRepository<Reservation>
        _reservationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVehicleCommandHandler(
        IRepository<Vehicle> vehicleRepository,
        IRepository<Reservation> reservationRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _reservationRepository = reservationRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        DeleteVehicleCommand request,
        CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(
            request.VehicleId,
            cancellationToken)
            ?? throw new KeyNotFoundException(
                "Vozilo nije pronađeno.");

        if (vehicle.OwnerId != _currentUserService.UserId)
        {
            throw new UnauthorizedAccessException(
                "Možete obrisati samo svoje vozilo.");
        }

        var hasReservations = _reservationRepository
            .Query()
            .Any(reservation =>
                reservation.VehicleId == request.VehicleId);

        if (hasReservations)
        {
            throw new InvalidOperationException(
                "Vozilo koje ima rezervacije ne može biti obrisano.");
        }

        _vehicleRepository.Remove(vehicle);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}