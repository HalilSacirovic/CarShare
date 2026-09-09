using CarShare.Domain.Common;
using CarShare.Domain.Enums;

namespace CarShare.Domain.Entities;

public sealed class Vehicle : BaseEntity
{
    public Guid OwnerId { get; set; }

    public User Owner { get; set; } = null!;

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

    public VehicleStatus Status { get; set; } = VehicleStatus.Available;

    public VerificationStatus VerificationStatus { get; set; }
        = VerificationStatus.NotSubmitted;

    public string? RegistrationDocumentUrl { get; set; }

    public ICollection<VehiclePhoto> Photos { get; set; }
        = new List<VehiclePhoto>();

    public ICollection<Equipment> EquipmentItems { get; set; }
        = new List<Equipment>();

    public ICollection<AvailabilityBlock> AvailabilityBlocks { get; set; }
    = new List<AvailabilityBlock>();

    public ICollection<Reservation> Reservations { get; set; }
        = new List<Reservation>();
}