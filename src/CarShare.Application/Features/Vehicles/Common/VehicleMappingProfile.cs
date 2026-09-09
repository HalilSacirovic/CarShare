using AutoMapper;
using CarShare.Application.Features.Vehicles.Commands.CreateVehicle;
using CarShare.Domain.Entities;

namespace CarShare.Application.Features.Vehicles.Common;

public sealed class VehicleMappingProfile : Profile
{
    public VehicleMappingProfile()
    {
        CreateMap<CreateVehicleCommand, Vehicle>()
            .ForMember(
                destination => destination.Owner,
                options => options.Ignore())
            .ForMember(
                destination => destination.Photos,
                options => options.Ignore())
            .ForMember(
                destination => destination.EquipmentItems,
                options => options.Ignore())
            .ForMember(
                destination => destination.AvailabilityBlocks,
                options => options.Ignore())
            .ForMember(
                destination => destination.Reservations,
                options => options.Ignore());

        CreateMap<Vehicle, VehicleViewModel>()
            .ForMember(
                destination => destination.PhotoUrls,
                options => options.MapFrom(source =>
                    source.Photos
                        .OrderBy(photo => photo.SortOrder)
                        .Select(photo => photo.Url)));
    }
}