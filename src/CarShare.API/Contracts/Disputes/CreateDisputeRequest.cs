namespace CarShare.API.Contracts.Disputes;

public sealed record CreateDisputeRequest(
    string Reason,
    string Description);