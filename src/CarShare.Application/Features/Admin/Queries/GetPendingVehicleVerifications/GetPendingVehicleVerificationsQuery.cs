using MediatR;

namespace CarShare.Application.Features.Admin.Queries.GetPendingVehicleVerifications;

public sealed record GetPendingVehicleVerificationsQuery
    : IRequest<IReadOnlyList<PendingVehicleVerificationViewModel>>;