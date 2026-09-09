using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Payments.Commands.CreatePayment;

public sealed class CreatePaymentCommandHandler
    : IRequestHandler<CreatePaymentCommand, CreatePaymentResult>
{
    private readonly IRepository<Reservation>
        _reservationRepository;
    private readonly IRepository<Payment> _paymentRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentCommandHandler(
        IRepository<Reservation> reservationRepository,
        IRepository<Payment> paymentRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _paymentRepository = paymentRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreatePaymentResult> Handle(
        CreatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        var renterId = _currentUserService.UserId;

        var reservation = _reservationRepository
            .Query()
            .FirstOrDefault(reservation =>
                reservation.Id == request.ReservationId &&
                reservation.RenterId == renterId);

        if (reservation is null)
        {
            throw new KeyNotFoundException(
                "Rezervacija nije pronađena.");
        }

        if (reservation.Status != ReservationStatus.Approved)
        {
            throw new InvalidOperationException(
                "Može se platiti samo odobrena rezervacija.");
        }

        var paymentAlreadyExists = _paymentRepository
            .Query()
            .Any(payment =>
                payment.ReservationId == reservation.Id);

        if (paymentAlreadyExists)
        {
            throw new InvalidOperationException(
                "Rezervacija je već plaćena.");
        }

        var paidAtUtc = DateTime.UtcNow;

        var payment = new Payment
        {
            ReservationId = reservation.Id,
            RentalAmount = reservation.RentalAmount,
            DepositAmount = reservation.DepositAmount,
            TotalAmount =
                reservation.RentalAmount +
                reservation.DepositAmount,
            Status = PaymentStatus.Paid,
            DepositStatus = DepositStatus.Held,
            PaymentMethod = request.PaymentMethod,
            CardLastFourDigits =
                request.CardNumber[^4..],
            PaidAtUtc = paidAtUtc
        };

        await _paymentRepository.AddAsync(
            payment,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreatePaymentResult(
            payment.Id,
            payment.ReservationId,
            payment.RentalAmount,
            payment.DepositAmount,
            payment.TotalAmount,
            payment.Status,
            payment.DepositStatus,
            payment.PaymentMethod,
            payment.CardLastFourDigits,
            paidAtUtc);
    }
}