using MediatR;

namespace CarShare.Application.Features.Users.Commands.SubmitDriverLicense;

public sealed record SubmitDriverLicenseCommand(
    string DriverLicenseUrl) : IRequest<Unit>;