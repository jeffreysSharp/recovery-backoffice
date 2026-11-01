using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Recovery.BackOffice.BuildingBlocks.Shared.Identity.Authorization;
using Recovery.BackOffice.BuildingBlocks.Web.Idempotency;
using Recovery.BackOffice.Modules.Catalog.Contracts.Authorization;
using Recovery.BackOffice.Modules.Catalog.Contracts.v1.Countries;

namespace Recovery.BackOffice.Modules.Catalog.Features.v1.Countries.UpdateCountry;

public static class UpdateCountryEndpoint
{
    internal static RouteHandlerBuilder MapUpdateCountryEndpoint(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapPut("/countries/{countryId:guid}", async (Guid countryId, UpdateCountryCommand body, IMediator mediator, CancellationToken ct) => Results.Ok(await mediator.Send(body with { CountryId = countryId }, ct)))
        .WithName("UpdateCountry")
        .WithSummary("UpdateCountry")
        .RequirePermission(CatalogPermissions.Countries.Update);
}
