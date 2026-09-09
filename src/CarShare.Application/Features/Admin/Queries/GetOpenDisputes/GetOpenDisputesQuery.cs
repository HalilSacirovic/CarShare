using MediatR;

namespace CarShare.Application.Features.Admin.Queries.GetOpenDisputes;

public sealed record GetOpenDisputesQuery
    : IRequest<IReadOnlyList<OpenDisputeViewModel>>;