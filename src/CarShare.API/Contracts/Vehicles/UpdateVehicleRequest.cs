using CarShare.Domain.Enums;

namespace CarShare.API.Contracts.Vehicles;

public sealed record UpdateVehicleRequest(
    string Brand,
    string Model,
    int Year,
    string Color,
    int Mileage,
    int SeatCount,
    TransmissionType TransmissionType,
    FuelType FuelType,
    string Description,
    string Address,
    double Latitude,
    double Longitude,
    decimal? PricePerHour,
    decimal? PricePerDay,
    decimal DepositAmount,
    bool SmokingAllowed,
    bool PetsAllowed,
    int DailyMileageLimit,
    bool AutoApproveReservations);