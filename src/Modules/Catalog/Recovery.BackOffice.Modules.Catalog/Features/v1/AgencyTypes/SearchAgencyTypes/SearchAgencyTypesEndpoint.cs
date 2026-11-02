using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.AgencyTypes;
namespace Recovery.BackOffice.Modules.Catalog.Features.v1.AgencyTypes.SearchAgencyTypes;
public static class SearchAgencyTypesEndpoint
{
    internal static RouteHandlerBuilder MapSearchAgencyTypesEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet("/agency-types", async (string? search, bool? isActive, int? pageNumber, int? pageSize, string? sortBy, string? sortDir, IMediator mediator, CancellationToken ct) =>
            Results.Ok(await mediator.Send(new SearchAgencyTypesQuery(search, isActive, pageNumber ?? 1, pageSize ?? 20, sortBy, sortDir), ct)))
        .WithName("SearchAgencyTypes").WithSummary("Search agency types (paged, sortable)").RequirePermission(CatalogPermissions.AgencyTypes.View);
}
