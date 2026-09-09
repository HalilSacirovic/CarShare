using AutoMapper;
using CarShare.Application.Common.Interfaces;
using CarShare.Application.Features.Vehicles.Common;
using CarShare.Domain.Entities;
using MediatR;

namespace CarShare.Application.Features.Vehicles.Commands.UpdateVehicle;

public sealed class UpdateVehicleCommandHandler
    : IRequestHandler<UpdateVehicleCommand, VehicleViewModel>
{
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateVehicleCommandHandler(
        IRepository<Vehicle> vehicleRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<VehicleViewModel> Handle(
        UpdateVehicleCommand request,
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
                "Možete menjati samo svoja vozila.");
        }

        vehicle.Brand = request.Brand.Trim();
        vehicle.Model = request.Model.Trim();
        vehicle.Year = request.Year;
        vehicle.Color = request.Color.Trim();
        vehicle.Mileage = request.Mileage;
        vehicle.SeatCount = request.SeatCount;
        vehicle.TransmissionType = request.TransmissionType;
        vehicle.FuelType = request.FuelType;
        vehicle.Description = request.Description.Trim();
        vehicle.Address = request.Address.Trim();
        vehicle.Latitude = request.Latitude;
        vehicle.Longitude = request.Longitude;
        vehicle.PricePerHour = request.PricePerHour;
        vehicle.PricePerDay = request.PricePerDay;
        vehicle.DepositAmount = request.DepositAmount;
        vehicle.SmokingAllowed = request.SmokingAllowed;
        vehicle.PetsAllowed = request.PetsAllowed;
        vehicle.DailyMileageLimit =
            request.DailyMileageLimit;
        vehicle.AutoApproveReservations =
            request.AutoApproveReservations;

        _vehicleRepository.Update(vehicle);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<VehicleViewModel>(vehicle);
    }
}