using MediatR;

namespace CarShare.Application.Features.Vehicles.Commands.SubmitVehicleVerification;

public sealed record SubmitVehicleVerificationCommand(
    Guid VehicleId,
    string RegistrationDocumentUrl) : IRequest;