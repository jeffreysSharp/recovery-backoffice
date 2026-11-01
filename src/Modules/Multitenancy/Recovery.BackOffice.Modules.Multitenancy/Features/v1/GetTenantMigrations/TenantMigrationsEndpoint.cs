using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Shared.Multitenancy;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.Authorization;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.Dtos;
using Recovery.BackOffice.Modules.Multitenancy.Contracts.v1.GetTenantMigrations;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Recovery.BackOffice.Modules.Multitenancy.Features.v1.GetTenantMigrations;

public static class TenantMigrationsEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet(
                "/migrations",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    IReadOnlyCollection<TenantMigrationStatusDto> result =
                        await mediator.Send(new GetTenantMigrationsQuery(), cancellationToken);

                    return TypedResults.Ok(result);
                })
            .WithName("GetTenantMigrations")
            .RequirePermission(MultitenancyPermissions.Tenants.View)
            .WithSummary("Get per-agency migration status")
            .WithDescription("Retrieve migration status for each agency, including pending migrations and provider information.")
            .Produces<IReadOnlyCollection<TenantMigrationStatusDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}