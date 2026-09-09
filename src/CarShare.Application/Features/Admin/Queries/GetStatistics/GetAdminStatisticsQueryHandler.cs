using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using MediatR;

namespace CarShare.Application.Features.Admin.Queries.GetStatistics;

public sealed class GetAdminStatisticsQueryHandler
    : IRequestHandler<
        GetAdminStatisticsQuery,
        AdminStatisticsViewModel>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IRepository<Reservation> _reservationRepository;
    private readonly IRepository<Dispute> _disputeRepository;
    private readonly IRepository<Payment> _paymentRepository;

    public GetAdminStatisticsQueryHandler(
        IRepository<User> userRepository,
        IRepository<Vehicle> vehicleRepository,
        IRepository<Reservation> reservationRepository,
        IRepository<Dispute> disputeRepository,
        IRepository<Payment> paymentRepository)
    {
        _userRepository = userRepository;
        _vehicleRepository = vehicleRepository;
        _reservationRepository = reservationRepository;
        _disputeRepository = disputeRepository;
        _paymentRepository = paymentRepository;
    }

    public Task<AdminStatisticsViewModel> Handle(
        GetAdminStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        var totalUsers = _userRepository.Query().Count();
        var totalVehicles = _vehicleRepository.Query().Count();

        var totalReservations =
            _reservationRepository.Query().Count();

        var openDisputes = _disputeRepository
            .Query()
            .Count(dispute =>
                dispute.Status == DisputeStatus.Open ||
                dispute.Status == DisputeStatus.UnderReview);

        var completedReservations = _reservationRepository
            .Query()
            .Count(reservation =>
                reservation.Status ==
                    ReservationStatus.Completed);

        var totalMockRevenue = _paymentRepository
            .Query()
            .Where(payment =>
                payment.Status == PaymentStatus.Paid)
            .Sum(payment => (decimal?)payment.TotalAmount)
            ?? 0;

        var result = new AdminStatisticsViewModel(
            totalUsers,
            totalVehicles,
            totalReservations,
            openDisputes,
            completedReservations,
            totalMockRevenue);

        return Task.FromResult(result);
    }
}