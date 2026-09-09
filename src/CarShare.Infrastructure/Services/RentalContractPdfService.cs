using CarShare.Application.Common.Interfaces;
using CarShare.Application.Features.Reservations.Common;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CarShare.Infrastructure.Services;

public sealed class RentalContractPdfService
    : IRentalContractPdfService
{
    public RentalContractPdfService()
    {
        QuestPDF.Settings.License =
            LicenseType.Community;
    }

    public byte[] Generate(
        RentalContractData contractData)
    {
        var totalAmount =
            contractData.RentalAmount +
            contractData.DepositAmount;

        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);

                page.DefaultTextStyle(style =>
                    style.FontSize(12));

                page.Header()
                    .AlignCenter()
                    .Text("CARSHARE - UGOVOR O IZNAJMLJIVANJU")
                    .SemiBold()
                    .FontSize(18);

                page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        column.Spacing(10);

                        column.Item().Text(
                            $"Broj rezervacije: " +
                            $"{contractData.ReservationId}");

                        column.Item().Text(
                            $"Vlasnik: {contractData.OwnerName}");

                        column.Item().Text(
                            $"Email vlasnika: " +
                            $"{contractData.OwnerEmail}");

                        column.Item().Text(
                            $"Renter: {contractData.RenterName}");

                        column.Item().Text(
                            $"Email rentera: " +
                            $"{contractData.RenterEmail}");

                        column.Item().Text(
                            $"Vozilo: {contractData.VehicleBrand} " +
                            $"{contractData.VehicleModel}");

                        column.Item().Text(
                            $"Pocetak: " +
                            $"{contractData.StartDateTimeUtc:u}");

                        column.Item().Text(
                            $"Kraj: " +
                            $"{contractData.EndDateTimeUtc:u}");

                        column.Item().Text(
                            $"Cena najma: " +
                            $"{contractData.RentalAmount:0.00} EUR");

                        column.Item().Text(
                            $"Depozit: " +
                            $"{contractData.DepositAmount:0.00} EUR");

                        column.Item().Text(
                            $"Ukupno: {totalAmount:0.00} EUR");

                        column.Item()
                            .PaddingTop(20)
                            .Text(
                                "Ovaj dokument je probni ugovor " +
                                "generisan u okviru studentskog " +
                                "CarShare projekta.");
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(
                        $"Generisano: {DateTime.UtcNow:u}");
            });
        }).GeneratePdf();
    }
}