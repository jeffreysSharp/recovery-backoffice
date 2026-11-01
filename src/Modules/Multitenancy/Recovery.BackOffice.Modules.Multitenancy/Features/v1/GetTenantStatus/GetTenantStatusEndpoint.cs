using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.Authorization;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.Dtos;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.v1.GetTenantStatus;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Recovery.BackOffice.Modules.Multitenancy.Features.v1.GetTenantStatus;

public static class GetTenantStatusEndpoint
{
    public static RouteHandlerBuilder Map(IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{id}/status", async (string id, IMediator mediator, CancellationToken cancellationToken) =>
                TypedResults.Ok(await mediator.Send(new GetTenantStatusQuery(id), cancellationToken)))
            .WithName("GetTenantStatus")
            .WithSummary("Get agency status")
            .WithDescription("Retrieve status information for an agency, including activation, validity, and basic metadata.")
            .RequirePermission(MultitenancyPermissions.Tenants.View)
            .Produces<TenantStatusDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
}