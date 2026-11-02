using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Countries;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.SearchCountries;

public static class SearchCountriesEndpoint
{
    internal static RouteHandlerBuilder MapSearchCountriesEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet("/countries", async (string? search, bool? isActive, int? pageNumber, int? pageSize, string? sortBy, string? sortDir, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(new SearchCountriesQuery(search, isActive, pageNumber ?? 1, pageSize ?? 20, sortBy, sortDir), ct)))
        .WithName("SearchCountries")
        .WithSummary("SearchCountries")
        .RequirePermission(CatalogPermissions.Countries.View);
}
