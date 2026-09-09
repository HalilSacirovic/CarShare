using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;
using System.Linq;

namespace CarShare.Application.Features.Admin.Queries.GetPendingVehicleVerifications;

public sealed class GetPendingVehicleVerificationsQueryHandler
    : IRequestHandler<
        GetPendingVehicleVerificationsQuery,
        IReadOnlyList<PendingVehicleVerificationViewModel>>
{
    private readonly IRepository<Vehicle> _vehicleRepository;

    public GetPendingVehicleVerificationsQueryHandler(
        IRepository<Vehicle> vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public Task<IReadOnlyList<PendingVehicleVerificationViewModel>> Handle(
        GetPendingVehicleVerificationsQuery request,
        CancellationToken cancellationToken)
    {
        var vehicles = _vehicleRepository
            .Query()
            .Where(vehicle =>
                vehicle.VerificationStatus ==
                    VerificationStatus.Pending &&
                vehicle.RegistrationDocumentUrl != null)
            .OrderBy(vehicle => vehicle.UpdatedAtUtc)
            .Select(vehicle =>
                new PendingVehicleVerificationViewModel(
                    vehicle.Id,
                    vehicle.OwnerId,
                    vehicle.Owner.FirstName + " " +
                        vehicle.Owner.LastName,
                    vehicle.Owner.Email,
                    vehicle.Brand,
                    vehicle.Model,
                    vehicle.Year,
                    vehicle.RegistrationDocumentUrl!,
                    vehicle.UpdatedAtUtc))
            .ToList();

        return Task.FromResult<
            IReadOnlyList<PendingVehicleVerificationViewModel>>(
                vehicles);
    }
}