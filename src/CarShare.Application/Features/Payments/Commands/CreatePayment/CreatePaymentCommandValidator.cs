using FluentValidation;

namespace CarShare.Application.Features.Payments.Commands.CreatePayment;

public sealed class CreatePaymentCommandValidator
    : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty();

        RuleFor(x => x.PaymentMethod)
            .Equal("MockCard")
            .WithMessage(
                "Za studentski projekat koristimo MockCard.");

        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .Matches(@"^\d{16}$")
            .WithMessage(
                "Testni broj kartice mora imati 16 cifara.");
    }
}