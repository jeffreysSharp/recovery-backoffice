using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.Authorization;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.Dtos;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.v1.GetTenantTheme;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Recovery.BackOffice.Modules.Multitenancy.Features.v1.GetTenantTheme;

public static class GetTenantThemeEndpoint
{
    public static RouteHandlerBuilder Map(IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/theme", async (IMediator mediator, CancellationToken cancellationToken) =>
                TypedResults.Ok(await mediator.Send(new GetTenantThemeQuery(), cancellationToken)))
            .WithName("GetTenantTheme")
            .WithSummary("Get current agency theme")
            .WithDescription("Retrieve the theme settings for the current agency, including colors, typography, and brand assets.")
            .RequirePermission(MultitenancyPermissions.Tenants.ViewTheme)
            .Produces<TenantThemeDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}