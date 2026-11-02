using Recovery.BackOffice.Modules.Billing.Contracts.Authorization;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Billing.Contracts.Dtos;
using Recovery.BackOffice.Modules.Billing.Contracts.v1.Usage;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Recovery.BackOffice.Modules.Billing.Features.v1.Usage.CaptureUsageSnapshots;

public static class CaptureUsageSnapshotsEndpoint
{
    internal static RouteHandlerBuilder MapCaptureUsageSnapshotsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/usage/snapshots/capture",
                async (CaptureUsageSnapshotsCommand command, IMediator mediator, CancellationToken ct) =>
                    Results.Ok(await mediator.Send(command, ct)))
            .WithName("CaptureUsageSnapshots")
            .WithSummary("Manually capture usage snapshots for an agency + period")
            .WithDescription("Ops endpoint wrapping IUsageReporter.CaptureForPeriodAsync. Idempotent: re-running for the same (agency, period) returns existing snapshots unchanged. Used for retroactive billing, debugging, and re-runs after fixes.")
            .RequirePermission(BillingPermissions.Manage)
            .WithIdempotency()
            .Produces<IReadOnlyList<UsageSnapshotDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}
