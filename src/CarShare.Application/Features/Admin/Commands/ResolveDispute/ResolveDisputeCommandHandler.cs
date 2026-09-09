using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Admin.Commands.ResolveDispute;

public sealed class ResolveDisputeCommandHandler
    : IRequestHandler<ResolveDisputeCommand>
{
    private readonly IRepository<Dispute> _disputeRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ResolveDisputeCommandHandler(
        IRepository<Dispute> disputeRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _disputeRepository = disputeRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        ResolveDisputeCommand request,
        CancellationToken cancellationToken)
    {
        var dispute = _disputeRepository
            .Query()
            .FirstOrDefault(dispute =>
                dispute.Id == request.DisputeId)
            ?? throw new KeyNotFoundException(
                "Spor nije pronađen.");

        var canBeResolved =
            dispute.Status == DisputeStatus.Open ||
            dispute.Status == DisputeStatus.UnderReview;

        if (!canBeResolved)
        {
            throw new InvalidOperationException(
                "Ovaj spor je već zatvoren.");
        }

        dispute.Status = DisputeStatus.Resolved;
        dispute.ResolvedByAdminId =
            _currentUserService.UserId;
        dispute.AdminDecision =
            request.AdminDecision.Trim();
        dispute.ResolvedAtUtc = DateTime.UtcNow;

        _disputeRepository.Update(dispute);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}