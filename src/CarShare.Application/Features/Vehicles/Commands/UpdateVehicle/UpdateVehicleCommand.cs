using CarShare.Application.Features.Vehicles.Common;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Vehicles.Commands.UpdateVehicle;

public sealed record UpdateVehicleCommand(
    Guid VehicleId,
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
    bool AutoApproveReservations)
    : IRequest<VehicleViewModel>;