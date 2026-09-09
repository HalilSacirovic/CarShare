using MediatR;

namespace CarShare.Application.Features.Admin.Queries.GetPendingDriverVerifications;

public sealed record GetPendingDriverVerificationsQuery
    : IRequest<IReadOnlyList<PendingDriverVerificationViewModel>>;