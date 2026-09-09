using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Admin.Commands.RejectDriverVerification;

public sealed class RejectDriverVerificationCommandHandler
    : IRequestHandler<RejectDriverVerificationCommand>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectDriverVerificationCommandHandler(
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        RejectDriverVerificationCommand request,
        CancellationToken cancellationToken)
    {
        var user = _userRepository
            .Query()
            .FirstOrDefault(user => user.Id == request.UserId)
            ?? throw new KeyNotFoundException(
                "Korisnik nije pronađen.");

        if (user.DriverVerificationStatus != VerificationStatus.Pending)
        {
            throw new InvalidOperationException(
                "Samo zahtev na čekanju može biti odbijen.");
        }

        user.DriverVerificationStatus = VerificationStatus.Rejected;

        _userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}