namespace CarShare.Application.Features.Admin.Queries.GetPendingVehicleVerifications;

public sealed record PendingVehicleVerificationViewModel(
    Guid VehicleId,
    Guid OwnerId,
    string OwnerName,
    string OwnerEmail,
    string Brand,
    string Model,
    int Year,
    string RegistrationDocumentUrl,
   DateTime? SubmittedAtUtc);