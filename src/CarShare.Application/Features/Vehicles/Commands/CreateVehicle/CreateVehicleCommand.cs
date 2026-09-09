using CarShare.Application.Features.Vehicles.Common;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Vehicles.Commands.CreateVehicle;

public sealed class CreateVehicleCommand : IRequest<VehicleViewModel>
{
    public Guid OwnerId { get; set; }

    public string Brand { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    public string Color { get; set; } = string.Empty;

    public int Mileage { get; set; }

    public int SeatCount { get; set; }

    public TransmissionType TransmissionType { get; set; }

    public FuelType FuelType { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public decimal? PricePerHour { get; set; }

    public decimal? PricePerDay { get; set; }

    public decimal DepositAmount { get; set; }

    public bool SmokingAllowed { get; set; }

    public bool PetsAllowed { get; set; }

    public int DailyMileageLimit { get; set; }

    public bool AutoApproveReservations { get; set; }

    public List<string> PhotoUrls { get; set; } = new();

    public List<Guid> EquipmentIds { get; set; } = new();
}