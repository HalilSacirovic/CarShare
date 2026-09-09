using CarShare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CarShare.Application.Common.Interfaces;
using CarShare.Infrastructure.Repositories;
using CarShare.Application.Common.Authentication;
using CarShare.Infrastructure.Authentication;
using CarShare.Infrastructure.Services;

namespace CarShare.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' nije pronađen.");

        services.AddDbContext<CarShareDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                postgreSqlOptions =>
                    postgreSqlOptions.UseNetTopologySuite()));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.Configure<JwtSettings>(
        configuration.GetSection(JwtSettings.SectionName));

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<
    IVehicleAvailabilitySearchService,
    VehicleAvailabilitySearchService>();

        services.AddScoped<
    IRentalContractPdfService,
    RentalContractPdfService>();

        return services;
    }
}