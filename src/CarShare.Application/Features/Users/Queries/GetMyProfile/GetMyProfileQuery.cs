using MediatR;

namespace CarShare.Application.Features.Users.Queries.GetMyProfile;

public sealed record GetMyProfileQuery
    : IRequest<UserProfileViewModel>;