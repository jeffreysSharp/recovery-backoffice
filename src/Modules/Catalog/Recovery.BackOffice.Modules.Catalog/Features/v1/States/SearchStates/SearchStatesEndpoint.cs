using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.States;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.States.SearchStates;

public static class SearchStatesEndpoint
{
    internal static RouteHandlerBuilder MapSearchStatesEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet("/states", async (string? search, Guid? countryId, bool? isActive, int? pageNumber, int? pageSize, string? sortBy, string? sortDir, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(new SearchStatesQuery(search, countryId, isActive, pageNumber ?? 1, pageSize ?? 20, sortBy, sortDir), ct)))
        .WithName("SearchStates")
        .WithSummary("Search states (paged, sortable)")
        .RequirePermission(CatalogPermissions.States.View);
}
