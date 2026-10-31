using Recovery.BackOffice.Modules.Auditing.Contracts.Authorization;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.Modules.Auditing.Contracts.Dtos;
using Recovery.BackOffice.Modules.Auditing.Contracts.v1.GetAuditById;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Recovery.BackOffice.Modules.Auditing.Features.v1.GetAuditById;

public static class GetAuditByIdEndpoint
{
    public static RouteHandlerBuilder MapGetAuditByIdEndpoint(this IEndpointRouteBuilder group)
    {
        return group.MapGet(
                "/{id:guid}",
                async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
                    TypedResults.Ok(await mediator.Send(new GetAuditByIdQuery(id), cancellationToken)))
            .WithName("GetAuditById")
            .WithSummary("Get audit event by ID")
            .WithDescription("Retrieve full details for a single audit event.")
            .RequirePermission(AuditingPermissions.AuditTrails.View)
            .Produces<AuditDetailDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
}