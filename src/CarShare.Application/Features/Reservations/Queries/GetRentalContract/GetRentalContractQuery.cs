using MediatR;

namespace CarShare.Application.Features.Reservations.Queries.GetRentalContract;

public sealed record GetRentalContractQuery(
    Guid ReservationId) : IRequest<byte[]>;