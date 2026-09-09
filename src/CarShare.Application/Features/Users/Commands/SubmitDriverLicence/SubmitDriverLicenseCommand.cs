using MediatR;

namespace CarShare.Application.Features.Users.Commands.SubmitDriverLicense;

public sealed record SubmitDriverLicenseCommand(
    string DriverLicenseUrl) : IRequest<Unit>;
C:\Users\PC\Desktop\CarShare\src\CarShare.Application\Features\Users\Commands\SubmitDriverLicence\SubmitDriverLicenseCommand.cs