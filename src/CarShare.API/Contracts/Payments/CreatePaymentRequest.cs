namespace CarShare.API.Contracts.Payments;

public sealed record CreatePaymentRequest(
    string PaymentMethod,
    string CardNumber);