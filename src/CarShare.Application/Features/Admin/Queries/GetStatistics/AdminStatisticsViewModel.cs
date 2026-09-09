namespace CarShare.Application.Features.Admin.Queries.GetStatistics;

public sealed record AdminStatisticsViewModel(
    int TotalUsers,
    int TotalVehicles,
    int TotalReservations,
    int OpenDisputes,
    int CompletedReservations,
    decimal TotalMockRevenue);