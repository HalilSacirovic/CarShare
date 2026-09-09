using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Admin.Queries.GetPendingDriverVerifications;

public sealed class GetPendingDriverVerificationsQueryHandler
    : IRequestHandler<
        GetPendingDriverVerificationsQuery,
        IReadOnlyList<PendingDriverVerificationViewModel>>
{
    private readonly IRepository<User> _userRepository;

    public GetPendingDriverVerificationsQueryHandler(
        IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<
        IReadOnlyList<PendingDriverVerificationViewModel>> Handle(
        GetPendingDriverVerificationsQuery request,
        CancellationToken cancellationToken)
    {
        var users = _userRepository
            .Query()
            .Where(user =>
                user.DriverVerificationStatus ==
                    VerificationStatus.Pending &&
                user.DriverLicenseUrl != null)
            .OrderBy(user => user.UpdatedAtUtc)
            .Select(user =>
                new PendingDriverVerificationViewModel(
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.DriverLicenseUrl!,
                    user.UpdatedAtUtc))
            .ToList();

        return Task.FromResult<
            IReadOnlyList<PendingDriverVerificationViewModel>>(
                users);
    }
}

