using System.Data;
using System.Data.Common;
using CarShare.Application.Common.Interfaces;
using CarShare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarShare.Infrastructure.Services;

public sealed class VehicleAvailabilitySearchService
    : IVehicleAvailabilitySearchService
{
    private readonly CarShareDbContext _dbContext;

    public VehicleAvailabilitySearchService(
        CarShareDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Guid>>
        FindAvailableVehicleIdsAsync(
            DateTime startDateTimeUtc,
            DateTime endDateTimeUtc,
            double latitude,
            double longitude,
            double radiusKilometers,
            CancellationToken cancellationToken)
    {
        var vehicleIds = new List<Guid>();

        var connection =
            _dbContext.Database.GetDbConnection();

        var shouldCloseConnection =
            connection.State != ConnectionState.Open;

        if (shouldCloseConnection)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command =
                connection.CreateCommand();

            command.CommandText =
                """
                SELECT v."Id"
                FROM "Vehicles" AS v
                WHERE v."Status" = 0
                  AND v."VerificationStatus" = 2
                  AND ST_DWithin(
                      ST_SetSRID(
                          ST_MakePoint(
                              v."Longitude",
                              v."Latitude"),
                          4326)::geography,
                      ST_SetSRID(
                          ST_MakePoint(
                              @longitude,
                              @latitude),
                          4326)::geography,
                      @radiusMeters)
                  AND NOT EXISTS
                  (
                      SELECT 1
                      FROM "AvailabilityBlocks" AS ab
                      WHERE ab."VehicleId" = v."Id"
                        AND ab."StartDateTimeUtc" < @endDateTimeUtc
                        AND ab."EndDateTimeUtc" > @startDateTimeUtc
                  )
                  AND NOT EXISTS
                  (
                      SELECT 1
                      FROM "Reservations" AS r
                      WHERE r."VehicleId" = v."Id"
                        AND r."StartDateTimeUtc" < @endDateTimeUtc
                        AND r."EndDateTimeUtc" > @startDateTimeUtc
                        AND
                        (
                            r."Status" IN (1, 3)
                            OR
                            (
                                r."Status" = 0
                                AND r."HoldExpiresAtUtc" > @nowUtc
                            )
                        )
                  );
                """;

            AddParameter(
                command,
                "@startDateTimeUtc",
                startDateTimeUtc.ToUniversalTime());

            AddParameter(
                command,
                "@endDateTimeUtc",
                endDateTimeUtc.ToUniversalTime());

            AddParameter(command, "@latitude", latitude);
            AddParameter(command, "@longitude", longitude);

            AddParameter(
                command,
                "@radiusMeters",
                radiusKilometers * 1000);

            AddParameter(
                command,
                "@nowUtc",
                DateTime.UtcNow);

            await using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                vehicleIds.Add(reader.GetGuid(0));
            }
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await connection.CloseAsync();
            }
        }

        return vehicleIds;
    }

    private static void AddParameter(
        DbCommand command,
        string name,
        object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}