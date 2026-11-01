using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Cities;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Cities.GetCityById;

public static class GetCityByIdEndpoint
{
    internal static RouteHandlerBuilder MapGetCityByIdEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet("/cities/{cityId:guid}", async (Guid cityId, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(new GetCityByIdQuery(cityId), ct)))
        .WithName("GetCityById")
        .WithSummary("Get a city")
        .RequirePermission(CatalogPermissions.Cities.View);
}
