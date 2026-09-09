using AutoMapper;
using CarShare.Application.Common.Interfaces;
using CarShare.Application.Features.Vehicles.Common;
using CarShare.Domain.Entities;
using MediatR;

namespace CarShare.Application.Features.Vehicles.Commands.CreateVehicle;

public sealed class CreateVehicleCommandHandler
    : IRequestHandler<CreateVehicleCommand, VehicleViewModel>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IRepository<Equipment> _equipmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateVehicleCommandHandler(
        IRepository<User> userRepository,
        IRepository<Vehicle> vehicleRepository,
        IRepository<Equipment> equipmentRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _vehicleRepository = vehicleRepository;
        _equipmentRepository = equipmentRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<VehicleViewModel> Handle(
        CreateVehicleCommand request,
        CancellationToken cancellationToken)
    {
        var owner = await _userRepository.GetByIdAsync(
            request.OwnerId,
            cancellationToken);

        if (owner is null)
        {
            throw new KeyNotFoundException(
                "Vlasnik vozila nije pronađen.");
        }

        if (!owner.IsOwner)
        {
            throw new InvalidOperationException(
                "Korisnik nema Owner ulogu.");
        }

        var vehicle = _mapper.Map<Vehicle>(request);

        vehicle.Photos = request.PhotoUrls
            .Select((url, index) => new VehiclePhoto
            {
                VehicleId = vehicle.Id,
                Url = url,
                IsMain = index == 0,
                SortOrder = index
            })
            .ToList();

        if (request.EquipmentIds.Count > 0)
        {
            var selectedEquipment = _equipmentRepository
                .Query()
                .Where(equipment =>
                    request.EquipmentIds.Contains(equipment.Id))
                .ToList();

            if (selectedEquipment.Count != request.EquipmentIds.Count)
            {
                throw new KeyNotFoundException(
                    "Jedna ili više stavki opreme nisu pronađene.");
            }

            vehicle.EquipmentItems = selectedEquipment;
        }

        await _vehicleRepository.AddAsync(
            vehicle,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<VehicleViewModel>(vehicle);
    }
}