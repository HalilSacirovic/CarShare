using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Users.Commands.SubmitDriverLicense;

public sealed class SubmitDriverLicenseCommandHandler
    : IRequestHandler<SubmitDriverLicenseCommand, Unit>
{
    private readonly IRepository<User> _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitDriverLicenseCommandHandler(
        IRepository<User> userRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        SubmitDriverLicenseCommand request,
        CancellationToken cancellationToken)
    {
        var user = _userRepository
            .Query()
            .FirstOrDefault(user =>
                user.Id == _currentUserService.UserId);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "Korisnik nije pronađen.");
        }

        if (user.DriverVerificationStatus ==
            VerificationStatus.Approved)
        {
            throw new InvalidOperationException(
                "Vozačka dozvola je već odobrena.");
        }

        user.DriverLicenseUrl =
            request.DriverLicenseUrl.Trim();

        user.DriverVerificationStatus =
            VerificationStatus.Pending;

        user.UpdatedAtUtc = DateTime.UtcNow;

        _userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}