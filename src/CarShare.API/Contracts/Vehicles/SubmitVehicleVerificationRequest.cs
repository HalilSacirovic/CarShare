namespace CarShare.API.Contracts.Vehicles;

public sealed record SubmitVehicleVerificationRequest(
    string RegistrationDocumentUrl);