using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Users.Queries.GetPublicUserProfile;

public sealed class GetPublicUserProfileQueryHandler
    : IRequestHandler<
        GetPublicUserProfileQuery,
        PublicUserProfileViewModel>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Review> _reviewRepository;
    private readonly IRepository<Reservation>
        _reservationRepository;

    public GetPublicUserProfileQueryHandler(
        IRepository<User> userRepository,
        IRepository<Review> reviewRepository,
        IRepository<Reservation> reservationRepository)
    {
        _userRepository = userRepository;
        _reviewRepository = reviewRepository;
        _reservationRepository = reservationRepository;
    }

    public Task<PublicUserProfileViewModel> Handle(
        GetPublicUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        var user = _userRepository
            .Query()
            .FirstOrDefault(user =>
                user.Id == request.UserId);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "Korisnik nije pronađen.");
        }

        var completedReservations =
            _reservationRepository
                .Query()
                .Count(reservation =>
                    reservation.RenterId ==
                        request.UserId &&
                    reservation.Status ==
                        ReservationStatus.Completed);

        var reviews =
            (
                from review in _reviewRepository.Query()
                join reviewer in _userRepository.Query()
                    on review.ReviewerId equals reviewer.Id
                where review.ReviewedUserId ==
                    request.UserId
                orderby review.CreatedAtUtc descending
                select new PublicReviewViewModel(
                    review.Id,
                    reviewer.FirstName +
                        " " +
                        reviewer.LastName,
                    review.Rating,
                    review.Comment,
                    review.CreatedAtUtc)
            )
            .ToList();

        var result = new PublicUserProfileViewModel(
            user.Id,
            user.FirstName + " " + user.LastName,
            user.DriverVerificationStatus ==
                VerificationStatus.Approved,
            user.AverageRating,
            user.TrustScore,
            completedReservations,
            reviews);

        return Task.FromResult(result);
    }
}