using System.Data;
using System.Data.Common;
using System.Text.Json;
using CarShare.API.Contracts.Reservations;
using CarShare.Domain.Enums;
using CarShare.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarShare.API.Middleware;

public sealed class AvailabilityOverlapGuardMiddleware
{
    private readonly RequestDelegate _next;

    public AvailabilityOverlapGuardMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        CarShareDbContext dbContext)
    {
        if (!IsCreateReservationRequest(context.Request))
        {
            await _next(context);
            return;
        }

        context.Request.EnableBuffering();

        CreateReservationRequest? request;

        try
        {
            request =
                await JsonSerializer.DeserializeAsync<CreateReservationRequest>(
                    context.Request.Body,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    },
                    context.RequestAborted);
        }
        catch (JsonException)
        {
            context.Request.Body.Position = 0;
            await _next(context);
            return;
        }

        context.Request.Body.Position = 0;

        if (request is null ||
            request.VehicleId == Guid.Empty ||
            request.EndDateTimeUtc <= request.StartDateTimeUtc)
        {
            await _next(context);
            return;
        }

        var hasOverlap = await HasReservationOverlapAsync(
            dbContext,
            request,
            context.RequestAborted);

        if (hasOverlap)
        {
            context.Response.StatusCode =
                StatusCodes.Status409Conflict;

            await context.Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Termin nije dostupan",
                    Detail =
                        "Vozilo već ima rezervaciju koja se preklapa sa izabranim periodom.",
                    Instance = context.Request.Path
                },
                context.RequestAborted);

            return;
        }

        await _next(context);
    }

    private static bool IsCreateReservationRequest(
        HttpRequest request)
    {
        return HttpMethods.IsPost(request.Method) &&
               request.Path.Value?.EndsWith(
                   "/reservations",
                   StringComparison.OrdinalIgnoreCase) == true;
    }

    private static async Task<bool> HasReservationOverlapAsync(
        CarShareDbContext dbContext,
        CreateReservationRequest request,
        CancellationToken cancellationToken)
    {
        var connection = dbContext.Database.GetDbConnection();
        var shouldCloseConnection =
            connection.State != ConnectionState.Open;

        if (shouldCloseConnection)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();

            command.CommandText =
                """
                SELECT EXISTS (
                    SELECT 1
                    FROM "Reservations"
                    WHERE "VehicleId" = @vehicleId
                      AND "StartDateTimeUtc" < @endDateTimeUtc
                      AND "EndDateTimeUtc" > @startDateTimeUtc
                      AND (
                          "Status" = @approvedStatus
                          OR "Status" = @activeStatus
                          OR (
                              "Status" = @pendingStatus
                              AND "HoldExpiresAtUtc" > NOW()
                          )
                      )
                );
                """;

            AddParameter(
                command,
                "@vehicleId",
                request.VehicleId);

            AddParameter(
                command,
                "@startDateTimeUtc",
                request.StartDateTimeUtc);

            AddParameter(
                command,
                "@endDateTimeUtc",
                request.EndDateTimeUtc);

            AddParameter(
                command,
                "@pendingStatus",
                (int)ReservationStatus.Pending);

            AddParameter(
                command,
                "@approvedStatus",
                (int)ReservationStatus.Approved);

            AddParameter(
                command,
                "@activeStatus",
                (int)ReservationStatus.Active);

            var result = await command.ExecuteScalarAsync(
                cancellationToken);

            return result is true;
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await connection.CloseAsync();
            }
        }
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