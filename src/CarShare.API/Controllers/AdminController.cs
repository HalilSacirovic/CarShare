using Asp.Versioning;
using CarShare.Application.Features.Admin.Queries.GetPendingDriverVerifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShare.Application.Features.Admin.Commands.ApproveDriverVerification;
using CarShare.Application.Features.Admin.Commands.RejectDriverVerification;
using CarShare.Application.Features.Admin.Queries.GetPendingVehicleVerifications;
using CarShare.Application.Features.Admin.Commands.ApproveVehicleVerification;
using CarShare.Application.Features.Admin.Commands.RejectVehicleVerification;
using CarShare.Application.Features.Admin.Queries.GetOpenDisputes;
using CarShare.API.Contracts.Disputes;
using CarShare.Application.Features.Admin.Commands.ResolveDispute;
using CarShare.Application.Features.Admin.Commands.RejectDispute;
using CarShare.Application.Features.Admin.Queries.GetStatistics;

namespace CarShare.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/admin")]
[Authorize(Roles = "Admin")]
public sealed class AdminController : ControllerBase
{
    private readonly ISender _sender;

    public AdminController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("driver-verifications/pending")]
    [ProducesResponseType(
        typeof(IReadOnlyList<PendingDriverVerificationViewModel>),
        StatusCodes.Status200OK)]
    public async Task<
        ActionResult<
            IReadOnlyList<PendingDriverVerificationViewModel>>>
        GetPendingDriverVerifications(
            CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetPendingDriverVerificationsQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpPatch("driver-verifications/{userId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApproveDriverVerification(
    Guid userId,
    CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ApproveDriverVerificationCommand(userId),
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("driver-verifications/{userId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RejectDriverVerification(
    Guid userId,
    CancellationToken cancellationToken)
    {
        await _sender.Send(
            new RejectDriverVerificationCommand(userId),
            cancellationToken);

        return NoContent();
    }

    [HttpGet("vehicle-verifications/pending")]
    [ProducesResponseType(
    typeof(IReadOnlyList<PendingVehicleVerificationViewModel>),
    StatusCodes.Status200OK)]
    public async Task<
    ActionResult<IReadOnlyList<PendingVehicleVerificationViewModel>>>
    GetPendingVehicleVerifications(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetPendingVehicleVerificationsQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpPatch("vehicle-verifications/{vehicleId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApproveVehicleVerification(
    Guid vehicleId,
    CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ApproveVehicleVerificationCommand(vehicleId),
            cancellationToken);

        return NoContent();
    }
    [HttpPatch("vehicle-verifications/{vehicleId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RejectVehicleVerification(
    Guid vehicleId,
    CancellationToken cancellationToken)
    {
        await _sender.Send(
            new RejectVehicleVerificationCommand(vehicleId),
            cancellationToken);

        return NoContent();
    }

    [HttpGet("disputes/open")]
    [ProducesResponseType(
    typeof(IReadOnlyList<OpenDisputeViewModel>),
    StatusCodes.Status200OK)]
    public async Task<
    ActionResult<IReadOnlyList<OpenDisputeViewModel>>>
    GetOpenDisputes(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetOpenDisputesQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpPatch("disputes/{disputeId:guid}/resolve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ResolveDispute(
    Guid disputeId,
    ResolveDisputeRequest request,
    CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ResolveDisputeCommand(
                disputeId,
                request.AdminDecision),
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("disputes/{disputeId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RejectDispute(
    Guid disputeId,
    ResolveDisputeRequest request,
    CancellationToken cancellationToken)
    {
        await _sender.Send(
            new RejectDisputeCommand(
                disputeId,
                request.AdminDecision),
            cancellationToken);

        return NoContent();
    }

    [HttpGet("statistics")]
    [ProducesResponseType(
    typeof(AdminStatisticsViewModel),
    StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminStatisticsViewModel>>
    GetStatistics(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAdminStatisticsQuery(),
            cancellationToken);

        return Ok(result);
    }
}