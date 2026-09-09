namespace CarShare.API.Contracts.Handovers;

public sealed record ConfirmReturnRequest(
    int ReturnMileage,
    string ReturnPhotoUrl);