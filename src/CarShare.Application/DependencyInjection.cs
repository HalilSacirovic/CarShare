using System.Reflection;
using CarShare.Application.Common.Behaviors;
using CarShare.Application.Features.Users.Queries.GetPublicUserProfile;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CarShare.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(
                assembly);

            configuration.AddOpenBehavior(
                typeof(ValidationBehavior<,>));
        });

        services.AddTransient<
            IRequestHandler<
                GetPublicUserProfileQuery,
                PublicUserProfileViewModel>,
            GetPublicUserProfileQueryHandler>();

        services.AddValidatorsFromAssembly(assembly);

        services.AddAutoMapper(
            configuration => { },
            assembly);

        return services;
    }
}