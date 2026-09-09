using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Reviews.Commands.CreateReview;

public sealed class CreateReviewCommandHandler
    : IRequestHandler<CreateReviewCommand, CreateReviewResult>
{
    private readonly IRepository<Reservation>
        _reservationRepository;
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IRepository<Review> _reviewRepository;
    private readonly IRepository<User> _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewCommandHandler(
        IRepository<Reservation> reservationRepository,
        IRepository<Vehicle> vehicleRepository,
        IRepository<Review> reviewRepository,
        IRepository<User> userRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _vehicleRepository = vehicleRepository;
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateReviewResult> Handle(
        CreateReviewCommand request,
        CancellationToken cancellationToken)
    {
        var reservation = _reservationRepository
            .Query()
            .FirstOrDefault(reservation =>
                reservation.Id == request.ReservationId);

        if (reservation is null)
        {
            throw new KeyNotFoundException(
                "Rezervacija nije pronađena.");
        }

        if (reservation.Status != ReservationStatus.Completed)
        {
            throw new InvalidOperationException(
                "Ocenjivanje je moguće tek nakon završetka rezervacije.");
        }

        var vehicle = _vehicleRepository
            .Query()
            .FirstOrDefault(vehicle =>
                vehicle.Id == reservation.VehicleId);

        if (vehicle is null)
        {
            throw new KeyNotFoundException(
                "Vozilo nije pronađeno.");
        }

        var currentUserId = _currentUserService.UserId;

        Guid reviewedUserId;

        if (currentUserId == reservation.RenterId)
        {
            reviewedUserId = vehicle.OwnerId;
        }
        else if (currentUserId == vehicle.OwnerId)
        {
            reviewedUserId = reservation.RenterId;
        }
        else
        {
            throw new UnauthorizedAccessException(
                "Samo učesnici rezervacije mogu ostaviti ocenu.");
        }

        var reviewAlreadyExists = _reviewRepository
            .Query()
            .Any(review =>
                review.ReservationId == reservation.Id &&
                review.ReviewerId == currentUserId);

        if (reviewAlreadyExists)
        {
            throw new InvalidOperationException(
                "Već ste ocenili korisnika za ovu rezervaciju.");
        }

        var reviewedUser = _userRepository
            .Query()
            .FirstOrDefault(user =>
                user.Id == reviewedUserId);

        if (reviewedUser is null)
        {
            throw new KeyNotFoundException(
                "Korisnik koji se ocenjuje nije pronađen.");
        }

        var existingRatings = _reviewRepository
            .Query()
            .Where(review =>
                review.ReviewedUserId == reviewedUserId)
            .Select(review => review.Rating)
            .ToList();

        var ratingSum =
            existingRatings.Sum() + request.Rating;

        var ratingCount =
            existingRatings.Count + 1;

        reviewedUser.AverageRating = Math.Round(
            ratingSum / (decimal)ratingCount,
            2);

        reviewedUser.TrustScore = Math.Round(
            reviewedUser.AverageRating * 20m,
            2);

        var review = new Review
        {
            ReservationId = reservation.Id,
            ReviewerId = currentUserId,
            ReviewedUserId = reviewedUserId,
            Rating = request.Rating,
            Comment = string.IsNullOrWhiteSpace(request.Comment)
                ? null
                : request.Comment.Trim()
        };

        await _reviewRepository.AddAsync(
            review,
            cancellationToken);

        _userRepository.Update(reviewedUser);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateReviewResult(
            review.Id,
            review.ReservationId,
            review.ReviewerId,
            review.ReviewedUserId,
            review.Rating,
            review.Comment);
    }
}