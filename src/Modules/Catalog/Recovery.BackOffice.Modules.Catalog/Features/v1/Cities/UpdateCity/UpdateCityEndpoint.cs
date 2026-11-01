using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Cities;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Cities.UpdateCity;

public static class UpdateCityEndpoint
{
    internal static RouteHandlerBuilder MapUpdateCityEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapPut("/cities/{cityId:guid}", async (Guid cityId, UpdateCityCommand body, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(body with { CityId = cityId }, ct)))
        .WithName("UpdateCity")
        .WithSummary("Update a city")
        .RequirePermission(CatalogPermissions.Cities.Update);
}
