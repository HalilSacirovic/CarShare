using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;


namespace CarShare.Application.Features.Admin.Commands.ApproveDriverVerification;

public sealed class ApproveDriverVerificationCommandHandler
    : IRequestHandler<ApproveDriverVerificationCommand>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveDriverVerificationCommandHandler(
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        ApproveDriverVerificationCommand request,
        CancellationToken cancellationToken)
    {
        var user = _userRepository
     .Query()
     .FirstOrDefault(user => user.Id == request.UserId)
     ?? throw new KeyNotFoundException(
         "Korisnik nije pronađen.");

        if (string.IsNullOrWhiteSpace(user.DriverLicenseUrl))
        {
            throw new InvalidOperationException(
                "Korisnik nije poslao vozačku dozvolu.");
        }

        if (user.DriverVerificationStatus != VerificationStatus.Pending)
        {
            throw new InvalidOperationException(
                "Samo zahtev na čekanju može biti odobren.");
        }

        user.DriverVerificationStatus = VerificationStatus.Approved;

        _userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}