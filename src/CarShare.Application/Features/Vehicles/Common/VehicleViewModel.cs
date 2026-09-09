using CarShare.Domain.Enums;

namespace CarShare.Application.Features.Vehicles.Common;

public sealed class VehicleViewModel
{
    public Guid Id { get; set; }

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

    public decimal? PricePerHour { get; set; }

    public decimal? PricePerDay { get; set; }

    public decimal DepositAmount { get; set; }

    public VehicleStatus Status { get; set; }

    public VerificationStatus VerificationStatus { get; set; }

    public List<string> PhotoUrls { get; set; } = new();
}