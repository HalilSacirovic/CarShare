using MediatR;

namespace CarShare.Application.Features.Admin.Commands.ApproveVehicleVerification;

public sealed record ApproveVehicleVerificationCommand(
    Guid VehicleId) : IRequest;