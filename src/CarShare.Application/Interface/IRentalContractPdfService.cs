using CarShare.Application.Features.Reservations.Common;

namespace CarShare.Application.Common.Interfaces;

public interface IRentalContractPdfService
{
    byte[] Generate(RentalContractData contractData);
}