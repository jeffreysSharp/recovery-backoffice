using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.Authorization;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.Dtos;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.v1.TenantProvisioning;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Recovery.BackOffice.Modules.Multitenancy.Features.v1.TenantProvisioning.RetryTenantProvisioning;

public static class RetryTenantProvisioningEndpoint
{
    public static RouteHandlerBuilder Map(IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/{tenantId}/provisioning/retry", async (
            [FromRoute] string tenantId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken) =>
            TypedResults.Ok(await mediator.Send(new RetryTenantProvisioningCommand(tenantId), cancellationToken)))
            .WithName("RetryTenantProvisioning")
            .WithSummary("Retry agency provisioning")
            .RequirePermission(MultitenancyPermissions.Tenants.Update)
            .WithDescription("Retry the provisioning workflow for an agency.")
            .Produces<TenantProvisioningStatusDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
}