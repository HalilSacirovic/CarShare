using MediatR;

namespace CarShare.Application.Features.Admin.Commands.RejectVehicleVerification;

public sealed record RejectVehicleVerificationCommand(
    Guid VehicleId) : IRequest;