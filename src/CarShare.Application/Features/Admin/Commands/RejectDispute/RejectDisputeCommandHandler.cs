using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Admin.Commands.RejectDispute;

public sealed class RejectDisputeCommandHandler
    : IRequestHandler<RejectDisputeCommand>
{
    private readonly IRepository<Dispute> _disputeRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RejectDisputeCommandHandler(
        IRepository<Dispute> disputeRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _disputeRepository = disputeRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        RejectDisputeCommand request,
        CancellationToken cancellationToken)
    {
        var dispute = _disputeRepository
            .Query()
            .FirstOrDefault(dispute =>
                dispute.Id == request.DisputeId)
            ?? throw new KeyNotFoundException(
                "Spor nije pronađen.");

        var canBeRejected =
            dispute.Status == DisputeStatus.Open ||
            dispute.Status == DisputeStatus.UnderReview;

        if (!canBeRejected)
        {
            throw new InvalidOperationException(
                "Ovaj spor je već zatvoren.");
        }

        if (string.IsNullOrWhiteSpace(request.AdminDecision))
        {
            throw new ArgumentException(
                "Obrazloženje odluke je obavezno.");
        }

        dispute.Status = DisputeStatus.Rejected;
        dispute.ResolvedByAdminId =
            _currentUserService.UserId;
        dispute.AdminDecision =
            request.AdminDecision.Trim();
        dispute.ResolvedAtUtc = DateTime.UtcNow;

        _disputeRepository.Update(dispute);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}