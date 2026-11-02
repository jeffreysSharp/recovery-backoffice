using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Cities;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Cities.CreateCity;

public static class CreateCityEndpoint
{
    internal static RouteHandlerBuilder MapCreateCityEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost("/cities", async (CreateCityCommand command, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(command, ct)))
        .WithName("CreateCity")
        .WithSummary("Create a city")
        .RequirePermission(CatalogPermissions.Cities.Create)
        .WithIdempotency();
}
