using CarShare.Application.Common.Interfaces;
using CarShare.Application.Features.Reservations.Common;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Reservations.Queries.GetRentalContract;

public sealed class GetRentalContractQueryHandler
    : IRequestHandler<GetRentalContractQuery, byte[]>
{
    private readonly IRepository<Reservation>
        _reservationRepository;
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IRepository<User> _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRentalContractPdfService
        _pdfService;

    public GetRentalContractQueryHandler(
        IRepository<Reservation> reservationRepository,
        IRepository<Vehicle> vehicleRepository,
        IRepository<User> userRepository,
        ICurrentUserService currentUserService,
        IRentalContractPdfService pdfService)
    {
        _reservationRepository = reservationRepository;
        _vehicleRepository = vehicleRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _pdfService = pdfService;
    }

    public Task<byte[]> Handle(
        GetRentalContractQuery request,
        CancellationToken cancellationToken)
    {
        var reservation = _reservationRepository
            .Query()
            .FirstOrDefault(reservation =>
                reservation.Id == request.ReservationId)
            ?? throw new KeyNotFoundException(
                "Rezervacija nije pronađena.");

        var vehicle = _vehicleRepository
            .Query()
            .FirstOrDefault(vehicle =>
                vehicle.Id == reservation.VehicleId)
            ?? throw new KeyNotFoundException(
                "Vozilo nije pronađeno.");

        var currentUserId = _currentUserService.UserId;

        var canDownloadContract =
            reservation.RenterId == currentUserId ||
            vehicle.OwnerId == currentUserId;

        if (!canDownloadContract)
        {
            throw new UnauthorizedAccessException(
                "Ugovor mogu preuzeti samo renter ili vlasnik.");
        }

        var contractIsAvailable =
            reservation.Status == ReservationStatus.Approved ||
            reservation.Status == ReservationStatus.Active ||
            reservation.Status == ReservationStatus.Completed ||
            reservation.Status == ReservationStatus.Disputed;

        if (!contractIsAvailable)
        {
            throw new InvalidOperationException(
                "Ugovor je dostupan tek nakon odobravanja rezervacije.");
        }

        var owner = _userRepository
            .Query()
            .FirstOrDefault(user =>
                user.Id == vehicle.OwnerId)
            ?? throw new KeyNotFoundException(
                "Vlasnik vozila nije pronađen.");

        var renter = _userRepository
            .Query()
            .FirstOrDefault(user =>
                user.Id == reservation.RenterId)
            ?? throw new KeyNotFoundException(
                "Renter nije pronađen.");

        var contractData = new RentalContractData(
            reservation.Id,
            $"{owner.FirstName} {owner.LastName}",
            owner.Email,
            $"{renter.FirstName} {renter.LastName}",
            renter.Email,
            vehicle.Brand,
            vehicle.Model,
            reservation.StartDateTimeUtc,
            reservation.EndDateTimeUtc,
            reservation.RentalAmount,
            reservation.DepositAmount);

        var pdf = _pdfService.Generate(contractData);

        return Task.FromResult(pdf);
    }
}