namespace CarShare.Application.Features.Admin.Queries.GetOpenDisputes;

public sealed record OpenDisputeViewModel(
    Guid DisputeId,
    Guid ReservationId,
    Guid OpenedByUserId,
    string OpenedByName,
    string OpenedByEmail,
    string VehicleBrand,
    string VehicleModel,
    string Reason,
    string Description,
    int Status,
    DateTime? OpenedAtUtc);