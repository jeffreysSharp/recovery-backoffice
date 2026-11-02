using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Countries;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.GetCountryById;

public static class GetCountryByIdEndpoint
{
    internal static RouteHandlerBuilder MapGetCountryByIdEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet("/countries/{countryId:guid}", async (Guid countryId, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(new GetCountryByIdQuery(countryId), ct)))
        .WithName("GetCountryById")
        .WithSummary("GetCountryById")
        .RequirePermission(CatalogPermissions.Countries.View);
}
