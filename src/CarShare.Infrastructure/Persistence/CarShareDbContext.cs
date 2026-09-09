using CarShare.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShare.Infrastructure.Persistence;

public sealed class CarShareDbContext : DbContext
{
    public CarShareDbContext(DbContextOptions<CarShareDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<VehiclePhoto> VehiclePhotos => Set<VehiclePhoto>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<AvailabilityBlock> AvailabilityBlocks => Set<AvailabilityBlock>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<RentalHandover> RentalHandovers => Set<RentalHandover>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Dispute> Disputes => Set<Dispute>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<Vehicle>()
            .Property(vehicle => vehicle.PricePerHour)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Vehicle>()
            .Property(vehicle => vehicle.PricePerDay)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Vehicle>()
            .Property(vehicle => vehicle.DepositAmount)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Reservation>()
            .Property(reservation => reservation.RentalAmount)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Reservation>()
            .Property(reservation => reservation.DepositAmount)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Payment>()
            .Property(payment => payment.RentalAmount)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Payment>()
            .Property(payment => payment.DepositAmount)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Payment>()
            .Property(payment => payment.TotalAmount)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Vehicle>()
            .HasOne(vehicle => vehicle.Owner)
            .WithMany(user => user.Vehicles)
            .HasForeignKey(vehicle => vehicle.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .HasOne(reservation => reservation.Renter)
            .WithMany(user => user.ReservationsAsRenter)
            .HasForeignKey(reservation => reservation.RenterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .HasOne(reservation => reservation.Vehicle)
            .WithMany(vehicle => vehicle.Reservations)
            .HasForeignKey(reservation => reservation.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VehiclePhoto>()
            .HasOne(photo => photo.Vehicle)
            .WithMany(vehicle => vehicle.Photos)
            .HasForeignKey(photo => photo.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AvailabilityBlock>()
            .HasOne(block => block.Vehicle)
            .WithMany(vehicle => vehicle.AvailabilityBlocks)
            .HasForeignKey(block => block.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Vehicle>()
            .HasMany(vehicle => vehicle.EquipmentItems)
            .WithMany(equipment => equipment.Vehicles)
            .UsingEntity(join => join.ToTable("VehicleEquipment"));

        modelBuilder.Entity<Reservation>()
            .HasOne(reservation => reservation.Payment)
            .WithOne(payment => payment.Reservation)
            .HasForeignKey<Payment>(payment => payment.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Reservation>()
            .HasOne(reservation => reservation.Handover)
            .WithOne(handover => handover.Reservation)
            .HasForeignKey<RentalHandover>(handover => handover.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Reservation>()
            .HasOne(reservation => reservation.Dispute)
            .WithOne(dispute => dispute.Reservation)
            .HasForeignKey<Dispute>(dispute => dispute.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(review => review.Reservation)
            .WithMany(reservation => reservation.Reviews)
            .HasForeignKey(review => review.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(review => review.Reviewer)
            .WithMany(user => user.ReviewsWritten)
            .HasForeignKey(review => review.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(review => review.ReviewedUser)
            .WithMany(user => user.ReviewsReceived)
            .HasForeignKey(review => review.ReviewedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasIndex(review => new { review.ReservationId, review.ReviewerId })
            .IsUnique();

        modelBuilder.Entity<Dispute>()
            .HasOne(dispute => dispute.OpenedByUser)
            .WithMany(user => user.DisputesOpened)
            .HasForeignKey(dispute => dispute.OpenedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Dispute>()
            .HasOne(dispute => dispute.ResolvedByAdmin)
            .WithMany(user => user.DisputesResolvedAsAdmin)
            .HasForeignKey(dispute => dispute.ResolvedByAdminId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<RefreshToken>()
            .HasOne(refreshToken => refreshToken.User)
            .WithMany(user => user.RefreshTokens)
            .HasForeignKey(refreshToken => refreshToken.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(refreshToken => refreshToken.TokenHash)
            .IsUnique();
    }


}