namespace CarShare.Application.Features.Admin.Queries.GetPendingDriverVerifications;

public sealed record PendingDriverVerificationViewModel(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string DriverLicenseUrl,
    DateTime? SubmittedAtUtc);