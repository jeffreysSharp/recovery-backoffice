using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Countries;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.CreateCountry;

public static class CreateCountryEndpoint
{
    internal static RouteHandlerBuilder MapCreateCountryEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost("/countries", async (CreateCountryCommand command, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(command, ct)))
        .WithName("CreateCountry")
        .WithSummary("CreateCountry")
        .RequirePermission(CatalogPermissions.Countries.Create)
        .WithIdempotency();
}
