using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Cities;
namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Cities.SearchCities;
public static class SearchCitiesEndpoint
{
    internal static RouteHandlerBuilder MapSearchCitiesEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet("/cities", async (string? search, Guid? countryId, Guid? stateId, bool? isActive, int? pageNumber, int? pageSize, string? sortBy, string? sortDir, IMediator mediator, CancellationToken ct) =>
            Results.Ok(await mediator.Send(new SearchCitiesQuery(search, countryId, stateId, isActive, pageNumber ?? 1, pageSize ?? 20, sortBy, sortDir), ct)))
        .WithName("SearchCities").WithSummary("Search cities (paged, sortable)").RequirePermission(CatalogPermissions.Cities.View);
}
