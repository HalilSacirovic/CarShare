using MediatR;

namespace CarShare.Application.Features.Users.Queries.GetPublicUserProfile;

public sealed record GetPublicUserProfileQuery(
    Guid UserId)
    : IRequest<PublicUserProfileViewModel>;