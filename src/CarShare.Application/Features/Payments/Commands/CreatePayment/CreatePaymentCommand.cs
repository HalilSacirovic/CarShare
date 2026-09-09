using MediatR;

namespace CarShare.Application.Features.Payments.Commands.CreatePayment;

public sealed record CreatePaymentCommand(
    Guid ReservationId,
    string PaymentMethod,
    string CardNumber) : IRequest<CreatePaymentResult>;