using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Admin.Queries.GetOpenDisputes;

public sealed class GetOpenDisputesQueryHandler
    : IRequestHandler<
        GetOpenDisputesQuery,
        IReadOnlyList<OpenDisputeViewModel>>
{
    private readonly IRepository<Dispute> _disputeRepository;

    public GetOpenDisputesQueryHandler(
        IRepository<Dispute> disputeRepository)
    {
        _disputeRepository = disputeRepository;
    }

    public Task<IReadOnlyList<OpenDisputeViewModel>> Handle(
        GetOpenDisputesQuery request,
        CancellationToken cancellationToken)
    {
        var disputes = _disputeRepository
            .Query()
            .Where(dispute =>
                dispute.Status == DisputeStatus.Open ||
                dispute.Status == DisputeStatus.UnderReview)
            .OrderBy(dispute => dispute.CreatedAtUtc)
            .Select(dispute =>
                new OpenDisputeViewModel(
                    dispute.Id,
                    dispute.ReservationId,
                    dispute.OpenedByUserId,
                    dispute.OpenedByUser.FirstName + " " +
                        dispute.OpenedByUser.LastName,
                    dispute.OpenedByUser.Email,
                    dispute.Reservation.Vehicle.Brand,
                    dispute.Reservation.Vehicle.Model,
                    dispute.Reason,
                    dispute.Description,
                    (int)dispute.Status,
                    dispute.CreatedAtUtc))
            .ToList();

        return Task.FromResult<IReadOnlyList<OpenDisputeViewModel>>(
            disputes);
    }
}