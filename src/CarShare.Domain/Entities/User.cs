using CarShare.Domain.Common;
using CarShare.Domain.Enums;

namespace CarShare.Domain.Entities;

public sealed class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public string? DriverLicenseUrl { get; set; }

    public VerificationStatus DriverVerificationStatus { get; set; }
        = VerificationStatus.NotSubmitted;

    public bool IsOwner { get; set; }

    public bool IsRenter { get; set; } = true;

    public bool IsAdmin { get; set; }

    public decimal AverageRating { get; set; }

    public decimal TrustScore { get; set; }
    public ICollection<Vehicle> Vehicles { get; set; }
    = new List<Vehicle>();

    public ICollection<Reservation> ReservationsAsRenter { get; set; }
    = new List<Reservation>();


    public ICollection<Review> ReviewsWritten { get; set; }
    = new List<Review>();

    public ICollection<Review> ReviewsReceived { get; set; }
        = new List<Review>();

    public ICollection<Dispute> DisputesOpened { get; set; }
        = new List<Dispute>();

    public ICollection<Dispute> DisputesResolvedAsAdmin { get; set; }
        = new List<Dispute>();

    public ICollection<RefreshToken> RefreshTokens { get; set; }
    = new List<RefreshToken>();
}