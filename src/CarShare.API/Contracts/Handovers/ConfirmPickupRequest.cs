namespace CarShare.API.Contracts.Handovers;

public sealed record ConfirmPickupRequest(
    int PickupMileage,
    string PickupPhotoUrl);