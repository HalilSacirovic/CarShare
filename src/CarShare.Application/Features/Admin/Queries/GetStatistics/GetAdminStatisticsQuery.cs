using MediatR;

namespace CarShare.Application.Features.Admin.Queries.GetStatistics;

public sealed record GetAdminStatisticsQuery
    : IRequest<AdminStatisticsViewModel>;